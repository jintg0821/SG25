using UnityEngine;
using System.Collections.Generic;

public class ShelfPlacement : MonoBehaviour
{
    public GameObject shelfPrefab; // 선반 프리팹
    private GameObject currentShelf; // 현재 배치 중인 선반
    private List<GameObject> placedShelves = new List<GameObject>(); // 배치된 선반 목록
    private bool isPlacing = false;  // 배치 모드 활성화 여부
    private float rotationSpeed = 90f; // 회전 속도 (도 단위)

    void Start()
    {
        // 게임 시작 시 배치 모드 활성화
        StartPlacement();
    }

    void Update()
    {
        // 배치 모드 활성화 시 배치 로직 실행
        if (isPlacing)
        {
            HandlePlacement();
        }
        // 배치 모드가 아닌 경우, 선택 로직 실행
        else if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }

    // 배치 모드 시작
    private void StartPlacement()
    {
        Debug.Log("Starting placement mode.");
        currentShelf = Instantiate(shelfPrefab); // 선반 프리팹 생성
        isPlacing = true; // 배치 모드 활성화
    }

    // 배치 로직
    private void HandlePlacement()
    {
        // 마우스 위치에 따라 선반 이동
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // 배치 가능한 영역만 이동 가능
            if (hitInfo.collider.CompareTag("PlacementArea"))
            {
                Vector3 snappedPosition = SnapToGrid(hitInfo.point);
                currentShelf.transform.position = snappedPosition;
            }
        }

        // 마우스 왼쪽 클릭으로 배치 확정
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse left-click detected. Confirming placement.");
            ConfirmPlacement();
        }

        // 마우스 오른쪽 클릭으로 배치 취소
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Mouse right-click detected. Cancelling placement.");
            CancelPlacement();
        }

        // Q/E 키 입력으로 선반 회전
        if (Input.GetKey(KeyCode.Q))
        {
            currentShelf.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            Debug.Log("Rotating shelf left.");
        }

        if (Input.GetKey(KeyCode.E))
        {
            currentShelf.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            Debug.Log("Rotating shelf right.");
        }
    }

    // 배치 확정
    private void ConfirmPlacement()
    {
        if (currentShelf != null)
        {
            Debug.Log("Placement confirmed. Shelf placed at: " + currentShelf.transform.position);
            placedShelves.Add(currentShelf); // 배치된 선반 목록에 추가
            currentShelf = null; // 현재 배치 중인 선반 초기화
            isPlacing = false;   // 배치 모드 비활성화
        }
        else
        {
            Debug.LogWarning("No shelf to confirm placement.");
        }
    }

    // 배치 취소
    private void CancelPlacement()
    {
        if (currentShelf != null)
        {
            Debug.Log("Placement cancelled. Deleting current shelf.");
            Destroy(currentShelf); // 배치 중이던 선반 제거
            currentShelf = null;   // 현재 선반 초기화
            isPlacing = false;     // 배치 모드 비활성화
        }
        else
        {
            Debug.LogWarning("No shelf to cancel placement.");
        }
    }

    // 배치된 선반 선택
    private void HandleSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // 배치된 선반 클릭 감지
            if (placedShelves.Contains(hitInfo.collider.gameObject))
            {
                Debug.Log("Shelf selected for re-placement: " + hitInfo.collider.gameObject.name);
                currentShelf = hitInfo.collider.gameObject; // 클릭한 선반을 재배치 대상으로 설정
                placedShelves.Remove(currentShelf); // 목록에서 제거
                isPlacing = true; // 배치 모드 활성화
            }
        }
    }

    // 그리드 스냅 함수
    private Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 1f; // 격자 크기 (1 단위)
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, position.y, z); // y 좌표는 그대로 유지
    }
}
