using UnityEngine;
using System.Collections.Generic;

public class ShelfPlacement : MonoBehaviour
{
    public GameObject selectedShelf; // ���� ���õ� ������
    public LayerMask placementLayer; // ��ġ ������ ������ ���̾�
    public float gridSize = 1f;      // �׸��� ����

    private bool isPlacing = false;  // ���� ������ ��ġ ������ ����

    void Update()
    {
        HandleInput(); // �Է� ó��
        if (isPlacing)
        {
            MoveShelfToMousePosition(); // ���콺 ��ġ�� ���� �̵�
        }
    }

    // 1. ���콺 Ŭ�� �Է� ó��
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) // ���� Ŭ��
        {
            if (isPlacing)
            {
                // ��ġ �Ϸ�
                isPlacing = false;
            }
            else
            {
                // ������ ���� (Raycast�� ����)
                SelectShelf();
            }
        }
    }

    // 2. ������ ����
    private void SelectShelf()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.CompareTag("Shelf")) // "Shelf" �±� Ȯ��
            {
                selectedShelf = hitInfo.collider.gameObject;
                isPlacing = true; // ���� �� ��ġ ��� Ȱ��ȭ
            }
        }
    }

    // 3. ���콺 ��ġ�� ���� �����븦 �̵� (�׸��� ���� ����)
    private void MoveShelfToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, placementLayer))
        {
            // ��ġ ������ ���������� �̵�
            Vector3 hitPosition = hitInfo.point;

            // �׸��� ���� ����
            Vector3 snappedPosition = SnapToGrid(hitPosition);
            selectedShelf.transform.position = snappedPosition;
        }
    }

    // 4. �׸��� ���� �Լ�
    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, y, z);
    }
}
