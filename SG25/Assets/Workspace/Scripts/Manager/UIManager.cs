using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText; // �Ӵ� UI
    [SerializeField] private GameObject shopPanel;      // ���� �г�

    // �ʱ� UI ����
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
