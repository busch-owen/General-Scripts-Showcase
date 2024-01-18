using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    #region So many fun variables I'm gonna have to assign later :D

    [SerializeField]
    TMP_Text moneyText;

    [SerializeField]
    Transform moneyTextTargetPos;

    [SerializeField]
    Transform moneyTextDefaultPos;

    [SerializeField] 
    Transform warehouseEnterPromptText;

    [SerializeField] 
    Transform warehouseEnterDefaultPos;

    [SerializeField]
    Transform warehouseEnterTargetPos;

    [SerializeField] 
    TMP_Text paycheckValueText;

    [SerializeField] 
    TMP_Text paycheckAccuracyBonusText;

    [SerializeField]
    TMP_Text paycheckTotalText;

    [SerializeField]
    TMP_Text runLevelText;

    [SerializeField]
    Image runLevelProgressBarFill;

    [SerializeField]
    Gradient levelProgressFillGradient;

    bool showMoneyText = false;

    ModShop modShop;

    public GameObject modShopMenu;
    public GameObject warehouseMenu;

    PlayerStats playerStats;

    [SerializeField]
    bool inWorldCanvas;

    #endregion

    #region Unity Runtime Crap

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("VehicleStats");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        modShop = FindObjectOfType<ModShop>();

        if(moneyText != null)
        {
            //this lerps the money text
            if (!showMoneyText)
            {
                moneyText.transform.position = Vector3.Lerp(moneyText.transform.position, moneyTextDefaultPos.position, 6f * Time.deltaTime);
            }
            else
                moneyText.transform.position = Vector3.Lerp(moneyText.transform.position, moneyTextTargetPos.position, 8f * Time.deltaTime);
        }    
    }

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        modShop = FindObjectOfType<ModShop>();
    }

    #endregion

    #region Player Stats UI Handling
    public void ShowMoneyText()
    {
        showMoneyText = true;
        UpdateMoneyText();
        Invoke("HideMoneyText", 5f);
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "Money: " + playerStats.playerMoney.ToString("c2");
    }

    public void ShowMoneyTextAndStay()
    {
        showMoneyText = true;
        UpdateMoneyText();
    }
    
    public void HideMoneyText()
    {
        showMoneyText = false;
    }

    public void UpdateLevelStats(int level, float levelProgress)
    {
        runLevelProgressBarFill.type = Image.Type.Filled;
        runLevelText.text = "Level: " + level;
        runLevelProgressBarFill.color = levelProgressFillGradient.Evaluate(runLevelProgressBarFill.fillAmount);
        runLevelProgressBarFill.fillAmount = levelProgress;
    }

    #endregion

    #region All of the Warehouse Stuff

    public void LerpEnterTextIntoPos()
    {
        warehouseEnterPromptText.position = Vector3.Lerp(warehouseEnterPromptText.position, warehouseEnterTargetPos.position, 3f * Time.deltaTime);
    }
    public void LerpEnterTextOutOfPos()
    {
        warehouseEnterPromptText.position = Vector3.Lerp(warehouseEnterPromptText.position, warehouseEnterDefaultPos.position, 6f * Time.deltaTime);
    }

    public void RefreshPaycheckTexts(float paycheckValue, float accuracyBonusValue)
    {
        paycheckValueText.text = "$" + paycheckValue;
        paycheckAccuracyBonusText.text = $"${Mathf.Abs(accuracyBonusValue)} Accuracy Negations";
        paycheckTotalText.text = "$" + (paycheckValue + accuracyBonusValue);
    }
    public void ResetPaycheckTexts()
    {
        paycheckValueText.text = "$0";
        paycheckAccuracyBonusText.text = "$0 Accuracy Negations";
        paycheckTotalText.text = "$0";
    }

    #endregion

    #region Mod Shop Interface

    [SerializeField]
    Transform modShopEnterPromptText;

    [SerializeField]
    Transform modShopEnterDefaultPos;

    [SerializeField]
    Transform modShopEnterTargetPos;

    [SerializeField]
    TMP_Text[] priceTextObjects;

    public void LerpModShopEnterTextIntoPos()
    {
        modShopEnterPromptText.position = Vector3.Lerp(modShopEnterPromptText.position, modShopEnterTargetPos.position, 3f * Time.deltaTime);
    }
    public void LerpModShopEnterTextOutOfPos()
    {
        modShopEnterPromptText.position = Vector3.Lerp(modShopEnterPromptText.position, modShopEnterDefaultPos.position, 6f * Time.deltaTime);
    }

    public void OpenSpecificMenu(GameObject menuToOpen)
    {
        menuToOpen.SetActive(true);
        modShop.currentMenu = menuToOpen;
        ShowMoneyTextAndStay();
    }

    public void UpdatePriceTexts(float upgradePrice, TMP_Text textAsset)
    {
        textAsset.text = "Price: " + upgradePrice.ToString("c2");
    }

    public void ChangePriceToEquipped(bool equipped, TMP_Text textAsset)
    {
        if(equipped)
        {
            textAsset.text = "Currently Equipped";
        }
    }

    public void PurchaseButton(Upgrade upgrade)
    {
        modShop.PurchaseUpgrade(upgrade);
    }

    public void UpgradeSoldOut(TMP_Text textAsset)
    {
        textAsset.text = "SOLD OUT";
    }

    #endregion

    #region General UI

    public void DisableAllMenus()
    {
        modShopMenu.SetActive(false);
        warehouseMenu.SetActive(false);
    }

    #endregion
}
