using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductBoxGenerator : MonoBehaviour
{
    private List<ProductBoxScriptObject> productBoxInfoList = new List<ProductBoxScriptObject>();
    
    public GameObject BoxPrefab;
    public Transform Pivot;

    public void GetOrder(ProductBoxScriptObject info, ProductData product)
    {
        info.ProductName = product.name;
        info.ProductType = (int)product.productType;
        productBoxInfoList.Add(info);
    }

    public void GetOrder(List<ProductBoxScriptObject> infoList, ProductData product)
    {
        foreach (var info in infoList)
        {
            info.ProductName = product.name;
            info.ProductType = (int)product.productType;
            productBoxInfoList.Add(info);
        }
    }

    public void GenerateProductBox(ProductData product)
    {
        foreach (var item in productBoxInfoList)
        {
            var output = Instantiate(BoxPrefab, Pivot.position, Quaternion.identity) as GameObject;
            var info = output.GetComponent<ProductBoxInfo>();
            
            info.ProductName = item.ProductName;
            info.ProductType = item.ProductType;
            info.ProductCount = info.ProductPosList.Count;

            for (int i = 0; i < info.ProductPosList.Count; i++)
            {
                GameObject productObj = Instantiate(product.ProductModel, info.ProductPosList[i].transform);
                var productBox = output.GetComponent<ProductBox>();
                productBox.ProductList.Add(productObj);
                productObj.transform.localPosition = Vector3.zero;
                productObj.transform.localScale = new Vector3(5, 5, 5);
                productObj.GetComponent<BoxCollider>().enabled = false;
            }
        }
        productBoxInfoList.Clear();
    }
}
