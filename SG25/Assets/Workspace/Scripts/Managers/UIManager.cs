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

    void Start()
    {
        moneyText.text = GameManager.Instance.playerMoney.ToString();
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
    }

    public void IncreaseMoneyText(int amount)
    {
        GameManager.Instance.playerMoney += amount;
        moneyText.text = GameManager.Instance.playerMoney.ToString();
    }

    public void DecreaseMoneyText(int amount)
    {
        GameManager.Instance.playerMoney -= amount;
        moneyText.text = GameManager.Instance.playerMoney.ToString();
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
}
