using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // �̱���
    public int playerMoney = 1000;      // �ʱ� �ݾ�

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int amount, UIManager uiManager)
    {
        playerMoney += amount;
        uiManager.UpdateMoneyText(playerMoney);
    }

    public void SpendMoney(int amount, UIManager uiManager)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            uiManager.UpdateMoneyText(playerMoney);
        }
        else
        {
            Debug.Log("Not enough money!");
        }
    }
}
