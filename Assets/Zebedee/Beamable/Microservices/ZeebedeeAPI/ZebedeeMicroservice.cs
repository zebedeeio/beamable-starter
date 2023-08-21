using System.Threading.Tasks;
using Beamable.Server;
using ZebedeeAPI;

namespace Beamable.Microservices
{
	[Microservice("ZebedeeMicroservice",
		CustomAutoGeneratedClientPath =
			"Assets/Zebedee/Beamable/AutoGenerated/Microservices")]
	public class ZebedeeMicroservice : Microservice
	{
		
		#region Config
		
		[ConfigureServices()]
		public static void Configure(IServiceBuilder builder)
		{
			builder.Builder.AddSingleton<Config>();
		}

		[InitializeServices()]
		public static async Task Init(IServiceInitializer init)
		{
			var config = init.GetService<Config>();
			await config.Init();
		}
		
		#endregion
		
		#region Wallet
		
		[ClientCallable]
		public async Task<string> GetWallet()
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.GetWalletDetails();
			
			return jsonResponse;
		}
		
		#endregion
		
		#region Gamertag
		
		[ClientCallable]
		public async Task<string> SendPaymentToGamertag(string gamertag, string amount, string description)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.SendPaymentToGamertag(gamertag, amount, description);
			
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> FetchGamertagByUserID(string userID)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.FetchGamertagByUserId(userID);
			
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> FetchUserIDByGamertag(string gamertag)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.FetchUserIdByGamertag(gamertag);
			
			return jsonResponse;
		}
		
		#endregion
		
		#region Charges

		[ClientCallable]
		public async Task<string> CreateCharge(string expiresIn, string amount, string description)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.CreateCharge(expiresIn, amount, description);

			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> GetChargeDetails(string chargeId)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.GetChargeDetails(chargeId);
		
			return jsonResponse;
		}
		
		#endregion
		
		#region Withdrawals
		
		[ClientCallable]
		public async Task<string> CreateWithdrawalRequest(string expiresIn, string amount, string description)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.CreateWithdrawalRequest(expiresIn, amount, description);
		
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> GetWithdrawalRequestDetails(string withdrawalId)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.GetWithdrawalRequestDetails(withdrawalId);
		
			return jsonResponse;
		}
		
		#endregion
		
		#region Utilities

		[ClientCallable]
		public async Task<string> GetBTCUSDExchangeRate()
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.BTCUSDExchangeRate();

			return jsonResponse;
		}

		[ClientCallable]
		public async Task<string> GetProductionIPs()
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.APIProductionIPs();

			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> SupportedRegion(string ip)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.IsSupportedRegion(ip);

			return jsonResponse;
		}
		
		#endregion
		
		#region Login
		
		
		[ClientCallable]
		public async Task<string> GetAccessToken(string clientID, string code, string codeVerifier, string redirectURL)
		{
			
			var apiKey = GetAPIKey();
			var clientSecret = await GetClientSecret();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.FetchAccessToken(clientID, clientSecret, code, codeVerifier, redirectURL);


			return jsonResponse;

		}

		[ClientCallable]
		public async Task<string> RefreshAccessToken(string clientID, string refreshToken, string redirectURL)
		{
			var apiKey = GetAPIKey();
			var clientSecret = await GetClientSecret();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.RefreshAccessToken(clientID, clientSecret, refreshToken, redirectURL);
			
			return jsonResponse;
		}

		[ClientCallable]
		public async Task<string> GetUserData(string userToken)
		{
			var apiKey = GetAPIKey();

			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.FetchUserData(userToken);

			return jsonResponse;
		}

		#endregion
		
		
		#region Helper Functions
		
		private string GetAPIKey()
		{
			var config = Provider.GetService<Config>();
			var apiKey = config.ApiKey;

			return apiKey;

		}

		private async Task<string> GetClientSecret()
		{
			//var config = await Constants.Features.Services.RealmConfig.GetRealmConfigSettings();
			//var secret = config.GetSetting("ZebedeeAPI", "clientsecret");

			return "";
		}
		
		#endregion
	}
}
