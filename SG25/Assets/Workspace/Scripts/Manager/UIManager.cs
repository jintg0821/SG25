using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText; // 머니 UI
    [SerializeField] private GameObject shopPanel;      // 상점 패널

    // 초기 UI 세팅
    public void InitializeUI(int initialMoney)
    {
        UpdateMoneyText(initialMoney);
        shopPanel.SetActive(false);
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = $"Money: {money}";
    }

    public void ToggleShopPanel()
    {
        shopPanel.SetActive(!shopPanel.activeSelf);
    }
}
