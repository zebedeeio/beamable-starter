using System.Threading.Tasks;
using Beamable.Server;


namespace Beamable.Microservices
{
	
	class AccessTokenRequest
	{
		public string code;
		public string client_secret;
		public string client_id;
		public string code_verifier;
		public string grant_type;
		public string redirect_uri;
		public string refresh_token;
	}
	public class ActionResponse
	{
		public bool error;
		public string response;
		public string type;
		public string data;
		public long responseCode;
	}
	
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
		public async Task<string> CreateWithdrawalRequest(string expiresIn, string amount, string description)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.CreateWithdrawalRequest(expiresIn, amount, description);
		
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
		
		#region Utilities

		[ClientCallable]
		public async Task<string> GetBTCUSDExchangeRate()
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.BTCUSDExchangeRate();

			return jsonResponse;
		}

		[ClientCallable]
		public async Task<string> GetProductionIPs()
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.APIProductionIPs();

			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> SupportedRegion(string ip)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.IsSupportedRegion(ip);

			return jsonResponse;
		}
		
		#endregion
		
		#region Login
		
		
		[ClientCallable]
		public async Task<string> GetAccessToken(string clientID, string code, string codeVerifier, string redirectURL)
		{
			
			var apiKey = await GetAPIKey();
			var clientSecret = await GetClientSecret();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.FetchAccessToken(clientID, clientSecret, code, codeVerifier, redirectURL);


			return jsonResponse;

		}

		[ClientCallable]
		public async Task<string> GetUserData(string userToken)
		{
			var apiKey = await GetAPIKey();
			var clientSecret = GetClientSecret().Result;
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.FetchUserData(userToken);

			return jsonResponse;
		}

		[ClientCallable]
		public async Task<string> TestService()
		{
			var clientSecret = await GetClientSecret();

			return clientSecret;
		}
		
		#endregion
		
		
		private async Task<string> GetAPIKey()
		{
			var config = await Services.RealmConfig.GetRealmConfigSettings();
			var key = config.GetSetting("ZebedeeAPI", "apikey");

			return key;

		}

		private async Task<string> GetClientSecret()
		{
			var config = await Services.RealmConfig.GetRealmConfigSettings();
			var secret = config.GetSetting("ZebedeeAPI", "clientsecret");

			return secret;
		}
	}
	
	
}
