using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfPurchaser : MonoBehaviour
{
    public GameObject snackShelfPrefab;
    public GameObject drinkShelfPrefab;
    public GameObject iceCreamShelfPrefab;
    public Transform playerHand;

    public void PurchaseSnackShelf()
    {
        PurchaseShelf(snackShelfPrefab);
    }

    public void PurchaseDrinkShelf()
    {
        PurchaseShelf(drinkShelfPrefab);
    }

    public void PurchaseIceCreamShelf()
    {
        PurchaseShelf(iceCreamShelfPrefab);
    }

    private void PurchaseShelf(GameObject shelfPrefab)
    {
        Shelf shelf = shelfPrefab.GetComponentInChildren<Shelf>();
        if (shelf != null)
        {
            Debug.Log($"Shelf Price: {shelf.Price}, Player Money: {GameManager.Instance.playerMoney}");
            if (GameManager.Instance.playerMoney >= shelf.Price)
            {
                GameManager.Instance.SpendMoney(shelf.Price);
                GameObject newShelf = Instantiate(shelfPrefab, playerHand.position, playerHand.rotation, playerHand);
                newShelf.AddComponent<ShelfPlacer>();
            }
            else
            {
                Debug.Log("Not enough money to buy this shelf!");
            }
        }
        else
        {
            Debug.LogError("Shelf component not found on the prefab!");
        }
    }
}
