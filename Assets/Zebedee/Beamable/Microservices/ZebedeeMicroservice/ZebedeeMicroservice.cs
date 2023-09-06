using System.Threading.Tasks;
using Beamable.Server;

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
		
		#region Static Charges
		
		[ClientCallable]
		public async Task<string> CreateStaticCharge(
			string allowedSlots, 
			string minAmount, 
			string maxAmount, 
			string description, 
			string internalID, 
			string callbackURL, 
			string successMessage)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.CreateStaticCharge(allowedSlots, minAmount, maxAmount, description, internalID, callbackURL, successMessage);
		
			return jsonResponse;
		}
		
		public async Task<string> UpdateStaticCharge(
			string staticChargeID,
			string allowedSlots, 
			string minAmount, 
			string maxAmount, 
			string description, 
			string internalID, 
			string callbackURL, 
			string successMessage)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.UpdateStaticCharge(staticChargeID, allowedSlots, minAmount, maxAmount, description, internalID, callbackURL, successMessage);
		
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> GetStaticChargeDetails(string staticChargeID)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.GetStaticChargeDetails(staticChargeID);
		
			return jsonResponse;
		}

		#endregion
		
		#region Payments

		[ClientCallable]
		public async Task<string> PayInvoice(string description, string internalID, string invoice, string callbackURL,
			string amount)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.PayInvoice(description, internalID, invoice, callbackURL, amount);
			
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> GetPaymentDetails(string paymentId)
		{
			var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
			var jsonResponse = await api.GetPaymentDetails(paymentId);
			
			return jsonResponse;
		}
		
		#endregion
		
		#region Lightning
		
		[ClientCallable]
        public async Task<string> SendPaymentToLightningAddress(string lnAddress, string amount, string comment)
        {
        	var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
        	var jsonResponse = await api.SendPaymentToLightningAddress(lnAddress, amount, comment);
        
        	return jsonResponse;
        }

        [ClientCallable]
        public async Task<string> FetchChargeFromLightningAddress(string lnAddress, string amount, string description)
        {
	        var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
	        var jsonResponse = await api.FetchChargeFromLightningAddress(lnAddress, amount, description);
           
            return jsonResponse; 
        }
        
        [ClientCallable]
        public async Task<string> ValidateLightningAddress(string lnAddress)
		{
	        var api = new ZebedeeAPI.ZebedeeAPI(GetAPIKey());
	        var jsonResponse = await api.ValidateLightningAddress(lnAddress);
		   
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
		
		#region Internal Transfer
		
		[ClientCallable]
		public async Task<string> InitiateInternalTransfer(string amount, string receiverWallet)
		{
			var apiKey = GetAPIKey();

			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.InitiateInternalTransfer(amount, receiverWallet);

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
			var config = Provider.GetService<Config>();
			var secret = config.ClientSecret;

			return secret;
		}
		
		#endregion
	}
}
