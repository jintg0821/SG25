using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : Singleton<ShopManager>
{
    [Header("????????")]
    public GameObject productPrefab;//?????? ?????? ????
    public GameObject shopPanel;    //???? ????
    public GameObject productContent;
    public Image image;         //???? ?????? ???? 
    public TextMeshProUGUI productName;      //???? ???? ???? ????
    public TextMeshProUGUI price;               //???? ???? ???? ????
    public ProductData[] products;
    public GameObject CartPanel;
    public TextMeshProUGUI currentCartMoney;
    public TextMeshProUGUI currentCartCount;
    public ProductBoxGenerator ProductBoxGenerator;
    public ProductBox productBox;
    public GameObject productBoxObj;        //???? ???? ??????

    [Header("???????? ????")]
    public int deliveryFee = 3000;
    public Button buyButton;    // ???????????? '????'
    public List<ProductData> productDatas = new List<ProductData>();
    public GameObject CartProductContent;
    public GameObject CartProductPrefab;

    public TextMeshProUGUI totalProductPriceText;
    public TextMeshProUGUI totalPriceText;
    public TextMeshProUGUI deliveryFeeText;
    public TextMeshProUGUI remainingMoneyText;

    [Header("???????? ????")]
    //public int playerMoney = 1000; // ???? ???????? ?? 
    public TextMeshProUGUI PlayerMoneyText; // UI???? ???????? ???? ???????? ??????
    public List<CartItem> cartItems = new List<CartItem>(); // ???????? ???? ??????

    void Start()
    {
        UpdatePlayerMoneyUI();
        products = Resources.LoadAll<ProductData>("Products");  //?????? ?????? ???? ProductData?????? ???? products?????? ??????.
        Generateproduct();
        GenerateCartProduct();

    }
    
    // ???????? ?????? ?????? ?????? ???? ???????? ????.
    //productDatas ?????????? ProductData?? ???????? ???? ??????, ?????? ?? ?????? ???? ???????? ???????? ????
    public class CartItem
    {
        public ProductData product;
        public int quantity;

        public CartItem(ProductData product, int quantity)
        {
            this.product = product;
            this.quantity = quantity;
        }
    }

    // ???????? ?? UI ????????
    public void UpdatePlayerMoneyUI()
    {
        PlayerMoneyText.text = GameManager.Instance.playerMoney.ToString("N0"); // ?????????? ???? ???????? ????
    }

    public void Generateproduct()
    {
        for (int i = 0; i < products.Length; i++)
        {
            GameObject productObj = Instantiate(productPrefab, productContent.transform);

            // Get references to the components
            TextMeshProUGUI productName = productObj.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            Image image = productObj.transform.GetChild(2).GetComponentInChildren<Image>();
            TextMeshProUGUI price = productObj.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
            TMP_InputField count = productObj.transform.GetChild(4).GetComponentInChildren<TMP_InputField>();
            count.text = "1";

            Button plusBtn = productObj.transform.GetChild(5).GetComponentInChildren<Button>();
            Button minusBtn = productObj.transform.GetChild(6).GetComponentInChildren<Button>();
            Button CartBtn = productObj.transform.GetChild(7).GetComponentInChildren<Button>();
            int index = i;
            // Store a local copy of the count input field
            TMP_InputField localCount = count;
            CartBtn.onClick.AddListener(()=> CartBtnClick(count, products[index]));
            plusBtn.onClick.AddListener(() => CountUp(localCount));
            minusBtn.onClick.AddListener(() => CountDown(localCount));


            if (productObj != null && products[index] != null)
            {
                productName.text = products[index].name;               
                price.text = products[index].buyCost.ToString("N0");
                image.sprite = products[index].image;
            }
        }
    }

    public void CartBtnClick(TMP_InputField count, ProductData product)
    {
        int productCount = int.Parse(count.text);

        if (productCount > 0)
        {
            // ?????????? ???? ???? ?????? ?????? ????
            CartItem existingItem = cartItems.Find(item => item.product.ID == product.ID);

            if (existingItem != null)
            {
                // ???? ?????? ???? ???? ?????? ????
                existingItem.quantity += productCount;
            }
            else
            {
                // ?????? ?????? ?????????? ????
                cartItems.Add(new CartItem(product, productCount));
            }
        }
        UpdateCartTotal();
    }

    public void UpdateCartTotal()
    {
        int totalProductPrice = CalculateTotalPrice(); // ???????? ?? ???? ????
        currentCartMoney.text = totalProductPrice.ToString("N0"); // UI?? ???? ????
        totalProductPriceText.text = totalProductPrice.ToString("N0");

        int totalPrice = totalProductPrice + deliveryFee;
        totalPriceText.text = totalPrice.ToString("N0");

        int remainingMoney = GameManager.Instance.playerMoney - totalPrice;
        remainingMoneyText.text = remainingMoney.ToString("N0");

        int totalCount = cartItems.Count;
        currentCartCount.text = totalCount.ToString("N0");
    }

    public void OnCartPanelButtonClick() //???????? ?????? ???????? ?? ????
    {
        CartPanel.SetActive(true); //Activ?? true?? ???????? ????. false?? ?????????? ????.
        GenerateCartProduct();
        deliveryFeeText.text = deliveryFee.ToString("N0");
    }

    public void CartPanelClose()
    {
        CartPanel.SetActive(false);
    }

    public void GenerateCartProduct()
    {
        // ???? ???????? ???????? ???? ????
        foreach (Transform child in CartProductContent.transform)
        {
            Destroy(child.gameObject);
        }

        // ?????????? ???? ???????? UI?? ????
        foreach (CartItem cartItem in cartItems)
        {
            GameObject cartProduct = Instantiate(CartProductPrefab, CartProductContent.transform);
            TextMeshProUGUI productName = cartProduct.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            //Image productImage = cartProduct.transform.GetChild(1).GetComponentInChildren<Image>();
            TextMeshProUGUI productQuantity = cartProduct.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            Button plusButton = cartProduct.transform.GetChild(3).GetComponentInChildren<Button>(); //???????????? '??????'
            Button minusButton = cartProduct.transform.GetChild(4).GetComponentInChildren<Button>(); //???????????? '??????'
            TextMeshProUGUI productPrice = cartProduct.transform.GetChild(5).GetComponentInChildren<TextMeshProUGUI>();
            Button allRemoveButton = cartProduct.transform.GetChild(6).GetComponentInChildren<Button>();  //???????????? '???? ??????'
            

            minusButton.onClick.AddListener(() => CartMinus(cartItem));
            plusButton.onClick.AddListener(() => CartPlus(cartItem));
            allRemoveButton.onClick.AddListener(() => AllRemove(cartItem));

            // ???? ?????? ???? ????
            productName.text = cartItem.product.name;
            productQuantity.text = $"x{cartItem.quantity}";
            productPrice.text = cartItem.product.buyCost.ToString("N0");

            if (cartItem.quantity == 0 || cartItem == null)
            {
                Destroy(cartProduct.gameObject);
            } 
        }
    }


    public void OnBuyButtonClick()
    {
        int totalPrice = CalculateTotalPrice(); // ?????????? ?? ???? ????

        // ?????????? ???? ?? ???????? ?????? ???? ?? ???? ????
        if (GameManager.Instance.playerMoney >= (totalPrice + deliveryFee))
        {
            GameManager.Instance.SpendMoney(totalPrice + deliveryFee); // ÇÃ·¹ÀÌ¾î µ·¿¡¼­ ÃÑ °¡°Ý Â÷°¨
            UpdatePlayerMoneyUI();     // UI ¾÷µ¥ÀÌÆ®

            Debug.Log($"Items purchased for {totalPrice}. Remaining money: {GameManager.Instance.playerMoney}");

            foreach (CartItem cartItem in cartItems)
            {
                for (int i = 0; i < cartItem.quantity; i++)
                {
                    OnProductButtonClick(cartItem.product);
                }
            }

            ClearCart(); // ???????? ??????
        }
        else
        {
            Debug.Log("???????? ?? ???? ??????????.");
        }
    }

    public void ClearCart()
    {
        cartItems.Clear(); // ???????? ??????
        GenerateCartProduct(); // UI ???????? (?????????? ???? ?????? ?????? ???? ????)
    }


    public void CountUp(TMP_InputField count)
    {
        int plus = int.Parse(count.text);
        plus++;
        count.text = plus.ToString("N0");
        Debug.Log(count.text);

    }

    public void CountDown(TMP_InputField count)
    {
        int minus = int.Parse(count.text);
        minus--;
        count.text = minus.ToString("N0");
        Debug.Log(count.text);
    }

    public void CartMinus(CartItem cartItem)
    {
        cartItem.quantity--;

        if (cartItem.quantity <= 0)
        {
            cartItems.Remove(cartItem);
        }

        GenerateCartProduct();
        UpdateCartTotal();

        Debug.Log($"?????? 1?? ???????? {cartItem.quantity}?? ????~");
    }

    public void CartPlus(CartItem cartItem)
    {
        cartItem.quantity++;
        GenerateCartProduct();
        UpdateCartTotal();
    }

    public void AllRemove(CartItem cartItem)
    {
        cartItems.Remove(cartItem);

        GenerateCartProduct();

        Debug.Log("?????? ???? ????");
    }

    public int CalculateTotalPrice()
    {
        int totalPrice = 0;

        // ?????????? ???? ???? ?????? ?? ???? ????
        foreach (CartItem cartItem in cartItems)
        {
            totalPrice += cartItem.product.buyCost * cartItem.quantity; // ???? ???? * ????
        }
        return totalPrice;
    }
    public void OnProductButtonClick(ProductData product)
    {
        var productInfo = new ProductBoxScriptObject();
            ProductBoxGenerator.GetOrder(productInfo, product);
        ProductBoxGenerator.GenerateProductBox(product);
    }
}
