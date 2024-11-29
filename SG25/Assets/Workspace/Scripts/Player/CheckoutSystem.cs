using System.Collections.Generic;
using UnityEngine;

public class CheckoutSystem : MonoBehaviour
{
    public List<GameObject> counterProduct = new List<GameObject>();
    public List<int> takeMoneys = new List<int>();

    public int totalPrice = 0;
    public int changeMoney = 0;
    public int takeMoney = 0;
    public bool isSell = false;
    public bool isCalculating = false;

    public UIManager UIManager;

    public void SelectedProduct(GameObject productObj)
    {
        productObj = counterProduct[counterProduct.Count - 1];
        Product product = productObj.GetComponent<Product>();
        counterProduct.Remove(productObj);
        //totalPrice += product.product.sellCost;
        Destroy(productObj);
        Debug.Log(totalPrice);
    }

    public void Calculate(Money moneyObj)
    {
        takeMoney += moneyObj.money.value;
        takeMoneys.Remove(moneyObj.money.value);

        if (takeMoneys.Count == 0)
        {
            UIManager.IncreaseMoneyText(takeMoney);
            isSell = true;
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