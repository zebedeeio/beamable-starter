using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Server.Clients;
using Newtonsoft.Json;
using UnityEngine;
using ZebedeeAPI;

public static class ZBDController
{
    #region Wallet
    
    public static async Task<GetWalletDetails_Response> GetWalletBalance()
    {
        Debug.Log("GetWalletBalance()");
        var ctx = BeamContext.Default;
        await ctx.OnReady;
    
        var result = await ctx.Microservices().ZBDMicroservice().GetWallet();
        var jsonObject = JsonConvert.DeserializeObject<GetWalletDetails_Response>(result);
    
        return jsonObject;
    }
    
    #endregion
    
    #region Gamertag
    public static async Task<SendPaymentToGamertag_Response> SendPaymentToGamertag(string gamertag, string amount, string description)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;
    
        var result = await ctx.Microservices().ZBDMicroservice().SendPaymentToGamertag(gamertag, amount, description);
        
        Debug.Log(result);
        
        var jsonObject = JsonConvert.DeserializeObject<SendPaymentToGamertag_Response>(result);
    
        return jsonObject;
    }

    public static async Task<FetchGamertagByUserID_Response> GetGamertagByUserID(string userID)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().FetchGamertagByUserID(userID);
        
        var jsonObject = JsonConvert.DeserializeObject<FetchGamertagByUserID_Response>(result);

        return jsonObject;
    }
    
    public static async Task<FetchUserIDByGamertag_Response> GetUserIDByGamertag(string gamertag)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().FetchUserIDByGamertag(gamertag);
        
        var jsonObject = JsonConvert.DeserializeObject<FetchUserIDByGamertag_Response>(result);

        return jsonObject;
    }
    
    #endregion
    
    #region Charges
    
    public static  async Task<CreateCharge_Response> CreateCharge(string expiresIn, string amount, string description)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;
    
        var result = await ctx.Microservices().ZBDMicroservice().CreateCharge(expiresIn, amount, description);
        
        var jsonObject = JsonConvert.DeserializeObject<CreateCharge_Response>(result);
    
        return jsonObject;
    }
    
    public static async Task<GetChargeDetails_Response> GetChargeDetails(string chargeID)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().GetChargeDetails(chargeID);
        
        var jsonObject = JsonConvert.DeserializeObject<GetChargeDetails_Response>(result);
    
        return jsonObject;
    }
    
    #endregion
    
    #region Withdrawals

    public static  async Task<CreateWithdrawalRequest_Response> CreateWithdrawal(string expiresIn, string amount, string description)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().CreateWithdrawalRequest(expiresIn, amount, description);
        
        var jsonObject = JsonConvert.DeserializeObject<CreateWithdrawalRequest_Response>(result);
    
        return jsonObject;
    }
    
    public static async Task<GetWithdrawalRequestDetails_Response> GetWithdrawalRequestDetails(string withdrawalID)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().GetWithdrawalRequestDetails(withdrawalID);
        
        var jsonObject = JsonConvert.DeserializeObject<GetWithdrawalRequestDetails_Response>(result);
    
        return jsonObject;
    }
    
    #endregion
    
    #region Utilities
    
    public static async Task<BTCUSDExchangeRate_Response> GetBTCUSDExchangeRate()
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().GetBTCUSDExchangeRate();
        
        var jsonObject = JsonConvert.DeserializeObject<BTCUSDExchangeRate_Response>(result);
    
        return jsonObject;
    }
    
    public static async Task<APIProductionIPs_Response> GetProductionIPs()
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().GetProductionIPs();
        
        var jsonObject = JsonConvert.DeserializeObject<APIProductionIPs_Response>(result);
    
        return jsonObject;
    }
    
    public static async Task<IsSupportedRegion_Response> IsSupportedRegion(string ip)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().SupportedRegion(ip);
        
        var jsonObject = JsonConvert.DeserializeObject<IsSupportedRegion_Response>(result);
    
        return jsonObject;
    }
    
    #endregion
}
