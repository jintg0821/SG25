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
    public Animator animator;
    private CheckoutSystem checkoutSystem;

    public bool isMoveDone = false;

    public Transform target;
    public Transform counter;
    public Transform counterPivot;
    public Transform customerHand;

    public GameObject[] exitPoints;
    public Transform exitPoint;
    public MoneyData[] moneyPrefabs;

    public List<Transform> targetPosList = new List<Transform>();
    public List<GameObject> pickProductList = new List<GameObject>();
    public List<GameObject> shelfList = new List<GameObject>();
    public Dictionary<ProductData, int> targetProduct = new Dictionary<ProductData, int>();

    public AudioSource AudioSource;
    public AudioClip[] footStepClips;
    public AudioClip giveMoneyClip;

    public float footstepDistance = 0.2f;
    float currentFootstepDistance = 0f;

    [Range(0f, 1f)]
    public float audioClipVolume = 0.1f;

    public float relativeRandomizedVolumeRange = 0.2f;

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
        checkoutSystem = FindObjectOfType<CheckoutSystem>();
        counter = GameObject.Find("Counter").transform;
        counterPivot = GameObject.Find("CounterPivot").transform;
        exitPoints = GameObject.FindGameObjectsWithTag("CustomerPoint");
        exitPoint = exitPoints[Random.Range(0, exitPoints.Length)].transform;

        currentState = CustomerState.Idle;

        AssignPriority();
        SearchShelfs();
        TargetProduct();
    }

    void Update()
    {
        timer.Update(Time.deltaTime);

        if (!agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                isMoveDone = true;
            }
            if (AudioSource != null && AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }
        }
        else
        {
            if (AudioSource != null && !AudioSource.isPlaying)
            {
                FootStepUpdate(agent.velocity.magnitude);
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
                Debug.Log($"AGENT : {agent.avoidancePriority} ID : {targetP.ID} NAME : {targetP.name} COUNT : {count}");
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
            animator.SetInteger("CustomerState", 1);
            animator.CrossFade("Walking", 0f);
            agent.SetDestination(target.position);

            if (!AudioSource.isPlaying)
            {
                AudioSource.Play();
            }
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
                animator.SetInteger("CustomerState", 1);
                animator.CrossFade("Walking", 0f);
                ChangeState(CustomerState.WalkingToShelf, waitTime);
            }
            else
            {
                if (pickProductList.Count > 0)
                {
                    animator.SetInteger("CustomerState", 1);
                    animator.CrossFade("Walking", 0f);
                    ChangeState(CustomerState.WaitCounter, waitTime);
                }
                else
                {
                    animator.SetInteger("CustomerState", 1);
                    animator.CrossFade("Walking", 0f);
                    ChangeState(CustomerState.LeavingStore, waitTime);
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
            if (!isPicking) // 현재 Picking 중인 상태를 확인
            {
                isPicking = true; // Picking 시작
                StartCoroutine(PickingProductCoroutine());
            }
        }
    }

    bool isPicking = false; // Picking 상태를 관리하는 변수

    IEnumerator PickingProductCoroutine()
    {
        Shelf shelf = target.GetComponent<Shelf>();

        foreach (var targetP in targetProduct.ToList())
        {
            int currentCount = pickProductList.Count(p =>
                p.GetComponent<Product>().product.ID == targetP.Key.ID);

            int requiredCount = targetP.Value;
            int productType = (int)targetP.Key.productType;

            if (currentCount < requiredCount)
            {
                for (int i = 0; i < requiredCount - currentCount; i++)
                {
                    GameObject product = shelf.ProductList.LastOrDefault(p => p.GetComponent<Product>().product.ID == targetP.Key.ID);

                    if (product != null)
                    {
                        animator.SetInteger("CustomerState", 2); // Picking 애니메이션 실행
                        animator.CrossFade("Picking", 0f);

                        yield return WaitForAnimationToFinish("Picking"); // 애니메이션 이름
                        //yield return new WaitForSeconds(1f);

                        shelf.PopItem(product, productType);
                        product.transform.SetParent(customerHand);
                        product.transform.localPosition = Vector3.zero;
                        product.transform.localRotation = Quaternion.identity;
                        product.SetActive(false);
                        pickProductList.Add(product);
                    }
                }
            }
            else
            {
                targetPosList.Remove(shelf.transform);
            }
        }

        isPicking = false; // Picking 완료
        animator.SetInteger("CustomerState", 1);
        animator.CrossFade("Walking", 0f);
        if (pickProductList.Count > 0)
        {
            animator.SetInteger("CustomerState", 1); // Walking 애니메이션으로 변경
            animator.CrossFade("Walking", 0f);
            ChangeState(CustomerState.WaitCounter);
        }
        else
        {
            animator.SetInteger("CustomerState", 1); // Walking 애니메이션으로 변경
            animator.CrossFade("Walking", 0f);
            ChangeState(CustomerState.LeavingStore);
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
                MoveToTarget();
                if (isMoveDone)
                {
                    animator.SetInteger("CustomerState", 0);
                    animator.CrossFade("Idle", 0);
                }
            }
        }

        if (!isCounterOccupied && pickProductList.Count > 0)
        {
            target = counterPivot;
            animator.SetInteger("CustomerState", 1);
            animator.CrossFade("Walking", 0);
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
        if (pickProductList.Count > 0)
        {
            StartCoroutine(PlaceProductCoroutine());
        }
        else
        {
            ChangeState(CustomerState.GivingMoney, waitTime);
        }
    }

    IEnumerator PlaceProductCoroutine()
    {
        while (pickProductList.Count > 0)
        {
            animator.SetInteger("CustomerState", 3);
            animator.CrossFade("Placing", 0f);
            yield return WaitForAnimationToFinish("Placing");

            if (pickProductList.Count > 0)
            {
                GameObject product = pickProductList[pickProductList.Count - 1];
                pickProductList.Remove(product);

                product.SetActive(true);
                product.GetComponent<BoxCollider>().enabled = true;
                product.transform.parent = counter;
                //product.transform.position = new Vector3(counter.position.x, counter.position.y + 2f, counter.position.z);
                product.transform.localPosition = new Vector3(0f, 0.02f, 0f);
                product.transform.rotation = Quaternion.Euler(product.transform.rotation.eulerAngles.x, product.transform.rotation.eulerAngles.y, 90f);
                
                product.SetActive(true);
                product.tag = "CounterProduct";
                checkoutSystem.counterProductList.Add(product);
            }
        }
        ChangeState(CustomerState.GivingMoney, waitTime);
    }

    void GiveMoney(int amount)
    {
        System.Array.Sort(moneyPrefabs, (a, b) => b.value.CompareTo(a.value));
        //bool giveExactChange = Random.Range(0, 2) == 0;

        foreach (MoneyData money in moneyPrefabs)
        {
            if (amount >= money.value)
            {
                int count = amount / money.value;
                amount -= count * money.value;

                for (int i = 0; i < count; i++)
                {
                    GameObject moneyObj = Instantiate(money.moneyModel, customerHand.transform.position, Quaternion.identity);
                    moneyObj.transform.SetParent(customerHand);
                    moneyObj.transform.localPosition = new Vector3(0.085f, -0.03f, -0.085f);
                    moneyObj.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                    checkoutSystem.takeMoneys.Add(money.value);
                }
            }
        }
    }

    void GivingMoney()
    {
        if (timer.IsFinished() && checkoutSystem.counterProductList.Count == 0)
        {
            transform.LookAt(checkoutSystem.gameObject.transform);
            StartCoroutine(GiveMoneyCoroutine());

            GiveMoney(checkoutSystem.totalPrice);
            for (int i = 0; i < checkoutSystem.takeMoneys.Count; i++)
            {
                int sum = 0;
                sum += checkoutSystem.takeMoneys[i];
                Debug.Log(sum);
            }
            AudioSource.PlayOneShot(giveMoneyClip, audioClipVolume);
            ChangeState(CustomerState.WaitingCalcPrice, waitTime);
        }
    }

    IEnumerator GiveMoneyCoroutine()
    {
        animator.SetInteger("CustomerState", 4);
        animator.CrossFade("GiveMoney", 0f);

        yield return WaitForAnimationToFinish("GiveMoney");
    }

    void WaitingCalcPrice()
    {
        if (checkoutSystem.takeMoneys.Count == 0 && checkoutSystem.counterProductList.Count == 0)
        {
            //checkoutSystem.ShowChangeAmount();
            Debug.Log(checkoutSystem.changeMoney);
            checkoutSystem.isCalculating = true;
            if (checkoutSystem.isSell == true)
            {
                checkoutSystem.isSell = false;
                animator.SetInteger("CustomerState", 1);
                animator.CrossFade("Walking", 0f);
                ChangeState(CustomerState.LeavingStore, waitTime);
            }
        }
    }

    void LeavingStore()
    {
        target = exitPoint;
        animator.SetInteger("CustomerState", 1);
        animator.CrossFade("Walking", 0f);
        MoveToTarget();

        if (!isMoveDone && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f)
        {
            isMoveDone = true;
        }

        if (timer.IsFinished() && isMoveDone)
        {
            Destroy(gameObject);
            Debug.Log("Customer Leaving");
        }
    }

    //IEnumerator PlaceProduct()
    //{
    //    Debug.Log("PlaceProduct Method Start");

    //    foreach (var product in pickProductList)
    //    {
    //        product.transform.SetParent(counter);
    //        product.transform.localPosition = new Vector3(0f, 1f, 0f);
    //        product.transform.localRotation = Quaternion.identity;

    //        pickProductList.Remove(product);

    //        yield return new WaitForSeconds(1f);
    //    }
    //    ChangeState(CustomerState.GivingMoney, waitTime);
    //}

    IEnumerator WaitForAnimationToFinish(string animationName)
    {
        // 현재 애니메이션 클립이 재생 중인지 확인
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) ||
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // 다음 프레임까지 대기
        }
    }

    void FootStepUpdate(float _movementSpeed)
    {
        float _speedThreshold = 0.05f;

        currentFootstepDistance += Time.deltaTime * _movementSpeed;

        //Play foot step audio clip if a certain distance has been traveled;
        if (currentFootstepDistance > footstepDistance)
        {
            //Only play footstep sound if movement speed is above the threshold;
            if (_movementSpeed > _speedThreshold)
                PlayFootstepSound(_movementSpeed);
            currentFootstepDistance = 0f;
        }
    }

    void PlayFootstepSound(float _movementSpeed)
    {
        if (footStepClips.Length > 0)
        {
            int _footStepClipIndex = Random.Range(0, footStepClips.Length);
            float volume = audioClipVolume + audioClipVolume * Random.Range(-relativeRandomizedVolumeRange, relativeRandomizedVolumeRange);
            AudioSource.PlayOneShot(footStepClips[_footStepClipIndex], volume);
        }
    }
}
