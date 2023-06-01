using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable;
using Beamable.Server.Clients;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using ZebedeeAPI;

public class UIController : MonoBehaviour
{
    public static UIController Instance => _instance;
    
    [Header("Main Canvas Group")]
    [SerializeField] private CanvasGroup homePanel;
    
    [Header("Wallet")]
    [SerializeField] private TextMeshProUGUI walletBalance;
    [SerializeField] private TextMeshProUGUI walletMessageText;
    
    [Header("Gamertag")]
    [SerializeField] private TMP_InputField gamertagPaymentInputField;
    [SerializeField] private TMP_InputField gamertagPaymentSatsInputField;
    [SerializeField] private TextMeshProUGUI gamertagPaymentMessageText;
    [SerializeField] private TextMeshProUGUI gamertagTransactionIDText;
    
    [SerializeField] private TMP_InputField fetchGamertagUserIDInput;
    [SerializeField] private TextMeshProUGUI fetchGamertagGamertagText;
    [SerializeField] private TextMeshProUGUI fetchGamertagMessageText;
    
    [SerializeField] private TMP_InputField fetchUserIDGamertagInput;
    [SerializeField] private TextMeshProUGUI fetchUserIDUserIDText;
    [SerializeField] private TextMeshProUGUI fetchUserIDMessageText;

    [Header("Charges")] 
    [SerializeField] private TMP_InputField createChargeAmountInput;
    [SerializeField] private TextMeshProUGUI createChargeMessageText;
    [SerializeField] private TextMeshProUGUI createChargeChargeIDText;
    [SerializeField] private TextMeshProUGUI createChargeStatusText;
    
    [SerializeField] private TMP_InputField getChargeDetailsIDInput;
    [SerializeField] private TextMeshProUGUI getChargeDetailsMessageText;
    [SerializeField] private TextMeshProUGUI getChargeDetailsAmountText;
    [SerializeField] private TextMeshProUGUI getChargeDetailsStatusText;
    
    [Header("Withdrawals")]
    [SerializeField] private TMP_InputField createWithdrawalAmountInput;
    [SerializeField] private TextMeshProUGUI createWithdrawalMessageText;
    [SerializeField] private TextMeshProUGUI createWithdrawalWithdrawalIDText;
    [SerializeField] private TextMeshProUGUI createWithdrawalStatusText;
    [SerializeField] private TextMeshProUGUI createWithdrawalInvoiceText;
    
    [SerializeField] private TMP_InputField getWithdrawalDetailsIDInput;
    [SerializeField] private TextMeshProUGUI getWithdrawalDetailsMessageText;
    [SerializeField] private TextMeshProUGUI getWithdrawalDetailsStatusText;
    [SerializeField] private TextMeshProUGUI getWithdrawalDetailsInvoiceText;
    
    [Header("Utilities")]
    [SerializeField] private TextMeshProUGUI btcPriceText;
    [SerializeField] private TextMeshProUGUI btcPriceMessageText;

    [SerializeField] private TextMeshProUGUI productionIPText;

    [SerializeField] private TMP_InputField supportedRegionIPInput;
    [SerializeField] private TextMeshProUGUI supportedRegionText;
    
    
    

    

    private static UIController _instance;
    private CanvasGroup[] _canvasGroups;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    
    private void Start()
    {
        _canvasGroups = GetComponentsInChildren<CanvasGroup>();
        ShowPanel(homePanel);
    }
    
    #region Panels

    public void ShowPanel(CanvasGroup canvasGroup)
    {
        HideAllPanels();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }


    private void HideAllPanels()
    {
        foreach (var canvasGroup in _canvasGroups)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    
    #endregion
    
    #region Wallet

    public async void GetWalletBalance()
    {
        walletBalance.text = "Sending Request...";
        
        var wallet = await MyController.GetWalletBalance();
        

        walletBalance.text = wallet.data.balance + " " + wallet.data.unit;
        walletMessageText.text = wallet.message;

    }
    
    
    #endregion
    
    #region Gamertags
    
    public async void SendPaymentToGamertag()
    {
        gamertagTransactionIDText.text = "Sending Request...";
        
        var gamertag = gamertagPaymentInputField.text;
        var amount = gamertagPaymentSatsInputField.text;

        var result = await MyController.SendPaymentToGamertag(gamertag, amount, "Test Transaction");

        gamertagPaymentMessageText.text = result.message;
        gamertagTransactionIDText.text = result.data.transactionId;
    }
    
    public async void GetGamertagByUserID()
    {
        
        fetchGamertagGamertagText.text = "Sending Request...";
        
        var userID = fetchGamertagUserIDInput.text;
        
        var result = await MyController.GetGamertagByUserID(userID);
        
        fetchGamertagGamertagText.text = result.data.gamertag;
        fetchGamertagMessageText.text = result.message;
    }
    
    public async void GetUserIDByGamertag()
    {
        fetchUserIDUserIDText.text = "Sending Request...";
        
        var gamertag = fetchUserIDGamertagInput.text;
        
        var result = await MyController.GetUserIDByGamertag(gamertag);
        
        fetchUserIDUserIDText.text = result.data.id;
        fetchUserIDMessageText.text = result.message;
    }
    
    #endregion
    
    #region Charges
    
    public async void CreateCharge()
    {
        gamertagTransactionIDText.text = "Sending Request...";
        
        var amount = createChargeAmountInput.text;

        var result = await MyController.CreateCharge("300", amount, "Test Charge ");

        createChargeMessageText.text = result.message;
        createChargeChargeIDText.text = result.data.id;
        createChargeStatusText.text = result.data.status;
    }

    public async void GetChargeDetails()
    {
        getChargeDetailsAmountText.text = "Sending Request...";
        
        var chargeID = getChargeDetailsIDInput.text;
        
        var result = await MyController.GetChargeDetails(chargeID);
        
        getChargeDetailsMessageText.text = result.message;
        getChargeDetailsAmountText.text = result.data.amount;
        getChargeDetailsStatusText.text = result.data.status;
    }
    
    #endregion
    
    #region Withdrawals

    public async void CreateWithdrawalRequest()
    {
        createWithdrawalWithdrawalIDText.text = "Sending Request...";
        
        var amount = createWithdrawalAmountInput.text;

        var result = await MyController.CreateWithdrawal("300", amount, "Test Withdrawal");
        
        createWithdrawalMessageText.text = result.message;
        createWithdrawalWithdrawalIDText.text = result.data.id;
        createWithdrawalStatusText.text = result.data.status;
        createWithdrawalInvoiceText.text = result.data.invoice.request;

    }

    public async void GetWithdrawalDetails()
    {
        getWithdrawalDetailsMessageText.text = "Sending Request...";
        
        var withdrawalID = getWithdrawalDetailsIDInput.text;
        
        var result = await MyController.GetWithdrawalRequestDetails(withdrawalID);
        
        getWithdrawalDetailsMessageText.text = result.message;
        getWithdrawalDetailsStatusText.text = result.data.status;
        getWithdrawalDetailsInvoiceText.text = result.data.invoice.request;
        
    }
    
    #endregion
    
    #region Utilities

    public async void GetBTCPrice()
    {
        btcPriceText.text = "Sending Request...";
        
        var result = await MyController.GetBTCUSDExchangeRate();
        
        btcPriceText.text = result.data.btcUsdPrice;
        btcPriceMessageText.text = result.message;
        
    }

    public async void GetProductionIPs()
    {
        productionIPText.text = "Sending Request...";
        
        var result = await MyController.GetProductionIPs();
        
        productionIPText.text = result.data.ips[0];
        
    }
    
    public async void GetSupportedRegions()
    {
        supportedRegionText.text = "Sending Request...";
        
        var ip = supportedRegionIPInput.text;
        
        var result = await MyController.IsSupportedRegion(ip);

        supportedRegionText.text = result.data.isSupported.ToString();

    }
    
    #endregion
}
