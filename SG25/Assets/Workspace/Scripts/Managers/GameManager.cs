using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [Header("Skybox Settings")]
    public Material daySkybox;
    public Material eveningSkybox;
    public Material nightSkybox;

    private Material currentSkybox;

    [Header("Time")] 
    public float gameTime = 0.0f; // ���� ������ �ð��� �ʷ� ����
    public float timeScale = 360.0f; // ���� �ð� 1�ʴ� ���� �ð� 60�� (1��) 360 => 4��
    public int hours = 6; // % 24�� �����Ͽ� 24 �̻� ���� ��� ����
    public int minutes = 0;
    public int days = 1;
    [Header("")]
    public int playerMoney = 10000;
    [Header("Total")]
    public int dailyEarnings = 0;
    public int totalEarnings = 0;
    public int dailyExpenses = 0;
    public int totalExpenses = 0;
    public int dailyCalculationCount = 0;
    public int totalCalculationCount = 0;
    public int dailyProfit = 0;
    public int totalProfit = 0;

    void Update()
    {
        hours = (int)(gameTime / 3600) % 24;
        minutes = (int)(gameTime / 60) % 60;
        // ���� ������ ��� �ð��� ����Ͽ� ���� �ð� ����
        gameTime += Time.deltaTime * timeScale;

        UpdateSkybox();
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;

        dailyEarnings += amount;
        totalEarnings += amount;

        UIManager UIManager = FindObjectOfType<UIManager>();
        UIManager.IncreaseMoneyText(playerMoney);
    }

    public void SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;

            dailyExpenses += amount;
            totalExpenses += amount;

            UIManager UIManager = FindObjectOfType<UIManager>();
            UIManager.DecreaseMoneyText(playerMoney);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }

    void UpdateSkybox()
    {
        if (hours >= 6 && hours < 16) // ��
        {
            SetSkybox(daySkybox);
        }
        else if (hours >= 16 && hours < 20) // ����
        {
            SetSkybox(eveningSkybox);
        }
        else // ��
        {
            SetSkybox(nightSkybox);
        }
    }

    void SetSkybox(Material skybox)
    {
        if (currentSkybox != skybox) // ��ī�̹ڽ��� ����Ǿ��� ���� ����
        {
            RenderSettings.skybox = skybox;
            currentSkybox = skybox;
            DynamicGI.UpdateEnvironment(); // ȯ�� �ݻ縦 ������Ʈ
        }
    }
}
