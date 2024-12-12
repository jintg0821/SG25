using UnityEngine;
using UnityEngine.UI;

public class CenterCameraRaycast : MonoBehaviour
{
    public Camera targetCamera;
    public float raycastRange = 100f; // ???????????? ???? ????

    public Outline currentOutline;

    public GameObject playerHand;
    public ProductBox productBox;
    public ProductBox lastBox;
    public CheckoutSystem checkoutSystem;
    public UIManager uiManager;

    private AudioSource audioSource;
    public AudioClip barcodeClip;
    public AudioClip moneyClip;

    void Start()
    {
        checkoutSystem = FindObjectOfType<CheckoutSystem>();
        uiManager = FindObjectOfType<UIManager>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        PerformRaycast();
    }

    public void SetCursorState(bool isVisible)
    {
        Cursor.visible = isVisible;
        if (!isVisible)
        {
            Cursor.lockState = CursorLockMode.Locked; // ?????? ???? ?????? ????
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // ?????? ?????? ?????? ???? ???????? ??????
        }
    }

    void PerformRaycast()
    {
        Ray ray = targetCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // ???????????? ?????? ?????? ?????? ??????????.
        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            Outline newOutline = hit.collider.gameObject.GetComponent<Outline>();

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    DoorController hitDoor = hit.collider.GetComponent<DoorController>();
                    hitDoor.ChangeDoorState();
                }
            }
            if (currentOutline != newOutline)
            {
                if (currentOutline != null)
                {
                    currentOutline.OutlineWidth = 0; // ???? ???????? ????
                    currentOutline = null;
                }

                currentOutline = newOutline;

                if (currentOutline != null)
                {
                    currentOutline.OutlineWidth = 10; // ?????? ???????? ????
                }
            }
            if (Input.GetMouseButtonDown(0))                                //?????? ???? ??
            {
                if (hit.collider.CompareTag("ProductBox"))  // ProductBox ?????? ?????? ??
                {
                    var p = hit.collider.GetComponent<ProductBoxInfo>();
                    productBox = hit.collider.GetComponent<ProductBox>();
                    if (productBox != null)
                    {
                        var collider = productBox.transform.gameObject.GetComponent<MeshCollider>();
                        collider.enabled = false;

                        // world traqnsform position
                        productBox.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        productBox.transform.SetParent(playerHand.transform);
                        productBox.transform.localPosition = Vector3.zero;
                        productBox.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                    }
                    //if (p != null)
                    //{
                    //    var boxCollider = p.transform.gameObject.GetComponent<BoxCollider>();                        
                    //    boxCollider.enabled = false;

                    //    // world traqnsform position
                    //    p.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    //    p.transform.SetParent(playerHand.transform);
                    //    p.transform.localPosition = Vector3.zero; 
                    //    p.transform.localRotation = Quaternion.identity;
                    //}



                    // 1. ???? ????
                    // 2. ???? ???? (???? ???? ???? ???? ????, ????, ????, ????)
                    // 3. ???? ???? (???? ?????? ?????? ???? ?? ???? ?????? ?????? ?? ??????)
                }

                if (hit.collider.CompareTag("Shelf"))                       // ???? ?????????? ???? ???? ?????? "Shelf"?? ??
                {
                    var shelf = hit.collider.gameObject.GetComponent<Shelf>();
                    if (shelf != null)
                    {
                        switch (shelf)
                        {
                            case SnackShelf:
                                {
                                    var snackShelf = shelf as SnackShelf;
                                    if (productBox != null)
                                    {
                                        var boxInfo = productBox.GetBoxInfo();
                                        if (boxInfo.ProductType == snackShelf.GetShelfType())
                                        {
                                            Debug.Log("?????? ???? " + productBox.ProductList.Count);

                                            if (productBox.ProductList.Count > 0)
                                            {   
                                                bool success = snackShelf.PushItem(productBox.ProductList[productBox.ProductList.Count - 1], boxInfo.ProductType);
                                                if (success)
                                                    productBox.ProductList.Remove(productBox.ProductList[productBox.ProductList.Count - 1]);
                                            }
                                        }
                                    }

                                }
                                break;
                            case DrinkShelf:
                                var drinkShelf = shelf as DrinkShelf;
                                if (productBox != null)
                                {
                                    var boxInfo = productBox.GetBoxInfo();
                                    if (boxInfo.ProductType == drinkShelf.GetShelfType())
                                    {
                                        if (productBox.ProductList.Count > 0)
                                        {
                                            bool success = drinkShelf.PushItem(productBox.ProductList[0], boxInfo.ProductType);
                                            if (success)
                                                productBox.ProductList.Remove(productBox.ProductList[0]);
                                        }
                                    }
                                }
                                break;
                            case IcecreamShelf:
                                var icecreamShelf = shelf as IcecreamShelf;
                                if (productBox != null)
                                {
                                    var boxInfo = productBox.GetBoxInfo();
                                    if (boxInfo.ProductType == icecreamShelf.GetShelfType())
                                    {
                                        if (productBox.ProductList.Count > 0)
                                        {
                                            bool success = icecreamShelf.PushItem(productBox.ProductList[0], boxInfo.ProductType);
                                            if (success)
                                                productBox.ProductList.Remove(productBox.ProductList[0]);
                                        }
                                    }
                                }
                                break;
                            default:
                                Debug.Log("Unknown shelf type");
                                break;
                        }
                    }
                }
                if (hit.collider.CompareTag("Product"))
                {
                    QuestManager questManager = FindObjectOfType<QuestManager>();
                    Product item = hit.collider.GetComponent<Product>();
                    
                    if (questManager != null)
                    {
                        questManager.OnItemClicked(item.product.ID);
                    }   
                }
                if (hit.collider.CompareTag("CounterProduct"))
                {
                    GameObject counterProductObj = hit.collider.gameObject;
                    checkoutSystem.SelectedProduct(counterProductObj);
                    audioSource.clip = barcodeClip;
                    audioSource.Play();
                }

                if (hit.collider.CompareTag("Money"))
                {
                    Money moneyObj = hit.collider.GetComponent<Money>();
                    //checkoutSystem.takeMoneys.Add(moneyObj.money.value);
                    //checkoutSystem.takeMoney += moneyObj.money.value;
                    //checkoutSystem.takeMoneys.Remove(moneyObj.money.value);
                    checkoutSystem.Calculate(moneyObj);
                    audioSource.clip = moneyClip;
                    audioSource.Play();
                    Destroy(moneyObj.gameObject);
                }
            }
            
            if (Input.GetMouseButtonDown(1))                                        //???????? ???? ??
            {
                if (hit.collider.CompareTag("Shelf"))                               //ray?? ???? ?????????? "Shelf" ?????? ?????? ??????
                {
                    var shelf = hit.collider.gameObject.GetComponent<Shelf>();
                    if (shelf != null)
                    {
                        switch (shelf)
                        {
                            case SnackShelf:
                                {
                                    var snackShelf = shelf as SnackShelf;
                                    if (productBox != null)
                                    {
                                        var boxInfo = productBox.GetBoxInfo();
                                        GameObject productObj = snackShelf.ProductList[snackShelf.ProductList.Count - 1];
                                        if (boxInfo.ProductType == snackShelf.GetShelfType() && productObj.GetComponent<Product>().product.ID == boxInfo.ProductID)
                                        {
                                            productBox.InsertProduct(productObj);
                                            snackShelf.PopItem(productObj, boxInfo.ProductType);
                                        }
                                    }
                                }
                                break;
                            case DrinkShelf:
                                var drinkShelf = shelf as DrinkShelf;
                                if (productBox != null)
                                {
                                    var boxInfo = productBox.GetBoxInfo();
                                    GameObject productObj = drinkShelf.ProductList[drinkShelf.ProductList.Count - 1];
                                    if (boxInfo.ProductType == drinkShelf.GetShelfType() && productObj.GetComponent<Product>().product.ID == boxInfo.ProductID)
                                    {
                                        productBox.InsertProduct(productObj);
                                        drinkShelf.PopItem(productObj, boxInfo.ProductType);
                                    }
                                }
                                break;
                            case IcecreamShelf:
                                var icecreamShelf = shelf as IcecreamShelf;
                                if (productBox != null)
                                {
                                    var boxInfo = productBox.GetBoxInfo();
                                    GameObject productObj = icecreamShelf.ProductList[icecreamShelf.ProductList.Count - 1];
                                    if (boxInfo.ProductType == icecreamShelf.GetShelfType() && productObj.GetComponent<Product>().product.ID == boxInfo.ProductID)
                                    {
                                        productBox.InsertProduct(productObj);
                                        icecreamShelf.PopItem(productObj, boxInfo.ProductType);
                                    }
                                }
                                break;
                            default:
                                Debug.Log("Unknown shelf type");
                                break;
                        }
                    }
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))                                                    //F?? ?????? ??
        {
            if (productBox != null)                                                         //productBox?? ???? ??????
            {
                productBox.GetComponent<MeshCollider>().enabled = true;
                productBox.GetComponent<Rigidbody>().isKinematic = false;
                productBox.transform.SetParent(null);
                productBox = lastBox;
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiManager.TogglePanel(uiManager.shopPanel);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiManager.TogglePanel(uiManager.menuPanel);
        }
    }
}
