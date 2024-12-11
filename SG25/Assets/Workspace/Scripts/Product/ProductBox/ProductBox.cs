using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductBox : MonoBehaviour
{
    public List<GameObject> ProductList = new List<GameObject>();

    public ProductBoxInfo GetBoxInfo()
    {
        return this.GetComponent<ProductBoxInfo>();
    }

    public GameObject RemoveProduct(GameObject productObj) // ???? ???? ???? ???????? ?????? ????
    {
        var info = gameObject.GetComponent<ProductBoxInfo>();

        if (info.ProductPosList.Count > 0)
        {
            ProductList.Remove(productObj);
            --info.ProductCount; 
        }
        else
        {
            Debug.Log("?????? ????????");
        }
        return null;
    }

    public GameObject InsertProduct(GameObject productObj) // ?????? ?????? ???? ????
    {
        var info = gameObject.GetComponent<ProductBoxInfo>();
        var newProduct = productObj.GetComponent<Product>();
        if ((int)newProduct.product.productType == info.ProductType)
        {
            if (ProductList.Count < info.ProductPosList.Count)
            {
                ProductList.Add(productObj);
                ++info.ProductCount;
                productObj.transform.SetParent(info.ProductPosList[ProductList.Count - 1].transform);
                productObj.transform.localPosition = Vector3.zero;
                productObj.transform.localScale = Vector3.one;
                productObj.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.Log("???? ????");
            }
        }
        return null;
    }
}
