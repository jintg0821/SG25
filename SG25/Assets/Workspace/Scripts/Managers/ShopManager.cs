using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using static ShopManager;

public class ShopManager : MonoBehaviour
{
    [Header("상점패널")]
    public GameObject productPrefab;//상품창 프리팹 변수
    public GameObject shopPanel;    //상점 패널
    public GameObject productContent;
    public Image image;         //물건 이미지 변수 
    public TextMeshProUGUI productName;      //상품 물건 이름 변수
    public TextMeshProUGUI price;               //상품 물건 가격 변수
    public ProductData[] products;
    public GameObject CartPanel;
    public TextMeshProUGUI currentCartMoney;
    public TextMeshProUGUI currentCartCount;
    public ProductBoxGenerator ProductBoxGenerator;
    public ProductBox productBox;
    public GameObject productBoxObj;        //상품 상자 프리팹

    [Header("장바구니 패널")]
    public int deliveryFee = 3000;
    public Button buyButton;    // 장바구니에서 '구매'
    public List<ProductData> productDatas = new List<ProductData>();
    public GameObject CartProductContent;
    public GameObject CartProductPrefab;

    public TextMeshProUGUI totalProductPriceText;
    public TextMeshProUGUI totalPriceText;
    public TextMeshProUGUI deliveryFeeText;
    public TextMeshProUGUI remainingMoneyText;

    [Header("플레이어 머니")]
    //public int playerMoney = 1000; // 초기 플레이어 돈 
    public TextMeshProUGUI PlayerMoneyText; // UI에서 플레이어 돈을 표시하는 텍스트
    public List<CartItem> cartItems = new List<CartItem>(); // 장바구니 항목 리스트

    void Start()
    {
        UpdatePlayerMoneyUI();
        products = Resources.LoadAll<ProductData>("Products");  //리소스 파일에 있는 ProductData타입을 모두 products배열에 넣는다.
        Generateproduct();
        GenerateCartProduct();

    }
    
    // 장바구니 항목을 제품과 수량을 함께 관리하는 구조.
    //productDatas 리스트에서 ProductData만 저장하는 것이 아니라, 제품과 그 수량을 함께 저장하는 방식으로 변경
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

    // 플레이어 돈 UI 업데이트
    public void UpdatePlayerMoneyUI()
    {
        PlayerMoneyText.text = GameManager.Instance.playerMoney.ToString(); // 플레이어의 돈을 텍스트에 표시
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
                price.text = products[index].buyCost.ToString();
                image.sprite = products[index].image;
            }
        }


    }

    public void CartBtnClick(TMP_InputField count, ProductData product)
    {
        int productCount = int.Parse(count.text);

        if (productCount > 0)
        {
            // 장바구니에 이미 같은 제품이 있는지 확인
            CartItem existingItem = cartItems.Find(item => item.product.ID == product.ID);

            if (existingItem != null)
            {
                // 같은 제품이 있을 경우 수량만 증가
                existingItem.quantity += productCount;
            }
            else
            {
                // 새로운 제품을 장바구니에 추가
                cartItems.Add(new CartItem(product, productCount));
            }
        }
        UpdateCartTotal();
    }

    public void UpdateCartTotal()
    {
        int totalProductPrice = CalculateTotalPrice(); // 장바구니 총 가격 계산
        currentCartMoney.text = totalProductPrice.ToString(); // UI에 총액 표시
        totalProductPriceText.text = totalProductPrice.ToString();

        int totalPrice = totalProductPrice + deliveryFee;
        totalPriceText.text = totalPrice.ToString();

        int remainingMoney = GameManager.Instance.playerMoney - totalPrice;
        remainingMoneyText.text = remainingMoney.ToString();

        int totalCount = cartItems.Count;
        currentCartCount.text = totalCount.ToString();
    }

    public void OnCartPanelButtonClick() //장바구니 버튼을 클릭했을 때 함수
    {
        CartPanel.SetActive(true); //Activ가 true면 활성화가 된다. false면 비활성화가 된다.
        GenerateCartProduct();
        deliveryFeeText.text = deliveryFee.ToString();
    }

    public void CartPanelClose()
    {
        CartPanel.SetActive(false);
    }

    public void GenerateCartProduct()
    {
        // 기존 장바구니 항목들을 모두 제거
        foreach (Transform child in CartProductContent.transform)
        {
            Destroy(child.gameObject);
        }

        // 장바구니에 담긴 항목들을 UI에 표시
        foreach (CartItem cartItem in cartItems)
        {
            GameObject cartProduct = Instantiate(CartProductPrefab, CartProductContent.transform);
            TextMeshProUGUI productName = cartProduct.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
            //Image productImage = cartProduct.transform.GetChild(1).GetComponentInChildren<Image>();
            TextMeshProUGUI productQuantity = cartProduct.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            Button plusButton = cartProduct.transform.GetChild(3).GetComponentInChildren<Button>(); //장바구니에서 '지우기'
            Button minusButton = cartProduct.transform.GetChild(4).GetComponentInChildren<Button>(); //장바구니에서 '지우기'
            TextMeshProUGUI productPrice = cartProduct.transform.GetChild(5).GetComponentInChildren<TextMeshProUGUI>();
            Button allRemoveButton = cartProduct.transform.GetChild(6).GetComponentInChildren<Button>();  //장바구니에서 '모두 지우기'
            

            minusButton.onClick.AddListener(() => CartMinus(cartItem));
            plusButton.onClick.AddListener(() => CartPlus(cartItem));
            allRemoveButton.onClick.AddListener(() => AllRemove(cartItem));

            // 제품 정보와 수량 표시
            productName.text = cartItem.product.name;
            productQuantity.text = $"x{cartItem.quantity}";
            productPrice.text = cartItem.product.buyCost.ToString();

            if (cartItem.quantity == 0 || cartItem == null)
            {
                Destroy(cartProduct.gameObject);
            } 
        }
    }


    public void OnBuyButtonClick()
    {
        int totalPrice = CalculateTotalPrice(); // 장바구니의 총 가격 계산

        // 플레이어의 돈이 총 가격보다 많거나 같을 때 구매 가능
        if (GameManager.Instance.playerMoney >= (totalPrice + deliveryFee))
        {
            GameManager.Instance.playerMoney -= (totalPrice + deliveryFee); // 플레이어 돈에서 총 가격 차감
            UpdatePlayerMoneyUI();     // UI 업데이트

            Debug.Log($"Items purchased for {totalPrice}. Remaining money: {GameManager.Instance.playerMoney}");

            foreach (CartItem cartItem in cartItems)
            {
                for (int i = 0; i < cartItem.quantity; i++)
                {
                    OnProductButtonClick(cartItem.product);
                }
            }

            ClearCart(); // 장바구니 비우기
        }
        else
        {
            Debug.Log("아이템을 살 돈이 부족합니다.");
        }
    }

    public void ClearCart()
    {
        cartItems.Clear(); // 장바구니 비우기
        GenerateCartProduct(); // UI 업데이트 (장바구니를 다시 그려서 비워진 상태 표시)
    }


    public void CountUp(TMP_InputField count)
    {
        int plus = int.Parse(count.text);
        plus++;
        count.text = plus.ToString();
        Debug.Log(count.text);

    }

    public void CountDown(TMP_InputField count)
    {
        int minus = int.Parse(count.text);
        minus--;
        count.text = minus.ToString();
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

        Debug.Log($"아이템 1개 삭제해서 {cartItem.quantity}개 남음~");
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

        Debug.Log("아이템 전부 삭제");
    }

    public int CalculateTotalPrice()
    {
        int totalPrice = 0;

        // 장바구니에 담긴 모든 제품의 총 가격 계산
        foreach (CartItem cartItem in cartItems)
        {
            totalPrice += cartItem.product.buyCost * cartItem.quantity; // 제품 가격 * 수량
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
