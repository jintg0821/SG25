using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return instance;
        }
    }

    [Header("Time")]
    
    public float gameTime = 0.0f; // 게임 세계의 시간을 초로 저장
    public float timeScale = 60.0f; // 현실 시간 1초당 게임 시간 60초 (1분)
    public int hours = 0; // % 24를 제거하여 24 이상 값도 계산 가능
    public int minutes = 0;
    [Header("")]
    public int playerMoney = 10000;

    private UIManager UIManager;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        hours = (int)(gameTime / 3600);
        minutes = (int)(gameTime / 60) % 60;
        // 현실 세계의 경과 시간에 비례하여 게임 시간 증가
        gameTime += Time.deltaTime * timeScale;
        // 25시가 넘으면 영업 종료
        if (hours >= 25)
        {
            Debug.Log("영업이 종료 되었습니다.");
        }

        
    }

    public void AddMoney(int amount, UIManager uiManager)
    {
        playerMoney += amount;
        //uiManager.UpdateMoneyText(playerMoney);
    }

    public void SpendMoney(int amount, UIManager uiManager)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            //uiManager.UpdateMoneyText(playerMoney);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}
