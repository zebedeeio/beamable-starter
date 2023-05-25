using Beamable.Api;
using Beamable.Api.Autogenerated.Models;
using Beamable.Api.Caches;
using Beamable.Api.Connectivity;
using Beamable.Api.Inventory;
using Beamable.Common;
using Beamable.Common.Api;
using Beamable.Common.Api.Content;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Api.Notifications;
using Beamable.Common.Content;
using Beamable.Common.Dependencies;
using Beamable.Common.Inventory;
using Beamable.Coroutines;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CurrencyProperty = Beamable.Api.Autogenerated.Models.CurrencyProperty;
using IInventoryApi = Beamable.Api.Autogenerated.Inventory.IInventoryApi;
using InventoryView = Beamable.Api.Autogenerated.Models.InventoryView;
using ItemGroup = Beamable.Api.Autogenerated.Models.ItemGroup;

namespace Beamable.Player
{
	[Serializable]
	public class SerializedDictionaryStringToPlayerItemGroup : SerializableDictionaryStringToSomething<PlayerItemGroup>
	{

	}

	[Serializable]
	public class SerializedDictionaryStringToPlayerCurrencyGroup : SerializableDictionaryStringToSomething<PlayerCurrencyGroup>
	{

	}
	[Serializable]
	public class PlayerItemTrieEntry : TrieSerializationEntry<PlayerItem> { }

	[Serializable]
	public class PlayerCurrencyTrieEntry : TrieSerializationEntry<PlayerCurrency> { }

	[Serializable]
	public class PlayerItemTrie : Trie<PlayerItem, PlayerItemTrieEntry>
	{

	}

	[Serializable]
	public class PlayerCurrencyTrie : Trie<PlayerCurrency, PlayerCurrencyTrieEntry>
	{

	}

	/// <summary>
	/// <para>
	/// The player's inventory can have <see cref="Currencies"/>, and items, which can be accessed via <see cref="GetItems"/>
	/// </para>
	/// </summary>
	[Serializable]
	public class PlayerInventory : IStorageHandler<PlayerInventory>, IServiceStorable
	{
		private readonly InventoryService _inventoryApi;
		private readonly IInventoryApi _openApi;
		private readonly IUserContext _userContext;
		private readonly Debouncer _debouncer;
		private readonly INotificationService _notificationService;
		private readonly CoreConfiguration _config;
		private readonly IContentApi _contentService;
		private readonly ISdkEventService _sdkEventService;
		private readonly SdkEventConsumer _consumer;
		private HashSet<string> nextScopes = new HashSet<string>();

		public PlayerCurrencyGroup Currencies { get; }

		/// <summary>
		/// The local items represent the current items that the SDK has subscribed to.
		/// This structure is not observable. Consider using the <see cref="GetItems"/> function to get an observable, serializable <see cref="PlayerItemGroup"/>.
		/// Modifications to this structure will not update the player's inventory.
		/// To update the items, use the <see cref="Update(System.Action{Beamable.Common.Api.Inventory.InventoryUpdateBuilder},string)"/> method.
		/// </summary>
		public PlayerItemTrie LocalItems => localItems;

		/// <summary>
		/// The local currencies represent the current currencies that the SDK has subscribed to.
		/// This structure is not observable. Consider using the <see cref="GetCurrencies"/> function to get an observable, serializable <see cref="PlayerCurrencyGroup"/>.
		/// Modifications to this structure will not update the player's inventory.
		/// To update the currencies, use the <see cref="Update(System.Action{Beamable.Common.Api.Inventory.InventoryUpdateBuilder},string)"/> method.
		/// </summary>
		public PlayerCurrencyTrie LocalCurrencies => localCurrencies;

		[SerializeField]
		private PlayerItemTrie localItems;

		[SerializeField]
		private PlayerCurrencyTrie localCurrencies;

		private IntTrie _requestCounter = new IntTrie();

		private SerializedDictionaryStringToPlayerItemGroup _scopeToItemGroup = new SerializedDictionaryStringToPlayerItemGroup();
		private SerializedDictionaryStringToPlayerCurrencyGroup _scopeToCurrencyGroup = new SerializedDictionaryStringToPlayerCurrencyGroup();
		private InventoryUpdateBuilder _delayedBuilder = new InventoryUpdateBuilder();
		private Promise _delayedUpdatePromise;


		[SerializeField]
		private SerializableDictionaryStringToString _itemIdToReqId = new SerializableDictionaryStringToString();
		[SerializeField]
		private long _nextOfflineItemId;
		[SerializeField]
		private InventoryUpdateBuilder _offlineUpdate = new InventoryUpdateBuilder();

		private StorageHandle<PlayerInventory> _saveHandle;
		private Promise _pendingUpdate;

		public PlayerInventory(
			IPlatformService platformService,
			INotificationService notificationService,
			IDependencyProvider provider,
			CoreConfiguration config,
			IContentApi contentService,
			ISdkEventService sdkEventService,
			IInventoryApi openApi,
			IUserContext userContext,
			Debouncer debouncer)
		{
			_openApi = openApi;
			_userContext = userContext;
			_debouncer = debouncer;
			_inventoryApi = provider.GetService<CachelessInventoryService>();
			_notificationService = notificationService;
			_config = config;
			_contentService = contentService;
			_sdkEventService = sdkEventService;

			localCurrencies = new PlayerCurrencyTrie();
			localItems = new PlayerItemTrie();

			Currencies = GetCurrencies();
			_notificationService.Subscribe<InventoryScopeNotification>("inventory.refresh", OnInventoryUpdated);
			provider.GetService<BeamContext>().OnUserLoggingOut += (_) =>
			{
				// before the context actually changes, save the old data.
				_saveHandle?.Save();
			};
			platformService.OnReloadUser += () =>
			{
				// Now that the user has changed, load and apply the latest data... 
				_saveHandle?.Load();
				var _ = Refresh();
			};

			_consumer = _sdkEventService.Register(nameof(PlayerInventory), HandleEvent);
		}

		public void ReceiveStorageHandle(StorageHandle<PlayerInventory> handle)
		{
			_saveHandle = handle;
		}

		[Serializable]
		private class InventoryScopeNotification
		{
			public string[] scopes;
		}

		private class InventoryViewResult
		{
			public InventoryView view;
			public bool isOffline;
		}

		/// <summary>
		/// Refresh the current items and currencies based on the given <see cref="scopes"/>.
		/// If no calls to <see cref="GetItems"/> or <see cref="GetCurrencies"/> have been made,
		/// then no scopes have been subscribed and therefor, no scopes will actually be updated.
		/// </summary>
		/// <param name="scopes"></param>
		public async Promise Refresh(params string[] scopes)
		{
			foreach (var scope in scopes)
			{
				nextScopes.Add(scope);
			}
			await _debouncer.SetTimeout(GetLatestInventory);
		}

		private void OnInventoryUpdated(InventoryScopeNotification update)
		{
			var _ = Refresh(update.scopes);
		}

		private async Promise<InventoryViewResult> DownloadInventoryData(string[] scopes)
		{
			var result = new InventoryViewResult();
			var req = new InventoryQueryRequest();
			req.scopes.Set(scopes);

			try
			{
				result.view = await _openApi.ObjectPost(_userContext.UserId, req);
			}
			catch (NoConnectivityException)
			{
				result.isOffline = true;
			}
			return result;


		}

		private async Promise GetLatestInventory()
		{
			try
			{
				var scopes = nextScopes.ToArray();
				nextScopes.Clear();

				// we only want to get scopes that are relevant to the currently requested data
				var filteredScopes = _requestCounter.GetRelevantKeys(scopes).ToArray();
				filteredScopes = filteredScopes.Where(scope => _requestCounter.GetExact(scope).Count > 0).ToArray();

				if (filteredScopes.Length == 0) return; // if there are no scopes, there is nothing to download. But actually, the API treats an empty scope as "everything", which is extra bad for us.

				var result = await DownloadInventoryData(filteredScopes);

				if (result.isOffline)
				{
					// if we are offline, then the trie already has the data, and there is nothing to do. 
					foreach (var kvp in _scopeToItemGroup)
					{
						kvp.Value.Notify();
					}
					foreach (var kvp in _scopeToCurrencyGroup)
					{
						kvp.Value.Notify();
					}
					return;
				}

				var unseenScopes = new HashSet<string>(scopes);

				var itemGroupsToUpdate = new HashSet<PlayerItemGroup>();
				var currGroupsToUpdate = new HashSet<PlayerCurrencyGroup>();
				var res = result.view;

				#region update or create items
				foreach (var group in res.items)
				{
					unseenScopes.Remove(group.id); // mark this scope of items as "seen"

					var plrItems = new PlayerItem[group.items.Length];
					var contentRef = new ItemRef(group.id);
					var content = await contentRef.Resolve().Recover(_ => null); ;
					var seen = new HashSet<PlayerItem>();

					// create a lookup table from item-id to existing PlayerItem. We don't want to re-create instances if they already exist by id.
					var existingItems = localItems.GetExact(group.id);
					var idToExistingItem = new Dictionary<int, PlayerItem>();
					foreach (var item in existingItems)
					{
						idToExistingItem[item.UniqueCode] = item;
					}

					// iterate through all the latest items in this group.
					for (var i = 0; i < group.items.Length; i++)
					{
						var itemId = group.items[i].id;
						var code = ((itemId.GetHashCode() << 5) + itemId.GetHashCode()) ^ group.id.GetHashCode();

						if (idToExistingItem.TryGetValue(code, out var existingItem))
						{
							// this item already exists, so we need to update the existing instance in place, and then trigger an update.
							seen.Add(existingItem);
							plrItems[i] = existingItem;
							existingItem.Content = content;
							existingItem.CreatedAt = group.items[i].createdAt.GetOrElse(0);
							existingItem.UpdatedAt = group.items[i].updatedAt.GetOrElse(0);
							existingItem.Properties.Clear();
							foreach (var property in group.items[i].properties)
							{
								existingItem.Properties[property.name] = property.value;
							}

							existingItem.TriggerUpdate();
						}
						else // when the new item isn't represented locally yet, we need to allocate a new PlayerItem
						{
							var newItem = plrItems[i] = new PlayerItem
							{
								Content = content,
								ItemId = itemId,
								ContentId = group.id,
								CreatedAt = group.items[i].createdAt.GetOrElse(0),
								UpdatedAt = group.items[i].updatedAt.GetOrElse(0),
								UniqueCode = code
							};
							foreach (var property in group.items[i].properties)
							{
								newItem.Properties[property.name] = property.value;
							}

							newItem.TriggerUpdate();
						}

					}

					// any item that wasn't seen in the latest server set needs to be removed.
					var unseen = existingItems.Except(seen);
					foreach (var item in unseen)
					{
						item.TriggerDeletion();
					}

					// commit the data to the local "database", trie. 
					localItems.SetRange(group.id, plrItems);
				}
				#endregion

				#region handle the deletion of entire item groups
				foreach (var deletedScope in unseenScopes)
				{
					var removed = localItems.GetExact(deletedScope);
					foreach (var removedItem in removed)
					{
						removedItem.TriggerDeletion();
					}

					localItems.ClearExact(deletedScope);
					foreach (var node in localItems.Traverse(deletedScope))
					{
						if (_scopeToItemGroup.TryGetValue(node.path, out var existingGroup))
						{
							itemGroupsToUpdate.Add(existingGroup);
						}
					}
				}
				#endregion

				#region update or create currencies

				// create a lookup table of existing currencies
				var existingCurrs = localCurrencies.GetAll("currency");
				var idToExistingCurr = new Dictionary<string, PlayerCurrency>();
				foreach (var item in existingCurrs)
				{
					idToExistingCurr[item.CurrencyId] = item;
				}

				foreach (var currency in res.currencies)
				{
					var currRef = new CurrencyRef(currency.id);
					var content = await currRef.Resolve().Recover(_ => null);
					if (idToExistingCurr.TryGetValue(currency.id, out var existing))
					{
						// the server currency already exited locally, so we can update it in place
						existing.Content = content;
						existing.Properties.Clear();
						foreach (var property in currency.properties)
						{
							existing.Properties[property.name] = property.value;
						}
						existing.Amount = currency.amount;
					}
					else // when the currency isn't represented locally yet, we need to allocate one
					{
						existing = new PlayerCurrency
						{
							Content = content,
							Amount = currency.amount,
							CurrencyId = currency.id,
						};
						foreach (var property in currency.properties)
						{
							existing.Properties[property.name] = property.value;
						}

						idToExistingCurr[currency.id] = existing;
						localCurrencies.Insert(currency.id, existing);
					}

				}
				#endregion

				#region handle group updates
				// now that the trie has been updated, notify all itemGroups that their data needs to update
				foreach (var group in res.items)
				{
					foreach (var node in localItems.Traverse(group.id))
					{
						if (_scopeToItemGroup.TryGetValue(node.path, out var existingGroup))
						{
							itemGroupsToUpdate.Add(existingGroup);
						}
					}
				}

				foreach (var group in res.currencies)
				{
					foreach (var node in localCurrencies.Traverse(group.id))
					{
						if (_scopeToCurrencyGroup.TryGetValue(node.path, out var existingGroup))
						{
							currGroupsToUpdate.Add(existingGroup);
						}
					}
				}

				// send a notification to all item groups that it should re-check its data against the trie
				foreach (var group in itemGroupsToUpdate)
				{
					group.Notify();
				}

				// send a notification to all currency groups that it should re-check its data against the trie
				foreach (var group in currGroupsToUpdate)
				{
					group.Notify();
				}
				#endregion
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
				throw;
			}
		}


		/// <summary>
		/// When the player inventory is initialized, shortly afterwards, this function will be called
		/// to re-load the previous state.
		/// </summary>
		public void OnAfterLoadState()
		{
			// now we need to try applying any old state we had
			var _ = _consumer.RunAfterReconnection(new SdkEvent(nameof(PlayerInventory), "commit"));
		}

		/// <summary>
		/// Get a player's currency data for a given type.
		/// </summary>
		/// <param name="currencyRef">A <see cref="CurrencyRef"/> for the type of currency to get. </param>
		/// <returns>A <see cref="PlayerCurrency"/> object for the given currencyRef</returns>
		public PlayerCurrency GetCurrency(CurrencyRef currencyRef)
		{
			var group = GetCurrencies(currencyRef);
			if (group.Count == 0)
			{
				// need to create...
				localCurrencies.Insert(currencyRef, new PlayerCurrency
				{
					CurrencyId = currencyRef.Id,
					Properties = new SerializableDictionaryStringToString(),
					Amount = 0, // TODO: if we had content, we could know the starting amount.
					Content = null
				});
				group.Notify();
				var _ = group.Refresh();
			}

			return group[0];
		}

		/// <summary>
		/// Get a category of <see cref="PlayerItem"/> for a given type.
		/// If you have subtypes of <see cref="ItemContent"/>,
		/// and you get an item group for a basetype of <see cref="ItemContent"/>,
		/// the resultant <see cref="PlayerItemGroup"/> will have all instances from all subclasses of the given type.
		/// </summary>
		/// <param name="itemRef">An <see cref="ItemRef"/> for the type of item to get. </param>
		/// <returns>a <see cref="PlayerItemGroup"/> object for the given itemRef</returns>
		public PlayerItemGroup GetItems(ItemRef itemRef = null)
		{
			itemRef = itemRef ?? "items";

			if (_scopeToItemGroup.TryGetValue(itemRef, out var group)) return group;

			_requestCounter.Insert(itemRef, 1); // pay attention to what specific item groups have been subscribed to.
			var itemGroup = new PlayerItemGroup(itemRef, this);
			_scopeToItemGroup.Add(itemRef, itemGroup);
			_saveHandle.Save();
			return itemGroup;
		}

		/// <summary>
		/// Get a category of <see cref="PlayerCurrency"/> for a given type.
		/// If you have subtypes of <see cref="CurrencyRef"/>,
		/// and you get an item group for a basetype of <see cref="CurrencyRef"/>,
		/// the resultant <see cref="PlayerCurrencyGroup"/> will have all instances from all subclasses of the given type.
		/// </summary>
		/// <param name="currencyRef">An <see cref="CurrencyRef"/> for the type of a currency to get. </param>
		/// <returns>a <see cref="PlayerCurrencyGroup"/> object for the given currencyRef</returns>
		public PlayerCurrencyGroup GetCurrencies(CurrencyRef currencyRef = null)
		{
			currencyRef = currencyRef ?? "currency";
			if (_scopeToCurrencyGroup.TryGetValue(currencyRef, out var group)) return group;
			_requestCounter.Insert(currencyRef, 1);
			var currencyGroup = new PlayerCurrencyGroup(currencyRef, this);
			currencyGroup.Notify();
			var _ = currencyGroup.Refresh();
			_scopeToCurrencyGroup.Add(currencyRef, currencyGroup);
			_saveHandle?.Save();
			return currencyGroup;
		}


		/// <summary>
		/// <inheritdoc cref="Update(Beamable.Common.Api.Inventory.InventoryUpdateBuilder,string)"/>
		/// </summary>
		/// <param name="updateBuilder">An action that gives you a <see cref="InventoryUpdateBuilder"/> to configure with actions to apply to the player's inventory</param>
		/// <param name="transaction">An optional transaction id for the operation. </param>
		/// <returns>A promise representing the success of the operation.</returns>
		public Promise Update(Action<InventoryUpdateBuilder> updateBuilder, string transaction = null)
		{
			var builder = new InventoryUpdateBuilder();
			updateBuilder?.Invoke(builder);

			// serialize the builder, and commit it the log state.
			return Update(builder, transaction);
		}

		/// <summary>
		/// Similar to <see cref="Update(System.Action{Beamable.Common.Api.Inventory.InventoryUpdateBuilder},string)"/>,
		/// However, this method will not actually submit the Inventory update request until the given <see cref="delay"/> passes.
		/// If multiple calls are made to this method, then the <see cref="updateBuilder"/>s will be merged together
		/// and sent as one network call.
		/// This method will minimize network traffic.
		/// This method returns a <see cref="Promise"/> that completes when the network call is actually made. Therefor,
		/// it is not a good idea to call this method several times and await each one, as that would essentially be the same
		/// as calling <see cref="Update(System.Action{Beamable.Common.Api.Inventory.InventoryUpdateBuilder},string)"/>.
		/// Instead, consider using the <see cref="WaitForDelayedUpdate"/> method to get a promise that completes when
		/// the accumulated action completes.
		/// </summary>
		/// <param name="updateBuilder">
		/// An action that delivers an <see cref="InventoryUpdateBuilder"/> instance that can be modified to schedule
		/// inventory updates. When the method is invoked multiple times, the previous instance will be used. 
		/// </param>
		/// <param name="delay">
		/// By default, the inventory update will take a few moments before being sent to Beamable.
		/// Anytime there is a pending request and a new invocation of the method occurs, the scheduled call
		/// will be pushed out by this delay again. 
		/// </param>
		/// <returns>A promise that returns when the inventory update call has actually be sent and confirmed.</returns>
		public Promise UpdateDelayed(Action<InventoryUpdateBuilder> updateBuilder, CustomYieldInstruction delay = null)
		{
			updateBuilder?.Invoke(_delayedBuilder);
			return _delayedUpdatePromise = _debouncer.SetTimeout(CommitDelayed, delay);
		}

		/// <summary>
		/// A <see cref="Promise"/> that completes when calls to <see cref="UpdateDelayed"/> are completed.
		/// </summary>
		public async Promise WaitForDelayedUpdate()
		{
			if (_delayedUpdatePromise != null)
			{
				await _delayedUpdatePromise;
			}
		}

		private Promise CommitDelayed()
		{
			var builder = new InventoryUpdateBuilder(_delayedBuilder);
			_delayedBuilder = new InventoryUpdateBuilder();
			return Update(builder);
		}

		/// <summary>
		/// Make an atomic update to the player's inventory state.
		/// If you are offline, then this function changes based on your <see cref="CoreConfiguration.InventoryOfflineMode"/> setting.
		///
		/// <para>
		/// Configure the <see cref="InventoryUpdateBuilder"/> with the modifications you'd like to make to the player's inventory.
		/// </para>
		/// <para>
		/// Note that by default, you cannot update currency or items, because they can only be set from Beamable Microservices, or
		/// the Beamable Platform itself. However, you can mark individual <see cref="CurrencyContent"/> and <see cref="ItemContent"/>
		/// objects as editable by setting their <see cref="CurrencyContent.clientPermission"/> writeSelf property.
		/// </para>
		/// </summary>
		/// <param name="updateBuilder">A <see cref="InventoryUpdateBuilder"/> containing actions to apply to the player's inventory</param>
		/// <param name="transaction">An optional transaction id for the operation. </param>
		/// <returns>A promise representing the success of the operation.</returns>
		public async Promise Update(InventoryUpdateBuilder updateBuilder, string transaction = null)
		{
			if (_pendingUpdate != null)
			{
				await _pendingUpdate;
			}
			var json = InventoryUpdateBuilderSerializer.ToNetworkJson(updateBuilder, transaction);
			var nextUpdatePromise = _sdkEventService.Add(new SdkEvent(nameof(PlayerInventory), "update", json));
			_pendingUpdate = nextUpdatePromise;
			await nextUpdatePromise;
		}

		private async Promise Apply(InventoryUpdateBuilder builder)
		{
			// TODO apply vip bonus
			if (builder.applyVipBonus == true)
			{
				throw new NotImplementedException("Cannot perform vipBonus in offline mode");
			}

			if (builder.newItems.Count > 0)
			{
				// get the fake item group.
				foreach (var newItem in builder.newItems)
				{

					var content = await _contentService.GetContent<ItemContent>(new ItemRef(newItem.contentId));
					if (!content.clientPermission.writeSelf)
						throw new PlatformRequesterException(new PlatformError
						{
							status = 403,
							service = "inventory",
							error = "not authorized",
							message = "in an offline mode, you tried to write to a protected inventory item. That can't be simulated."
						}, null, "403 offline");


					_nextOfflineItemId--;
					var nextItemId = _nextOfflineItemId;
					_itemIdToReqId[OfflineIdKey(newItem.contentId, nextItemId)] = newItem.requestId;

					localItems.Insert(newItem.contentId, new PlayerItem
					{
						ContentId = newItem.contentId,
						ItemId = nextItemId,
						Properties = newItem.properties,
						CreatedAt = 0,
						UpdatedAt = 0,
						Content = content
					});
					await Refresh(newItem.contentId);
				}
			}

			if (builder.updateItems.Count > 0)
			{
				foreach (var updateItem in builder.updateItems)
				{
					var existingItems = localItems.GetAll(updateItem.contentId);
					var existingItem = existingItems.FirstOrDefault(x => x.ItemId == updateItem.itemId);
					if (existingItem == null)
						throw new InvalidOperationException($"Cannot update non existent item while in offline mode. contentid=[{updateItem.contentId}] itemid=[{updateItem.itemId}]");

					existingItem.Properties = updateItem.properties;
					existingItem.UpdatedAt = 0; // TODO how to get server time in offline way?

					await Refresh(updateItem.contentId);
				}
			}

			if (builder.deleteItems.Count > 0)
			{
				foreach (var deleteItem in builder.updateItems)
				{
					// TODO: why do we need this? If we have the id, we can remove it, and its like it never entered the player's inventorym ,right?
					if (deleteItem.itemId < 0)
						throw new InvalidOperationException(
							"Cannot delete an item that was created while in offline mode. This is because the id of the item isn't actually known, so the update");
				}
				foreach (var deleteItem in builder.deleteItems)
				{
					var existingItems = localItems.GetAll(deleteItem.contentId);
					var existingItem = existingItems.FirstOrDefault(x => x.ItemId == deleteItem.itemId);
					if (existingItem == null)
						throw new InvalidOperationException($"Cannot delete non existent item while in offline mode. contentid=[{deleteItem.contentId}] itemid=[{deleteItem.itemId}]");

					localItems.Remove(deleteItem.contentId, existingItem);
					await Refresh(deleteItem.contentId);
				}
			}


			if (builder.currencyProperties.Count > 0)
			{
				foreach (var currProps in builder.currencyProperties)
				{
					var curr = GetCurrency(currProps.Key);
					foreach (var prop in currProps.Value.Properties)
					{
						curr.Properties[prop.name] = prop.value;
					}
				}

				await Currencies.Refresh();
			}

			foreach (var kvp in builder.currencies)
			{
				Currencies.GetCurrency(kvp.Key).Amount += kvp.Value;
			}

		}


		private string OfflineIdKey(string contentId, long itemId) => $"{contentId}-{itemId}"; // TODO: Maybe don't need content id?

		private bool TryGetOfflineId(string contentId, long itemId, out string reqId)
		{
			return _itemIdToReqId.TryGetValue(OfflineIdKey(contentId, itemId), out reqId);
		}

		private void UpdateOfflineBuilder(InventoryUpdateBuilder builder)
		{
			if (builder.applyVipBonus.HasValue)
			{
				_offlineUpdate.applyVipBonus = builder.applyVipBonus;
			}

			foreach (var kvp in builder.currencyProperties)
			{
				_offlineUpdate.currencyProperties[kvp.Key] = kvp.Value;
			}

			foreach (var curr in builder.currencies)
			{
				_offlineUpdate.CurrencyChange(curr.Key, curr.Value);
			}

			_offlineUpdate.newItems.AddRange(builder.newItems);
			_offlineUpdate.updateItems.AddRange(builder.updateItems);
			_offlineUpdate.deleteItems.AddRange(builder.deleteItems);

			// if we delete an item that we had only ever added offline, then we don't ever send it. So we remove it from the newItems
			foreach (var delete in builder.deleteItems)
			{
				if (TryGetOfflineId(delete.contentId, delete.itemId, out var reqId))
				{
					var newItem = _offlineUpdate.newItems.FirstOrDefault(item => item.requestId == reqId);
					var deleteItem = _offlineUpdate.deleteItems.FirstOrDefault(
						item => item.contentId == delete.contentId && item.itemId == delete.itemId);
					_offlineUpdate.newItems.Remove(newItem);
					_offlineUpdate.deleteItems.Remove(deleteItem);
				}
			}

			// if we update an item that we only ever added offline, then we don't send the update request, but instead modify the original start parameters...
			foreach (var update in builder.updateItems)
			{
				if (TryGetOfflineId(update.contentId, update.itemId, out var reqId))
				{
					var newItem = _offlineUpdate.newItems.FirstOrDefault(item => item.requestId == reqId);
					if (newItem == null)
						throw new InvalidOperationException($"Cannot update item that doesnt exist in builder. {update.contentId}-{update.itemId}");
					newItem.properties = update.properties;
					var updateItem = _offlineUpdate.updateItems.FirstOrDefault(
						item => item.contentId == update.contentId && item.itemId == update.itemId);
					_offlineUpdate.updateItems.Remove(updateItem);
				}
			}
		}

		private async Promise HandleUpdate(SdkEvent evt)
		{
			var json = evt.Args[0];
			var data = InventoryUpdateBuilderSerializer.FromNetworkJson(json);
			var builder = data.Item1;
			var relevantScopes = builder.BuildScopes();
			var transaction = data.Item2;
			try
			{
				Debug.Log("SENDING INVENTORY UPDATE!!!!");
				await _inventoryApi.Update(builder, transaction);
			}
			catch (NoConnectivityException)
			{
				if (_config.InventoryOfflineMode == CoreConfiguration.OfflineStrategy.Disable)
				{
					throw;
				}

				UpdateOfflineBuilder(builder);
				var _ = _consumer.RunAfterReconnection(new SdkEvent(nameof(PlayerInventory), "commit", json));
				await Apply(builder);
				return;
			}

			try
			{
				foreach (var itemGroup in _scopeToItemGroup.Values)
				{
					// only bother updating the group if its in the builder.
					if (relevantScopes.Any(itemGroup.IsScopePartOfGroup))
					{
						await itemGroup.Refresh();
					}
				}
				await Currencies.Refresh();
			}
			catch (NoConnectivityException)
			{
				// oh well.
			}
		}


		private async Promise HandleEvent(SdkEvent evt)
		{
			switch (evt.Event)
			{
				case "update":
					await HandleUpdate(evt);
					break;

				case "commit": // TODO: turn into const strings.
					if (_offlineUpdate.IsEmpty)
					{
						break;
					}
					var nextBuilder = _offlineUpdate;
					_offlineUpdate = new InventoryUpdateBuilder();
					await Update(nextBuilder); // TODO: this might fail, and we'd lose accrued data.
					_itemIdToReqId.Clear();
					break;
			}
			_saveHandle.Save();
		}

		/// <summary>
		/// Refreshes all <see cref="PlayerItemGroup"/>s that have been established using <see cref="GetItems"/>,
		/// and refreshes currencies <see cref="Currencies"/>.
		///
		/// In order to refresh a specific set of scopes, use the <see cref="Refresh(string[])"/> method.
		/// </summary>
		public async Promise Refresh()
		{
			var scopes = _requestCounter.GetNonEmptyKeys(_requestCounter.GetKeys()).ToArray();
			await Refresh(scopes);
			_saveHandle.Save();
		}


		public void OnBeforeSaveState()
		{

		}

	}
}
