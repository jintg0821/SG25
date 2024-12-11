using System.Collections.Generic;
using UnityEngine;

public class CheckoutSystem : MonoBehaviour
{
    public List<GameObject> counterProductList = new List<GameObject>();
    public List<int> takeMoneys = new List<int>();

    public int totalPrice = 0;
    public int changeMoney = 0;
    public int takeMoney = 0;
    public bool isSell = false;
    public bool isCalculating = false;

    public UIManager UIManager;

    void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
    }

    public void SelectedProduct(GameObject productObj)
    {
        productObj = counterProductList[counterProductList.Count - 1];
        Product product = productObj.GetComponent<Product>();
        counterProductList.Remove(productObj);
        totalPrice += product.product.sellCost;
        Destroy(productObj);
        Debug.Log($"{product.name} : {product.product.sellCost}");
    }

    public void Calculate(Money moneyObj)
    {
        takeMoney += moneyObj.money.value;
        takeMoneys.Remove(moneyObj.money.value);

        if (takeMoneys.Count == 0)
        {
            GameManager.Instance.AddMoney(takeMoney);
            isSell = true;
            totalPrice = 0;
            changeMoney = 0;
            takeMoney = 0;
            QuestManager.Instance.Calculate();
            GameManager.Instance.dailyCalculationCount++;
            GameManager.Instance.totalCalculationCount++;
        }
    }

    //public void ShowChangeAmount()
    //{
    //    if (takeMoneys.Count == 0)
    //    {
    //        if (takeMoney >= totalPrice)
    //        {
    //            changeMoney = takeMoney - totalPrice;
    //            UIManager.ShowChangeText(changeMoney);
    //        }
    //    }
    //}

    //public void CalculateChange()
    //{
    //    if (isCalculating)
    //    {
    //        if (changeMoney > 0)
    //        {   
    //            changeMoney = takeMoney - changeMoney;
    //            UIManager.IncreaseMoneyText(changeMoney);
    //        }
    //        else if (changeMoney == 0)
    //        {
    //            UIManager.IncreaseMoneyText(takeMoney);
    //        }
    //        Debug.Log(changeMoney);
    //        isSell = true;
    //        isCalculating = false;
    //        takeMoney = 0;
    //        totalPrice = 0;
    //        changeMoney = 0;
    //        takeMoneys.Clear();
            
    //    }
        
    //}
}