using UnityEngine;
using System.Collections.Generic;

public class ShelfPlacement : MonoBehaviour
{
    public GameObject shelfPrefab; // ���� ������
    private GameObject currentShelf; // ���� ��ġ ���� ����
    private List<GameObject> placedShelves = new List<GameObject>(); // ��ġ�� ���� ���
    private bool isPlacing = false;  // ��ġ ��� Ȱ��ȭ ����
    private float rotationSpeed = 90f; // ȸ�� �ӵ� (�� ����)

    void Start()
    {
        // ���� ���� �� ��ġ ��� Ȱ��ȭ
        StartPlacement();
    }

    void Update()
    {
        // ��ġ ��� Ȱ��ȭ �� ��ġ ���� ����
        if (isPlacing)
        {
            HandlePlacement();
        }
        // ��ġ ��尡 �ƴ� ���, ���� ���� ����
        else if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }

    // ��ġ ��� ����
    private void StartPlacement()
    {
        Debug.Log("Starting placement mode.");
        currentShelf = Instantiate(shelfPrefab); // ���� ������ ����
        isPlacing = true; // ��ġ ��� Ȱ��ȭ
    }

    // ��ġ ����
    private void HandlePlacement()
    {
        // ���콺 ��ġ�� ���� ���� �̵�
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // ��ġ ������ ������ �̵� ����
            if (hitInfo.collider.CompareTag("PlacementArea"))
            {
                Vector3 snappedPosition = SnapToGrid(hitInfo.point);
                currentShelf.transform.position = snappedPosition;
            }
        }

        // ���콺 ���� Ŭ������ ��ġ Ȯ��
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse left-click detected. Confirming placement.");
            ConfirmPlacement();
        }

        // ���콺 ������ Ŭ������ ��ġ ���
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Mouse right-click detected. Cancelling placement.");
            CancelPlacement();
        }

        // Q/E Ű �Է����� ���� ȸ��
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

    // ��ġ Ȯ��
    private void ConfirmPlacement()
    {
        if (currentShelf != null)
        {
            Debug.Log("Placement confirmed. Shelf placed at: " + currentShelf.transform.position);
            placedShelves.Add(currentShelf); // ��ġ�� ���� ��Ͽ� �߰�
            currentShelf = null; // ���� ��ġ ���� ���� �ʱ�ȭ
            isPlacing = false;   // ��ġ ��� ��Ȱ��ȭ
        }
        else
        {
            Debug.LogWarning("No shelf to confirm placement.");
        }
    }

    // ��ġ ���
    private void CancelPlacement()
    {
        if (currentShelf != null)
        {
            Debug.Log("Placement cancelled. Deleting current shelf.");
            Destroy(currentShelf); // ��ġ ���̴� ���� ����
            currentShelf = null;   // ���� ���� �ʱ�ȭ
            isPlacing = false;     // ��ġ ��� ��Ȱ��ȭ
        }
        else
        {
            Debug.LogWarning("No shelf to cancel placement.");
        }
    }

    // ��ġ�� ���� ����
    private void HandleSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            // ��ġ�� ���� Ŭ�� ����
            if (placedShelves.Contains(hitInfo.collider.gameObject))
            {
                Debug.Log("Shelf selected for re-placement: " + hitInfo.collider.gameObject.name);
                currentShelf = hitInfo.collider.gameObject; // Ŭ���� ������ ���ġ ������� ����
                placedShelves.Remove(currentShelf); // ��Ͽ��� ����
                isPlacing = true; // ��ġ ��� Ȱ��ȭ
            }
        }
    }

    // �׸��� ���� �Լ�
    private Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 1f; // ���� ũ�� (1 ����)
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, position.y, z); // y ��ǥ�� �״�� ����
    }
}
