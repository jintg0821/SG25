using UnityEngine;
using UnityEngine.UI;

public enum PRODUCTTYPE
{
    Beverages,
    Snacks,
    FrozenFoods,
    Null
}

[CreateAssetMenu(fileName = "NewProductData", menuName = "ScriptableObjects/ProductModel")]
public class ProductData : ScriptableObject
{
    public PRODUCTTYPE productType;
    public int ID;
    public string Name;
    public Sprite image;
    public GameObject ProductModel;
    public int buyCost;
    public int sellCost;
}
