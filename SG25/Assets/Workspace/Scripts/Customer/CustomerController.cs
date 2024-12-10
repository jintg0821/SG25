using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerState
{
    Idle,
    WalkingToShelf,
    PickingProduct,
    WaitCounter,
    WalkingToCounter,
    PlacingProduct,
    WaitingCalcPrice,
    GivingMoney,
    LeavingStore
}

public class Timer
{
    private float timeRemaining;

    public void Set(float time)
    {
        timeRemaining = time;
    }

    public void Update(float deltaTime)
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= deltaTime;
        }
    }

    public bool IsFinished()
    {
        return timeRemaining <= 0;
    }
}

public class CustomerController : MonoBehaviour
{
    public float waitTime = 0.5f;

    public CustomerState currentState;
    private Timer timer;
    public NavMeshAgent agent;
    //public Animator animator;

    public bool isMoveDone = false;

    public Transform target;
    public Transform counter;
    public Transform customerHand;

    public Transform[] exitPoints;
    public MoneyData[] moneyPrefabs;

    public List<Transform> targetPosList = new List<Transform>();
    public List<GameObject> pickProductList = new List<GameObject>();
    public List<GameObject> shelfList = new List<GameObject>();
    public Dictionary<ProductData, int> targetProduct = new Dictionary<ProductData, int>();

    private static int nextPriority = 0;
    private static readonly object priorityLock = new object();

    void AssignPriority()
    {
        lock (priorityLock)
        {
            agent.avoidancePriority = nextPriority;
            nextPriority = (nextPriority + 1) % 100;
        }
    }

    void Start()
    {
        timer = new Timer();
        agent = GetComponent<NavMeshAgent>();
        counter = GameObject.Find("Counter").transform;

        currentState = CustomerState.Idle;

        AssignPriority();
        SearchShelfs();
        TargetProduct();
    }

    void Update()
    {
        timer.Update(Time.deltaTime);

        if (!agent.hasPath && agent.remainingDistance <= agent.remainingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                isMoveDone = true;
            }
        }

        switch (currentState)
        {
            case CustomerState.Idle:
                Idle();
                break;
            case CustomerState.WalkingToShelf:
                WalkingToShelf();
                break;
            case CustomerState.PickingProduct:
                PickingProduct();
                break;
            case CustomerState.WaitCounter:
                WaitCounter();
                break;
            case CustomerState.WalkingToCounter:
                WalkingToCounter();
                break;
            case CustomerState.PlacingProduct:
                PlacingProduct();
                break;
            case CustomerState.GivingMoney:
                GivingMoney();
                break;
            case CustomerState.WaitingCalcPrice:
                WaitingCalcPrice();
                break;
            case CustomerState.LeavingStore:
                LeavingStore();
                break;
        }
    }

    void SearchShelfs()
    {
        GameObject[] shelfs = GameObject.FindGameObjectsWithTag("Shelf");

        if (shelfs != null)
        {
            foreach (GameObject shelf in shelfs)
            {
                shelfList.Add(shelf);
            }
        }
    }

    void TargetProduct()
    {
        ProductData[] products = Resources.LoadAll<ProductData>("Products");
        
        for (int i = 0; i < 6; i++)
        {
            int count = Random.Range(0, 5);
            ProductData targetP = products[Random.Range(0, products.Length)];
            if (!targetProduct.ContainsKey(targetP) && count > 0)
            {
                targetProduct.Add(targetP, count);
                Debug.Log($"ID : {targetP.ID} NAME : {targetP.name} COUNT : {count}");
                foreach (var shelfObj in shelfList)
                {
                    Shelf shelf = shelfObj.GetComponent<Shelf>();
                    foreach (var shelfProduct in shelf.ProductList)
                    {
                        Product product = shelfProduct.GetComponent<Product>();
                        if (product.product.ID == targetP.ID && !targetPosList.Contains(shelf.transform))
                        {
                            targetPosList.Add(shelf.transform);
                            break;
                        }
                    }
                }
            }
        }
    }

    void ChangeState(CustomerState nextState, float waitTime = 0.0f)
    {
        currentState = nextState;
        timer.Set(waitTime);
    }

    void MoveToTarget()
    {
        isMoveDone = false;
        if (targetPosList != null)
        {
            agent.SetDestination(target.position);
        }
    }

    void Idle()
    {
        if (timer.IsFinished())
        {
            if (targetPosList.Count > 0)
            {
                target = targetPosList[Random.Range(0, targetPosList.Count)];
                MoveToTarget();
                ChangeState(CustomerState.WalkingToShelf, waitTime);
            }
            else
            {
                if (pickProductList.Count > 0 && targetProduct.Count == 0)
                {
                    ChangeState(CustomerState.WaitCounter, waitTime);
                }
                else
                {
                    //ChangeState(CustomerState.LeavingStore, waitTime);
                }
            }
        }
    }

    void WalkingToShelf()
    {
        if (timer.IsFinished() && isMoveDone)
        {
            ChangeState(CustomerState.PickingProduct, waitTime);
        }
    }

    void PickingProduct()
    {
        if (timer.IsFinished() && isMoveDone)
        {
            Shelf shelf = target.GetComponent<Shelf>();

            foreach (var targetP in targetProduct)
            {
                int productType = (int)targetP.Key.productType;
                int requiredCount = targetP.Value;
                Debug.Log($"requiredCount : {requiredCount}");

                for (int i = 0; i < requiredCount; i++)
                {
                    GameObject product = shelf.ProductList.FirstOrDefault(p => (int)p.GetComponent<Product>().product.productType == productType);
                    
                    if (product != null)
                    {
                        shelf.PopItem(product, productType);

                        product.transform.SetParent(customerHand);
                        product.transform.localPosition = Vector3.one;
                        product.transform.localRotation = Quaternion.identity;

                        pickProductList.Add(product);
                        
                        ChangeState(CustomerState.Idle);
                        Debug.Log("A");
                    }
                }
                targetPosList.Remove(shelf.transform);
                ChangeState(CustomerState.Idle);
                Debug.Log("B");
            }   
        }
    }

    Transform GetAvailableCounterLinePosition()
    {
        GameObject[] counterLine = GameObject.FindGameObjectsWithTag("CounterLine");

        foreach (GameObject pos in counterLine)
        {
            bool positionOccupied = false;
            CustomerController[] allCustomers = FindObjectsOfType<CustomerController>();
            foreach (var customer in allCustomers)
            {
                if (customer != this && customer.target == pos.transform)
                {
                    positionOccupied = true; ;
                    break;
                }
            }

            if (!positionOccupied)
            {
                return pos.transform;
            }
        }
        return null;
    }

    void WaitCounter()
    {
        CustomerController[] allCustomers = FindObjectsOfType<CustomerController>();
        bool isCounterOccupied = false;
        foreach (var customer in allCustomers)
        {
            if (customer != this && customer.currentState == CustomerState.WaitingCalcPrice || customer.currentState == CustomerState.GivingMoney
                || customer.currentState == CustomerState.WalkingToCounter || customer.currentState == CustomerState.PlacingProduct)
            {
                isCounterOccupied = true;
                break;
            }
        }

        if (isCounterOccupied)
        {
            Transform availablePosition = GetAvailableCounterLinePosition();
            if (availablePosition != null)
            {
                target = availablePosition;
                agent.SetDestination(availablePosition.position);
            }
        }

        if (!isCounterOccupied && pickProductList.Count > 0)
        {
            target = counter;
            MoveToTarget();
            ChangeState(CustomerState.WalkingToCounter, waitTime);
        }
    }

    void WalkingToCounter()
    {
        if (timer.IsFinished() && isMoveDone)
        {
            ChangeState(CustomerState.PlacingProduct, waitTime);
        }
    }

    void PlacingProduct()
    {
        if (timer.IsFinished() && isMoveDone)
        {
            PlaceProduct();

            if (pickProductList.Count == 0) PlaceProduct();
        }
    }

    void GivingMoney()
    {
        //if (timer.IsFinished() && checkoutSystem.counterProduct.Count == 0)
        //{
        //    GiveMoney(checkoutSystem.totalPrice);
        //    for (int i = 0; i < checkoutSystem.takeMoneys.Count; i++)
        //    {
        //        int sum = 0;
        //        sum += checkoutSystem.takeMoneys[i];
        //        Debug.Log(sum);
        //    }
        //    animator.SetBool("Idle", true);
        //    ChangeState(CustomerState.WaitingCalcPrice, waitTime);
        //}
    }

    void WaitingCalcPrice()
    {
        //if (checkoutSystem.takeMoneys.Count == 0 && checkoutSystem.counterProduct.Count == 0)
        //{
        //    //checkoutSystem.ShowChangeAmount();
        //    Debug.Log(checkoutSystem.changeMoney);
        //    checkoutSystem.isCalculating = true;
        //    if (checkoutSystem.isSell == true)
        //    {
        //        checkoutSystem.isSell = false;
        //        animator.SetBool("Idle", true);
        //        ChangeState(CustomerState.LeavingStore, waitTime);
        //    }

        //}
    }

    void LeavingStore()
    {
        Destroy(gameObject);
        Debug.Log("Customer Leaving");
    }

    IEnumerator PlaceProduct()
    {
        foreach (var product in pickProductList)
        {
            product.transform.SetParent(counter);
            product.transform.localPosition = new Vector3(0f, 1f, 0f);
            product.transform.localRotation = Quaternion.identity;

            pickProductList.Remove(product);

            yield return new WaitForSeconds(1f);
        }
        ChangeState(CustomerState.GivingMoney, waitTime);
    }
}
