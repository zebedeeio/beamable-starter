//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Beamable.Server.Clients
{
    using System;
    using Beamable.Platform.SDK;
    using Beamable.Server;
    
    
    /// <summary> A generated client for <see cref="Beamable.Microservices.ZBDMicroservice"/> </summary
    public sealed class ZBDMicroserviceClient : MicroserviceClient, Beamable.Common.IHaveServiceName
    {
        
        public ZBDMicroserviceClient(BeamContext context = null) : 
                base(context)
        {
        }
        
        public string ServiceName
        {
            get
            {
                return "ZBDMicroservice";
            }
        }
        
        /// <summary>
        /// Call the GetWallet method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.GetWallet"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetWallet()
        {
            string[] serializedFields = new string[0];
            return this.Request<string>("ZBDMicroservice", "GetWallet", serializedFields);
        }
        
        /// <summary>
        /// Call the SendPaymentToGamertag method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.SendPaymentToGamertag"/>
        /// </summary>
        public Beamable.Common.Promise<string> SendPaymentToGamertag(string gamertag, string amount, string description)
        {
            string serialized_gamertag = this.SerializeArgument<string>(gamertag);
            string serialized_amount = this.SerializeArgument<string>(amount);
            string serialized_description = this.SerializeArgument<string>(description);
            string[] serializedFields = new string[] {
                    serialized_gamertag,
                    serialized_amount,
                    serialized_description};
            return this.Request<string>("ZBDMicroservice", "SendPaymentToGamertag", serializedFields);
        }
        
        /// <summary>
        /// Call the FetchGamertagByUserID method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.FetchGamertagByUserID"/>
        /// </summary>
        public Beamable.Common.Promise<string> FetchGamertagByUserID(string userID)
        {
            string serialized_userID = this.SerializeArgument<string>(userID);
            string[] serializedFields = new string[] {
                    serialized_userID};
            return this.Request<string>("ZBDMicroservice", "FetchGamertagByUserID", serializedFields);
        }
        
        /// <summary>
        /// Call the FetchUserIDByGamertag method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.FetchUserIDByGamertag"/>
        /// </summary>
        public Beamable.Common.Promise<string> FetchUserIDByGamertag(string gamertag)
        {
            string serialized_gamertag = this.SerializeArgument<string>(gamertag);
            string[] serializedFields = new string[] {
                    serialized_gamertag};
            return this.Request<string>("ZBDMicroservice", "FetchUserIDByGamertag", serializedFields);
        }
        
        /// <summary>
        /// Call the CreateCharge method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.CreateCharge"/>
        /// </summary>
        public Beamable.Common.Promise<string> CreateCharge(string expiresIn, string amount, string description)
        {
            string serialized_expiresIn = this.SerializeArgument<string>(expiresIn);
            string serialized_amount = this.SerializeArgument<string>(amount);
            string serialized_description = this.SerializeArgument<string>(description);
            string[] serializedFields = new string[] {
                    serialized_expiresIn,
                    serialized_amount,
                    serialized_description};
            return this.Request<string>("ZBDMicroservice", "CreateCharge", serializedFields);
        }
        
        /// <summary>
        /// Call the GetChargeDetails method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.GetChargeDetails"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetChargeDetails(string chargeId)
        {
            string serialized_chargeId = this.SerializeArgument<string>(chargeId);
            string[] serializedFields = new string[] {
                    serialized_chargeId};
            return this.Request<string>("ZBDMicroservice", "GetChargeDetails", serializedFields);
        }
        
        /// <summary>
        /// Call the CreateWithdrawalRequest method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.CreateWithdrawalRequest"/>
        /// </summary>
        public Beamable.Common.Promise<string> CreateWithdrawalRequest(string expiresIn, string amount, string description)
        {
            string serialized_expiresIn = this.SerializeArgument<string>(expiresIn);
            string serialized_amount = this.SerializeArgument<string>(amount);
            string serialized_description = this.SerializeArgument<string>(description);
            string[] serializedFields = new string[] {
                    serialized_expiresIn,
                    serialized_amount,
                    serialized_description};
            return this.Request<string>("ZBDMicroservice", "CreateWithdrawalRequest", serializedFields);
        }
        
        /// <summary>
        /// Call the GetWithdrawalRequestDetails method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.GetWithdrawalRequestDetails"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetWithdrawalRequestDetails(string withdrawalId)
        {
            string serialized_withdrawalId = this.SerializeArgument<string>(withdrawalId);
            string[] serializedFields = new string[] {
                    serialized_withdrawalId};
            return this.Request<string>("ZBDMicroservice", "GetWithdrawalRequestDetails", serializedFields);
        }
        
        /// <summary>
        /// Call the GetBTCUSDExchangeRate method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.GetBTCUSDExchangeRate"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetBTCUSDExchangeRate()
        {
            string[] serializedFields = new string[0];
            return this.Request<string>("ZBDMicroservice", "GetBTCUSDExchangeRate", serializedFields);
        }
        
        /// <summary>
        /// Call the GetProductionIPs method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.GetProductionIPs"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetProductionIPs()
        {
            string[] serializedFields = new string[0];
            return this.Request<string>("ZBDMicroservice", "GetProductionIPs", serializedFields);
        }
        
        /// <summary>
        /// Call the SupportedRegion method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.SupportedRegion"/>
        /// </summary>
        public Beamable.Common.Promise<string> SupportedRegion(string ip)
        {
            string serialized_ip = this.SerializeArgument<string>(ip);
            string[] serializedFields = new string[] {
                    serialized_ip};
            return this.Request<string>("ZBDMicroservice", "SupportedRegion", serializedFields);
        }
        
        /// <summary>
        /// Call the GetAccessToken method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.GetAccessToken"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetAccessToken(string clientID, string code, string codeVerifier, string redirectURL)
        {
            string serialized_clientID = this.SerializeArgument<string>(clientID);
            string serialized_code = this.SerializeArgument<string>(code);
            string serialized_codeVerifier = this.SerializeArgument<string>(codeVerifier);
            string serialized_redirectURL = this.SerializeArgument<string>(redirectURL);
            string[] serializedFields = new string[] {
                    serialized_clientID,
                    serialized_code,
                    serialized_codeVerifier,
                    serialized_redirectURL};
            return this.Request<string>("ZBDMicroservice", "GetAccessToken", serializedFields);
        }
        
        /// <summary>
        /// Call the RefreshAccessToken method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.RefreshAccessToken"/>
        /// </summary>
        public Beamable.Common.Promise<string> RefreshAccessToken(string clientID, string refreshToken, string redirectURL)
        {
            string serialized_clientID = this.SerializeArgument<string>(clientID);
            string serialized_refreshToken = this.SerializeArgument<string>(refreshToken);
            string serialized_redirectURL = this.SerializeArgument<string>(redirectURL);
            string[] serializedFields = new string[] {
                    serialized_clientID,
                    serialized_refreshToken,
                    serialized_redirectURL};
            return this.Request<string>("ZBDMicroservice", "RefreshAccessToken", serializedFields);
        }
        
        /// <summary>
        /// Call the GetUserData method on the ZBDMicroservice microservice
        /// <see cref="Beamable.Microservices.ZBDMicroservice.GetUserData"/>
        /// </summary>
        public Beamable.Common.Promise<string> GetUserData(string userToken)
        {
            string serialized_userToken = this.SerializeArgument<string>(userToken);
            string[] serializedFields = new string[] {
                    serialized_userToken};
            return this.Request<string>("ZBDMicroservice", "GetUserData", serializedFields);
        }
    }
    
    internal sealed class MicroserviceParametersZBDMicroserviceClient
    {
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_String : MicroserviceClientDataWrapper<string>
        {
        }
    }
    
    [BeamContextSystemAttribute()]
    public static class ExtensionsForZBDMicroserviceClient
    {
        
        [Beamable.Common.Dependencies.RegisterBeamableDependenciesAttribute()]
        public static void RegisterService(Beamable.Common.Dependencies.IDependencyBuilder builder)
        {
            builder.AddScoped<ZBDMicroserviceClient>();
        }
        
        public static ZBDMicroserviceClient ZBDMicroservice(this Beamable.Server.MicroserviceClients clients)
        {
            return clients.GetClient<ZBDMicroserviceClient>();
        }
    }
}
