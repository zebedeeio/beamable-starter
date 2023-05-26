using System.Threading.Tasks;
using Beamable.Server;
using UnityEngine;
using ZebedeeAPI;


namespace Beamable.Microservices
{
	[Microservice("ZBDMicroservice")]
	public class ZBDMicroservice : Microservice
	{
		
		#region Wallet
		
		[ClientCallable]
		public async Task<string> GetWallet()
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.GetWalletDetails();

			
			return jsonResponse;
		}
		
		#endregion
		
		#region Gamertag
		
		[ClientCallable]
		public async Task<string> SendPaymentToGamertag(string gamertag, string amount, string description)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.SendPaymentToGamertag(gamertag, amount, description);
			
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> FetchGamertagByUserID(string userID)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.FetchGamertagByUserId(userID);
			
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> FetchUserIDByGamertag(string gamertag)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.FetchUserIdByGamertag(gamertag);
			
			return jsonResponse;
		}
		
		#endregion
		
		#region Charges

		[ClientCallable]
		public async Task<string> CreateCharge(string expiresIn, string amount, string description)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.CreateCharge(expiresIn, amount, description);

			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> GetChargeDetails(string chargeId)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.GetChargeDetails(chargeId);
		
			return jsonResponse;
		}
		
		#endregion
		
		#region Withdrawals
		
		[ClientCallable]
		public async Task<string> CreateWithdrawalRequest(string expiresIn, string amount)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.CreateWithdrawalRequest(expiresIn, amount);
		
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> GetWithdrawalRequestDetails(string withdrawalId)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.GetWithdrawalRequestDetails(withdrawalId);
		
			return jsonResponse;
		}
		
		#endregion
		
		
		private async Task<string> GetAPIKey()
		{
			var config = await Services.RealmConfig.GetRealmConfigSettings();
			var key = config.GetSetting("ZebedeeAPI", "apikey");

			return key;

		}
	}
	
	
}
