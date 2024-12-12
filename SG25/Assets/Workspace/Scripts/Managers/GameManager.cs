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
    public float gameTime = 0.0f; // 게임 세계의 시간을 초로 저장
    public float timeScale = 360.0f; // 현실 시간 1초당 게임 시간 60초 (1분) 360 => 4분
    public int hours = 6; // % 24를 제거하여 24 이상 값도 계산 가능
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
        // 현실 세계의 경과 시간에 비례하여 게임 시간 증가
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
        if (hours >= 6 && hours < 16) // 낮
        {
            SetSkybox(daySkybox);
        }
        else if (hours >= 16 && hours < 20) // 저녁
        {
            SetSkybox(eveningSkybox);
        }
        else // 밤
        {
            SetSkybox(nightSkybox);
        }
    }

    void SetSkybox(Material skybox)
    {
        if (currentSkybox != skybox) // 스카이박스가 변경되었을 때만 설정
        {
            RenderSettings.skybox = skybox;
            currentSkybox = skybox;
            DynamicGI.UpdateEnvironment(); // 환경 반사를 업데이트
        }
    }
}
