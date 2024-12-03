using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class CenterCameraRaycast : MonoBehaviour
{
    public Camera targetCamera;
    public float raycastRange = 100f; // 레이캐스트의 최대 거리

    public Outline currentOutline;

    public GameObject playerHand;
    public ProductBox productBox;
    public ProductBox lastBox;
    public CheckoutSystem checkoutSystem;
    public UIManager uiManager;
    void Update()
    {
        PerformRaycast();
    }

    public void SetCursorState(bool isVisible)
    {
        Cursor.visible = isVisible;
        if (!isVisible)
        {
            Cursor.lockState = CursorLockMode.Locked; // 커서를 화면 중앙에 고정
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // 커서를 화면에 보이게 하고 자유롭게 움직임
        }
    }

    void PerformRaycast()
    {
        Ray ray = targetCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        // 레이캐스트가 충돌한 물체가 있는지 확인합니다.
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
                    currentOutline.OutlineWidth = 0; // 이전 아웃라인 해제
                    currentOutline = null;
                }

                currentOutline = newOutline;

                if (currentOutline != null)
                {
                    currentOutline.OutlineWidth = 10; // 새로운 아웃라인 적용
                }
            }
            if (Input.GetMouseButtonDown(0))                                //좌클릭 했을 때
            {
                if (hit.collider.CompareTag("ProductBox"))  // ProductBox 태그에 닿았을 때
                {
                    var p = hit.collider.GetComponent<ProductBoxInfo>();
                    productBox = hit.collider.GetComponent<ProductBox>();
                    if (productBox != null)
                    {
                        var boxCollider = productBox.transform.gameObject.GetComponent<BoxCollider>();
                        boxCollider.enabled = false;

                        // world traqnsform position
                        productBox.transform.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        productBox.transform.SetParent(playerHand.transform);
                        productBox.transform.localPosition = Vector3.zero;
                        productBox.transform.localRotation = Quaternion.identity;
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



                    // 1. 박스 클릭
                    // 2. 박스 정보 (박스 안에 있는 상품 이름, 개수, 사진, 타입)
                    // 3. 박스 이동 (집고 상자에 물건이 있을 땐 넣고 상자가 비었을 땐 버린다)
                }

                if (hit.collider.CompareTag("Shelf"))                       // 닿은 콜라이더가 갖고 있는 태그가 "Shelf"일 때
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
                                            Debug.Log("아이템 갯수 " + productBox.ProductList.Count);

                                            if (productBox.ProductList.Count > 0)
                                            {
                                                snackShelf.PushItem(productBox.ProductList[0], boxInfo.ProductType);
                                                productBox.ProductList.Remove(productBox.ProductList[0]);
                                            }
                                        }
                                    }

                                }
                                Debug.Log("SnackShelf");
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
                                            drinkShelf.PushItem(productBox.ProductList[0], boxInfo.ProductType);
                                            productBox.ProductList.Remove(productBox.ProductList[0]);
                                        }
                                    }
                                }
                                Debug.Log("DrinkShelf");
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
            }
            if (hit.collider.CompareTag("CounterProduct"))
            {
                GameObject counterProductObj = hit.collider.gameObject;
                checkoutSystem.SelectedProduct(counterProductObj);
            }
            
            if (hit.collider.CompareTag("Money"))
            {
                Money moneyObj = hit.collider.GetComponent<Money>();
                //checkoutSystem.takeMoneys.Add(moneyObj.money.value);  
                //checkoutSystem.takeMoney += moneyObj.money.value;
                //checkoutSystem.takeMoneys.Remove(moneyObj.money.value);
                checkoutSystem.Calculate(moneyObj);
                Destroy(moneyObj.gameObject);
            }
            if (Input.GetMouseButtonDown(1))                                        //우클릭을 했을 때
            {
                if (hit.collider.CompareTag("Shelf"))                               //ray에 닿은 오브젝트가 "Shelf" 태그를 가지고 있다면
                {
                    //SnackShelf hitShelf = hit.collider.GetComponent<SnackShelf>();    //ray에 닿은 오브젝트에게서 ShelfCtrl 컴포넌트를 갖고 온다.
                    //if (hitShelf.SnackList.Count != 0)                            //hitShelf가 상품을 하나라도 갖고 있다면
                    //{
                    //    GameObject productObj = hitShelf.SnackList[hitShelf.SnackList.Count - 1];        //productObj는 hitShelf가 갖고 있는 상품 목록의 가장 위에 있는 오브젝트 데이터를 가진다.
                    //    var boxInfo = productBox.GetComponent<ProductBoxInfo>();
                    //    productBox.InsertProduct(productObj);
                    //    hitShelf.PopItem(productObj, boxInfo.ProductType);
                    //    uiManager.OnProductBoxInfo(boxInfo.ProductName, boxInfo.ProductCount);

                    //}
                }
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))                                                    //F를 눌렀을 때
        {
            if (productBox != null)                                                         //productBox를 들고 있다면
            {
                productBox.GetComponent<BoxCollider>().enabled = true;
                productBox.transform.SetParent(null);
                productBox = lastBox;
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uiManager.ToggleShopPanel();
        }
    }
}
