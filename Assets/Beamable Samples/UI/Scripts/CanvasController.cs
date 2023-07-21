using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    #region Serialized Fields
    
    [SerializeField] private CanvasGroup mainPanel;
    [Header("Gamertag")]
    [SerializeField] private TMP_InputField gamertagPaymentInputField;
    [SerializeField] private TMP_InputField gamertagPaymentSatsInputField;
    [SerializeField] private TextMeshProUGUI gamertagTransactionIDText;
    [SerializeField] private TMP_InputField fetchGamertagUserIDInput;
    [SerializeField] private TextMeshProUGUI fetchGamertagGamertagText;
    [SerializeField] private TMP_InputField fetchUserIDGamertagInput;
    [SerializeField] private TextMeshProUGUI fetchUserIDUserIDText;

    [Header("Wallet")]
    [Header("Content")]
    [SerializeField] private TextMeshProUGUI walletBalance;

    [Header("Charges")]
    [SerializeField] private TMP_InputField createChargeAmountInput;
    [SerializeField] private TextMeshProUGUI createChargeChargeIDText;
    [SerializeField] private TMP_InputField getChargeDetailsIDInput;
    [SerializeField] private TextMeshProUGUI getChargeDetailsMessageText;
    [SerializeField] private TextMeshProUGUI getChargeDetailsAmountText;

    [Header("Withdrawals")]
    [SerializeField] private TMP_InputField createWithdrawalAmountInput;
    [SerializeField] private TextMeshProUGUI createWithdrawalWithdrawalIDText;
    [SerializeField] private TMP_InputField getWithdrawalDetailsIDInput;
    [SerializeField] private TextMeshProUGUI getWithdrawalDetailsAmountText;
    [SerializeField] private TextMeshProUGUI getWithdrawalDetailsStatusText;

    
    [Header("Utilities")]
    [SerializeField] private TextMeshProUGUI btcPriceText;
    [SerializeField] private TextMeshProUGUI productionIPText;
    [SerializeField] private TMP_InputField supportedRegionIPInput;
    [SerializeField] private TextMeshProUGUI supportedRegionText;

    [Header("Settings")] 
    [SerializeField] private float fadeDuration = 0.5f;
    
    #endregion
    
    private CanvasGroup _currentCanvasGroup;
    private CanvasGroup[] _canvasGroups;

    #region Unity Methods
    private void Awake()
    {
        _currentCanvasGroup = mainPanel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _canvasGroups = GetComponentsInChildren<CanvasGroup>();
        HideAllPanels();
        ShowPanel(mainPanel);
    }
    
    #endregion
    
    #region Panels

    public void SwitchPanel(CanvasGroup canvasToSwitchTo)
    {
        StartCoroutine(HandleSwitch(canvasToSwitchTo));
    }

    private IEnumerator HandleSwitch(CanvasGroup canvasToSwitchTo)
    {
        yield return Fade(_currentCanvasGroup, 0f);
        HideAllPanels();
        EnablePanel(canvasToSwitchTo);
        yield return Fade(canvasToSwitchTo, 1f);
        _currentCanvasGroup = canvasToSwitchTo;
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha)
    {
        var time = 0f;
        var startAlpha = canvasGroup.alpha;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
    
    public void ShowPanel(CanvasGroup canvasGroup)
    {
        HideAllPanels();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void EnablePanel(CanvasGroup cg)
    {
        cg.interactable = true;
        cg.blocksRaycasts = true;
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
        
        var result = await MyController.GetWalletBalance();

        if (result.success && result.data.balance != null)
        {
            walletBalance.text = ConvertToSats(Int32.Parse(result.data.balance)).ToString();
        }
        else
        {
            walletBalance.text = "Error getting balance";
        }
    }
    
    
    #endregion
    
    #region Utilities
    
    public async void GetBTCPrice()
    {
        btcPriceText.text = "Sending Request...";
        
        var result = await MyController.GetBTCUSDExchangeRate();

        if (result.success && result.data.btcUsdPrice != null)
        {
            btcPriceText.text = result.data.btcUsdPrice;
        }
        else
        {
            btcPriceText.text = "Error getting price";
        }

    }

    public async void GetProductionIPs()
    {
        productionIPText.text = "Sending Request...";
        
        var result = await MyController.GetProductionIPs();

        if (result.success && result.data.ips.Count > 0)
        {
            productionIPText.text = result.data.ips[0];
        }
        else
        {
            productionIPText.text = "Error getting IPs";
        }

    }
    
    public async void GetSupportedRegions()
    {
        supportedRegionText.text = "Sending Request...";
        
        var ip = supportedRegionIPInput.text;
        
        var result = await MyController.IsSupportedRegion(ip);

        if (result.success && result.data.isSupported)
        {
            supportedRegionText.text = result.data.isSupported.ToString();
        }

    }
    
    #endregion
    
    #region Charges
    
    public async void CreateCharge()
    {
        createChargeChargeIDText.text = "Sending Request...";
        
        var amount = ConvertToMSats(Int32.Parse(createChargeAmountInput.text)).ToString();

        var result = await MyController.CreateCharge("300", amount, "Test Charge ");

        if (result.success && result.data.id != null)
        {
            createChargeChargeIDText.text = result.data.id;
        }
        else
        {
            createChargeChargeIDText.text = "Error creating charge";
        }
    }

    public async void GetChargeDetails()
    {
        getChargeDetailsAmountText.text = "Sending Request...";
        
        var chargeID = getChargeDetailsIDInput.text;
        
        var result = await MyController.GetChargeDetails(chargeID);

        if (result.success && result.data.status != null && result.data.amount != null)
        {
            getChargeDetailsMessageText.text = result.data.status;
            getChargeDetailsAmountText.text = ConvertToSats(Int32.Parse(result.data.amount)).ToString();
            
        }
        else
        {
            getChargeDetailsAmountText.text = "Error getting charge details";
        }
    }
    
    #endregion
    
    #region Withdrawals

    public async void CreateWithdrawalRequest()
    {
        createWithdrawalWithdrawalIDText.text = "Sending Request...";
        
        var amount = ConvertToMSats(Int32.Parse(createWithdrawalAmountInput.text)).ToString();

        var result = await MyController.CreateWithdrawal("300", amount, "Test Withdrawal");

        if (result.success && result.data.id != null)
        {
            createWithdrawalWithdrawalIDText.text = result.data.id;
        }
        else
        {
            createWithdrawalWithdrawalIDText.text = "Error creating withdrawal";
        }
    }

    public async void GetWithdrawalDetails()
    {
        getWithdrawalDetailsAmountText.text = "Sending Request...";
        
        var withdrawalID = getWithdrawalDetailsIDInput.text;
        
        var result = await MyController.GetWithdrawalRequestDetails(withdrawalID);

        if (result.success && result.data.status != null && result.data.amount != null)
        {
            getWithdrawalDetailsStatusText.text = result.data.status;
            getWithdrawalDetailsAmountText.text = ConvertToSats(Int32.Parse(result.data.amount)).ToString();
        }
        else
        {
            getWithdrawalDetailsAmountText.text = "Error getting withdrawal details";
        }
    }
    
    #endregion
    
    #region Gamertags
    
    public async void SendPaymentToGamertag()
    {
        gamertagTransactionIDText.text = "Sending Request...";
        
        var gamertag = gamertagPaymentInputField.text;
        var amount = ConvertToMSats(Int32.Parse(gamertagPaymentSatsInputField.text)).ToString();
    
        var result = await MyController.SendPaymentToGamertag(gamertag, amount, "Test Transaction");

        if (result.success && result.data.transactionId != null)
        {
            gamertagTransactionIDText.text = result.data.transactionId;
        }
        else
        {
            gamertagTransactionIDText.text = "Error sending payment";
        }
    }
    
    public async void GetGamertagByUserID()
    {
        
        fetchGamertagGamertagText.text = "Sending Request...";
        
        var userID = fetchGamertagUserIDInput.text;
        
        var result = await MyController.GetGamertagByUserID(userID);

        if (result.success && result.data.gamertag != null)
        {
            fetchGamertagGamertagText.text = result.data.gamertag;
        }
        else
        {
            fetchGamertagGamertagText.text = "Error getting gamertag";
        }
    }
    
    public async void GetUserIDByGamertag()
    {
        fetchUserIDUserIDText.text = "Sending Request...";
        
        var gamertag = fetchUserIDGamertagInput.text;
        
        var result = await MyController.GetUserIDByGamertag(gamertag);

        if (result.success && result.data.id != null)
        {
            fetchUserIDUserIDText.text = result.data.id;
        }
        else
        {
            fetchUserIDUserIDText.text = "Error getting user ID";
        }
    }
    
    #endregion
    
    #region Helper Functions

    private int ConvertToMSats(int satsAmount)
    {
        return satsAmount * 1000;
    }
    
    private int ConvertToSats(int msatsAmount)
    {
        return msatsAmount / 1000;
    }

    #endregion
    
    #region Buttons

    public void CopyToClipboard(TextMeshProUGUI field)
    {
        GUIUtility.systemCopyBuffer = field.text;
    }
    
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
    
    #endregion
}