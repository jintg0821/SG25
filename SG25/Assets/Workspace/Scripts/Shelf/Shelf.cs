using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfType 
{
    public const int SHELF_SNACK = 0;
    public const int SHELF_DRINK = 1;
    public const int SHELF_ICECREAM = 2;
}

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
                    Transform availablePosition = ProductPoseList[ProductList.Count];
                    product.transform.SetParent(availablePosition);
                    product.transform.localPosition = Vector3.zero;
                    product.transform.localScale = Vector3.one;
                    product.transform.localRotation = Quaternion.identity;
                    ProductList.Add(product);
                }
                else
                {
                    Debug.Log("과자 진열대 꽉참");
                }
            }
            else
            {
                Debug.Log("진열대에 자리 없음");
            }
        }
    }
}