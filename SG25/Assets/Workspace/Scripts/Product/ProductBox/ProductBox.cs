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

    public GameObject RemoveProduct(GameObject productObj) // 박스 안에 있는 상품들을 지우는 함수
    {
        var info = gameObject.GetComponent<ProductBoxInfo>();

        if (info.ProductPosList.Count > 0)
        {
            ProductList.Remove(productObj);
            --info.ProductCount; 
            Debug.Log($"남은 물건 갯수 : {info.ProductCount}");
        }
        else
        {
            Debug.Log("박스가 비었어여");
        }
        return null;
    }

    public GameObject InsertProduct(GameObject productObj) // 상품을 박스에 넣는 함수
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
                productObj.transform.localScale = new Vector3(5f, 5f, 5f);
            }
            else
            {
                Debug.Log("박스 꽉참");
            }
        }
        return null;
    }
}
