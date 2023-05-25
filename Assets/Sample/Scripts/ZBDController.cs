using System;
using System.Threading.Tasks;
using Beamable;
using Beamable.Server;
using Beamable.Server.Clients;
using Newtonsoft.Json;
using PubNubMessaging.Core;
using UnityEngine;
using ZebedeeAPI;
using ZebedeeAPI.Wallet;

public class ZBDController : MonoBehaviour
{
    public static ZBDController Instance => _instance;

    private static ZBDController _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    public static async Task<string> GetWalletBalance()
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZBDMicroservice().GetWallet();

        var jsonObject = JsonConvert.DeserializeObject<GetWalletDetails_Response>(result);

        return jsonObject.data.balance;
    }
    
    public static async Task<string> SendPaymentToGamertag(string gamertag, string sats)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;
    
        var result = await ctx.Microservices().ZBDMicroservice().SendPaymentToGamertag(gamertag, sats);
        
        var jsonObject = JsonConvert.DeserializeObject<SendPaymentToGamertag_Response>(result);
    
        return result;
    }
    
    public static async Task<string> CreateCharge(string expiresIn, string amount, string description)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;
    
        var result = await ctx.Microservices().ZBDMicroservice().CreateCharge(expiresIn, amount, description);
    
        return result;
    }

    public static async Task<string> CreateWithdrawal(string amount, string description)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;
    
        var result = await ctx.Microservices().ZBDMicroservice().CreateWithdrawal(amount, description);
    
        return result;
    }
}
