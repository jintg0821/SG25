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
                WaitCounter();
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
                Debug.Log($"목표 상품 : ID{targetP.ID}{targetP.name} {count}개 담음");
                foreach (var shelfObj in shelfList)
                {
                    Shelf shelf = shelfObj.GetComponent<Shelf>();
                    if (shelf.ProductList.Contains(targetP.ProductModel))
                    {
                        targetPosList.Add(shelf.transform);
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
                ChangeState(CustomerState.WaitCounter, waitTime);
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

            }
        }
    }

    void WaitCounter()
    {

    }

    void WalkingToCounter()
    {

    }

    void PlacingProduct()
    {

    }

    void GivingMoney()
    {

    }

    void WaitingCalcPrice()
    {

    }

    void LeavingStore()
    {

    }
}
