using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
                                                snackShelf.PushItem(productBox.ProductList[0], boxInfo.ProductType);
                                                productBox.ProductList.Remove(productBox.ProductList[0]);
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
                                            drinkShelf.PushItem(productBox.ProductList[0], boxInfo.ProductType);
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
            if (Input.GetMouseButtonDown(1))                                        //???????? ???? ??
            {
                if (hit.collider.CompareTag("Shelf"))                               //ray?? ???? ?????????? "Shelf" ?????? ?????? ??????
                {
                    //SnackShelf hitShelf = hit.collider.GetComponent<SnackShelf>();    //ray?? ???? ?????????????? ShelfCtrl ?????????? ???? ????.
                    //if (hitShelf.SnackList.Count != 0)                            //hitShelf?? ?????? ???????? ???? ??????
                    //{
                    //    GameObject productObj = hitShelf.SnackList[hitShelf.SnackList.Count - 1];        //productObj?? hitShelf?? ???? ???? ???? ?????? ???? ???? ???? ???????? ???????? ??????.
                    //    var boxInfo = productBox.GetComponent<ProductBoxInfo>();
                    //    productBox.InsertProduct(productObj);
                    //    hitShelf.PopItem(productObj, boxInfo.ProductType);
                    //    uiManager.OnProductBoxInfo(boxInfo.ProductName, boxInfo.ProductCount);

                    //}
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
            uiManager.ToggleShopPanel();
        }
    }
}
