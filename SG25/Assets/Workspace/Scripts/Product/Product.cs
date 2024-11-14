using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProductData", menuName = "ScriptableObjects/ProductModel")]
public class ProductData : ScriptableObject
{
    public enum IPRODUCTTYPE
    {
        Drink,
        Snack,
        DairyProducts,
        Icecream
    }

    public IPRODUCTTYPE ProductType;
    public int ID;
    public string Name;
    public Sprite Image;
    public GameObject ProductPrefab;
}

public class Product : MonoBehaviour
{
    public ProductData productData;
    public int buyCost;
    public int sellCost;
}