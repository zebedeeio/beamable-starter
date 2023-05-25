using System.Threading.Tasks;
using Beamable.Server;
using UnityEngine;
using ZebedeeAPI;
using ZebedeeAPI.Wallet;

namespace Beamable.Microservices
{
	[Microservice("ZBDMicroservice")]
	public class ZBDMicroservice : Microservice
	{
		[ClientCallable]
		public async Task<string> GetWallet()
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.GetWalletDetails();
			
			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> SendPaymentToGamertag(string gamertag, string sats)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.SendPaymentToGamertag(gamertag, sats, "Beamable Sample");
			
			return jsonResponse;
		}

		[ClientCallable]
		public async Task<string> CreateCharge(string expiresIn, string amount, string description)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.CreateCharge(expiresIn, amount, description);

			return jsonResponse;
		}
		
		[ClientCallable]
		public async Task<string> CreateWithdrawal(string amount, string description)
		{
			var apiKey = await GetAPIKey();
			
			var api = new ZebedeeAPI.ZebedeeAPI(apiKey);
			var jsonResponse = await api.CreateWithdrawalRequest("300", "1000");
		
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
		
		private async Task<string> GetAPIKey()
		{
			var config = await Services.RealmConfig.GetRealmConfigSettings();
			var key = config.GetSetting("ZebedeeAPI", "apikey");

			return key;

		}
	}
	
	
}
