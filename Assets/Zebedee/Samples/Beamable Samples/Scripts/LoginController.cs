using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Beamable;
using Beamable.Server.Clients;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using ZebedeeAPI;


public class LoginController : MonoBehaviour
{
    
    public static LoginController Instance { get; private set; }
    
    [Header("Zebedee API Config")]
    [SerializeField] private string apiLoginBaseEndpoint = "https://api.zebedee.io/";
    [SerializeField] private string clientID = "YOUR_CLIENT_ID";
    [SerializeField] private string redirectURL = "";
    [SerializeField] private string responseType = "code";

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI GamertagText;
    [SerializeField] private TextMeshProUGUI lightningAddressText;
    [SerializeField] private TextMeshProUGUI emailText;
    [SerializeField] private TextMeshProUGUI isVerifiedText;
    [SerializeField] private TextMeshProUGUI refreshText;
    [SerializeField] private TextMeshProUGUI responseText;
    [SerializeField] private GameObject refreshButton;

    [HideInInspector]
    public string accessToken;
    [HideInInspector]
    public string refreshToken;
    
    private string deeplinkURL;
    

    #region Unity Methods
    private void Awake()
    {
        if (clientID == "YOUR_CLIENT_ID" || clientID == "")
        {
            responseText.text = "You must enter your Client ID in the Login Controller.";
        }
        
        if (Instance == null)
        {
            Instance = this; 
            
            Application.deepLinkActivated += onDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                onDeepLinkActivated(Application.absoluteURL);
            }
            else
            {
                deeplinkURL = "";
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #endregion

    #region Deep Link Event
    
    private void onDeepLinkActivated(string absoluteURL)
    {
        deeplinkURL = absoluteURL;
        responseText.text = "Making Call...";
        ContinueLogin(deeplinkURL);
    }
    
    #endregion

    #region Login Functions
    
    public void Login()
    {
        var pkce = ZebedeeUtils.GeneratePKCE();
        
        string state = Guid.NewGuid().ToString();

        PlayerPrefs.SetString("verifier", pkce["verifier"]);
        PlayerPrefs.SetString("state", state);

        var authURL = ZebedeeUtils.BuildAuthorizationURL(apiLoginBaseEndpoint, clientID, responseType, redirectURL, pkce["challenge"]);

        Application.OpenURL(authURL);

    }

    public void Refresh()
    {
        responseText.text = "Refreshing...";
        RefreshUserData();
    }
    
    private async void ContinueLogin(string url)
    {
        if (!url.Contains(redirectURL))
        {
            return;
        }
        
        var queryString = new Uri(url).Query;
        var parameters = HttpUtility.ParseQueryString(queryString);
        var code = parameters.Get("code");
        var verifier = PlayerPrefs.GetString("verifier");
        
        
        var tokens = await GetAccessToken(code, verifier);
        accessToken = tokens["access_token"];
        refreshToken = tokens["refresh_token"];
        
        var userData = await GetUserData(accessToken);
        responseText.text = userData;
        
        var jsonObject = JsonConvert.DeserializeObject<FetchUserData_Response>(userData);
        GamertagText.text = jsonObject.data.gamertag;
        lightningAddressText.text = jsonObject.data.lightningAddress;
        emailText.text = jsonObject.data.email;
        isVerifiedText.text = jsonObject.data.isVerified.ToString();
        
        refreshButton.SetActive(true);

    }

    private async void RefreshUserData()
    {
        var refreshedAccessToken = await RefreshAccessToken(refreshToken);
        refreshText.text = refreshedAccessToken["access_token"];
        responseText.text = "Refreshed Access Token: ";
    }
    
    #endregion

    #region Beamable Microservice Calls
    
    private async Task<string> GetUserData(string token)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZebedeeMicroservice().GetUserData(token);

        return result;
    }

    private async Task<Dictionary<string, string>> GetAccessToken(string code, string verifier)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZebedeeMicroservice().GetAccessToken(clientID, code, verifier, redirectURL);
        var jsonObject = JsonConvert.DeserializeObject<FetchAccessToken_Response>(result);

        return new Dictionary<string, string>()
        {
            { "access_token", jsonObject.access_token },
            { "refresh_token", jsonObject.refresh_token }
        };
    }
    
    private async Task<Dictionary<string, string>> RefreshAccessToken(string refreshToken)
    {
        var ctx = BeamContext.Default;
        await ctx.OnReady;

        var result = await ctx.Microservices().ZebedeeMicroservice().RefreshAccessToken(clientID, refreshToken, redirectURL);
        var jsonObject = JsonConvert.DeserializeObject<FetchAccessToken_Response>(result);

        return new Dictionary<string, string>()
        {
            { "access_token", jsonObject.access_token },
            { "refresh_token", jsonObject.refresh_token }
        };
    }
    
    #endregion
    
}
