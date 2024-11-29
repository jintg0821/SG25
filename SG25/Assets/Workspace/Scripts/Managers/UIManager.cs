using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    private CenterCameraRaycast playerCtrl;
    public GameObject shopPanel;
    public GameObject cartPanel;
    public bool isPanelOn;
    public TextMeshProUGUI timeText; // UI�� ���� ���� �ð��� ǥ���ϴ� �ؽ�Ʈ

    void Start()
    {
        moneyText.text = GameManager.Instance.playerMoney.ToString();
        playerCtrl = FindObjectOfType<CenterCameraRaycast>();
    }

    private void Update()
    {
        // UI ������Ʈ (���� �ð� ���)
        timeText.text = string.Format("{0:00}:{1:00}", GameManager.Instance.hours % 24, GameManager.Instance.minutes);
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
