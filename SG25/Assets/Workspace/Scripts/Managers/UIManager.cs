using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public TextMeshProUGUI moneyText;
    private CenterCameraRaycast playerCtrl;
    public GameObject shopPanel;
    public GameObject cartPanel;
    public bool isPanelOn;

    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;

    [Header("DailyStatisticsPanel")]
    public GameObject DailyStatisticsPanel;
    public TextMeshProUGUI dailyEarningsText;
    public TextMeshProUGUI dailyExpensesText;
    public TextMeshProUGUI dailyCalculationCountText;
    public TextMeshProUGUI dailyProfitText;
    public TextMeshProUGUI DailyStatisticsDayText;
    public Button nextDayButton;

    void Start()
    {
        moneyText.text = GameManager.Instance.playerMoney.ToString("N0");
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
        nextDayButton.onClick.AddListener(StoreOpen);
        GameManager.Instance.hours = 6;
        GameManager.Instance.minutes = 0;
        GameManager.Instance.gameTime = 6 * 3600;
        DailyStatisticsPanel.SetActive(false);
        playerCtrl.SetCursorState(false);
    }

    void Update()
    {
        timeText.text = $"{GameManager.Instance.hours:D2} : {GameManager.Instance.minutes:D2}";
        dayText.text = $"{GameManager.Instance.days.ToString()}일";

        StoreClose();
    }

    public void IncreaseMoneyText(int amount)
    {
        moneyText.text = GameManager.Instance.playerMoney.ToString("N0");
    }

    public void DecreaseMoneyText(int amount)
    {
        moneyText.text = GameManager.Instance.playerMoney.ToString("N0");
    }

    public void ToggleShopPanel()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
        playerCtrl.SetCursorState(shopPanel.activeSelf);
    } 
    
    public void ClosePanel()
    {
        cartPanel.SetActive(false);
        playerCtrl.SetCursorState(false);
    }

    public void OnDailyStatisticsPanel()
    {
        DailyStatisticsPanel.SetActive(true);
        playerCtrl.SetCursorState(true);
        dailyEarningsText.text = $"수입 : {GameManager.Instance.dailyEarnings.ToString("N0")}";
        dailyExpensesText.text = $"구매한 상품 비용 : {GameManager.Instance.dailyExpenses.ToString("N0")}";
        dailyCalculationCountText.text = $"총 고객 {GameManager.Instance.dailyCalculationCount.ToString("N0")}";
        dailyProfitText.text = $"총액 : {GameManager.Instance.dailyProfit.ToString("N0")}";
        DailyStatisticsDayText.text = $"{GameManager.Instance.days}일차";
    }

    public void StoreOpen()
    {
        DailyStatisticsPanel.SetActive(false);
        playerCtrl.SetCursorState(false);
        GameManager.Instance.days++;
        GameManager.Instance.hours = 6;
        GameManager.Instance.minutes = 0;
        GameManager.Instance.gameTime = 6 * 3600;
        GameManager.Instance.dailyCalculationCount = 0;
        GameManager.Instance.dailyEarnings = 0;
        GameManager.Instance.dailyExpenses = 0;
        GameManager.Instance.dailyProfit = 0;
    }

    public void StoreClose()
    {
        if (GameManager.Instance.hours >= 22)
        {
            GameManager.Instance.dailyProfit = (GameManager.Instance.dailyEarnings - GameManager.Instance.dailyExpenses);
            OnDailyStatisticsPanel();
            
            Debug.Log("영업이 종료 되었습니다.");
        }
        
    }
}
