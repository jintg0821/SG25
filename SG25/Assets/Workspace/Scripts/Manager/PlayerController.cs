using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;

    public void EarnMoney(int amount)
    {
        gameManager.AddMoney(amount, uiManager);
    }

    public void SpendMoney(int amount)
    {
        gameManager.SpendMoney(amount, uiManager);
    }

    public void ToggleShop()
    {
        uiManager.ToggleShopPanel();
    }
}
