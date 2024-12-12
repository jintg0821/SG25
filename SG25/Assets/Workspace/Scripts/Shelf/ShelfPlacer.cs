using UnityEngine;

public class ShelfPlacer : MonoBehaviour
{
    public float snapDistance = 0.5f; // 그리드 스냅 크기
    private LayerMask placementMask; // 진열대를 배치할 수 있는 레이어
    private LayerMask allOtherLayersMask; // 'Placing' 레이어를 제외한 모든 레이어
    public float placementHeightOffset = 0.1f; // 진열대가 바닥에서 약간 위로 떠 있게 할 오프셋
    public float collisionCheckRadius = 0.5f; // 충돌 체크 반경
    public float rotationSpeed = 90f; // 회전 속도 (도/초)

    private bool isPlacing = true;
    private Material originalMaterial;
    private Renderer shelfRenderer;
    private Material redMaterial;
    private Material greenMaterial;

    void Awake()
    {
        placementMask = LayerMask.GetMask("Placing");
        allOtherLayersMask = ~placementMask; // 'Placing' 레이어를 제외한 모든 레이어

        // 진열대의 메테리얼을 저장하고, 빨간색 및 초록색 메테리얼을 준비
        shelfRenderer = GetComponentInChildren<Renderer>();
        if (shelfRenderer != null)
        {
            originalMaterial = shelfRenderer.material;
            redMaterial = new Material(originalMaterial);
            redMaterial.color = Color.red;
            greenMaterial = new Material(originalMaterial);
            greenMaterial.color = Color.green;
        }
    }

    void Update()
    {
        if (isPlacing)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, placementMask))
            {
                Vector3 snapPosition = new Vector3(
                    Mathf.Round(hit.point.x / snapDistance) * snapDistance,
                    hit.point.y + placementHeightOffset,
                    Mathf.Round(hit.point.z / snapDistance) * snapDistance
                );

                transform.position = snapPosition;

                // 회전 처리
                if (Input.GetKey(KeyCode.Q))
                {
                    transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime); // 좌회전
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // 우회전
                }

                if (shelfRenderer != null)
                {
                    // 다른 레이어와의 충돌 체크
                    if (!Physics.CheckSphere(snapPosition, collisionCheckRadius, allOtherLayersMask))
                    {
                        shelfRenderer.material = greenMaterial; // 배치 가능할 때 초록색

                        if (Input.GetMouseButtonDown(0)) // 마우스 클릭으로 배치
                        {
                            PlaceShelf(hit);
                        }
                    }
                    else
                    {
                        shelfRenderer.material = redMaterial; // 다른 레이어와 충돌 시 빨간색
                    }
                }
            }
            else
            {
                // 배치할 수 없는 경우
                transform.position = Vector3.zero; // 보이지 않는 위치로 이동
                if (shelfRenderer != null)
                {
                    shelfRenderer.material = originalMaterial; // 원래 색으로
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) // 취소
            {
                CancelPlacement();
            }
        }
    }

    void PlaceShelf(RaycastHit hit)
    {
        // 'Placing' 레이어에 닿았고 다른 레이어와 충돌하지 않을 때만 배치
        if (!Physics.CheckSphere(transform.position, collisionCheckRadius, allOtherLayersMask))
        {
            isPlacing = false;
            transform.parent = null; // 플레이어 손에서 분리
            transform.position = new Vector3(transform.position.x, hit.point.y + placementHeightOffset, transform.position.z);
            if (shelfRenderer != null)
            {
                shelfRenderer.material = originalMaterial; // 배치 후에도 원래 색으로 복구
            }
            Destroy(this); // 이 스크립트 제거
        }
        else
        {
            Debug.Log("Cannot place shelf here due to collision!");
        }
    }

    void CancelPlacement()
    {
        isPlacing = false;
        Shelf shelf = gameObject.GetComponentInChildren<Shelf>();
        if (shelf != null)
        {
            GameManager.Instance.AddMoney(shelf.Price); // 환불
        }
        else
        {
            Debug.LogError("Shelf component not found for refund!");
        }
        Destroy(gameObject); // 진열대 오브젝트 제거
    }
}