using System.Collections.Generic;
using UnityEngine;
using MyGame.QuestSystem;

public abstract class Shelf : MonoBehaviour
{
    public List<Transform> ProductPoseList = new List<Transform>();
    public List<GameObject> ProductList = new List<GameObject>();
     
    public abstract int GetShelfType();

    public int GetSize()
    {
        return ProductList.Count;
    }

    public void PopItem(GameObject product, int productType)
    {
        if (GetShelfType() == productType)
        {
            if (ProductList.Count > 0)
            {
                ProductList.Remove(product);
            }
        }
    }

    public void PushItem(GameObject product, int productType)
    {
        if (GetShelfType() == productType)
        {
            Transform nullPose = null;
            foreach (Transform pose in ProductPoseList)
            {
                if (pose.childCount == 0)
                {
                    nullPose = pose;
                    break;
                }
            }
            if (nullPose != null)
            {
                if (ProductList.Count < ProductPoseList.Count)
                {
                    
                    Product newProduct = product.GetComponent<Product>();
                    if (ProductList.Count != 0)
                    {   
                        Product firstProduct = ProductList[0].GetComponent<Product>();
                        if (firstProduct.product.ID == newProduct.product.ID)
                        {
                            Transform availablePosition = ProductPoseList[ProductList.Count];
                            product.transform.SetParent(availablePosition);
                            product.transform.localPosition = Vector3.zero;
                            product.transform.localScale = Vector3.one;
                            product.transform.localRotation = Quaternion.identity;
                            ProductList.Add(product);
                        }
                    }
                    else
                    {
                        Transform availablePosition = ProductPoseList[ProductList.Count];
                        product.transform.SetParent(availablePosition);
                        product.transform.localPosition = Vector3.zero;
                        product.transform.localScale = Vector3.one;
                        product.transform.localRotation = Quaternion.identity;
                        ProductList.Add(product);
                    }
                    QuestManager questManager = FindObjectOfType<QuestManager>();
                    if (questManager != null)
                    {
                        questManager.OnItemClicked(newProduct.product.ID);
                    }

                    QuestManager.Instance.ItemShelfStock(productObj.product.ID);
                    QuestManager.Instance.ItemTypeShelfStock((int)productObj.product.productType);
                    
                }
                else
                {
                    Debug.Log("full");
                }
            }
        }
    }
}