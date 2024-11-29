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
    
    public float gameTime = 0.0f; // ���� ������ �ð��� �ʷ� ����
    public float timeScale = 60.0f; // ���� �ð� 1�ʴ� ���� �ð� 60�� (1��)
    public int hours = 0; // % 24�� �����Ͽ� 24 �̻� ���� ��� ����
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
        // ���� ������ ��� �ð��� ����Ͽ� ���� �ð� ����
        gameTime += Time.deltaTime * timeScale;
        // 25�ð� ������ ���� ����
        if (hours >= 25)
        {
            Debug.Log("������ ���� �Ǿ����ϴ�.");
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
