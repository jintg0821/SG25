using UnityEngine;
using System.Collections.Generic;

public class ShelfPlacement : MonoBehaviour
{
    public GameObject selectedShelf; // 현재 선택된 진열대
    public LayerMask placementLayer; // 배치 가능한 영역의 레이어
    public float gridSize = 1f;      // 그리드 간격

    private bool isPlacing = false;  // 현재 진열대 배치 중인지 여부

    void Update()
    {
        HandleInput(); // 입력 처리
        if (isPlacing)
        {
            MoveShelfToMousePosition(); // 마우스 위치에 따라 이동
        }
    }

    // 1. 마우스 클릭 입력 처리
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 클릭
        {
            if (isPlacing)
            {
                // 배치 완료
                isPlacing = false;
            }
            else
            {
                // 진열대 선택 (Raycast로 선택)
                SelectShelf();
            }
        }
    }

    // 2. 진열대 선택
    private void SelectShelf()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.CompareTag("Shelf")) // "Shelf" 태그 확인
            {
                selectedShelf = hitInfo.collider.gameObject;
                isPlacing = true; // 선택 후 배치 모드 활성화
            }
        }
    }

    // 3. 마우스 위치에 따라 진열대를 이동 (그리드 스냅 적용)
    private void MoveShelfToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, placementLayer))
        {
            // 배치 가능한 영역에서만 이동
            Vector3 hitPosition = hitInfo.point;

            // 그리드 스냅 적용
            Vector3 snappedPosition = SnapToGrid(hitPosition);
            selectedShelf.transform.position = snappedPosition;
        }
    }

    // 4. 그리드 스냅 함수
    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, y, z);
    }
}
