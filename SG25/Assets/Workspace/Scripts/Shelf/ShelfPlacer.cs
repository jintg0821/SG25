using UnityEngine;

public class ShelfPlacer : MonoBehaviour
{
    public float snapDistance = 0.5f; // �׸��� ���� ũ��
    private LayerMask placementMask; // �����븦 ��ġ�� �� �ִ� ���̾�
    private LayerMask allOtherLayersMask; // 'Placing' ���̾ ������ ��� ���̾�
    public float placementHeightOffset = 0.1f; // �����밡 �ٴڿ��� �ణ ���� �� �ְ� �� ������
    public float collisionCheckRadius = 0.5f; // �浹 üũ �ݰ�
    public float rotationSpeed = 90f; // ȸ�� �ӵ� (��/��)

    private bool isPlacing = true;
    private Material originalMaterial;
    private Renderer shelfRenderer;
    private Material redMaterial;
    private Material greenMaterial;

    void Awake()
    {
        placementMask = LayerMask.GetMask("Placing");
        allOtherLayersMask = ~placementMask; // 'Placing' ���̾ ������ ��� ���̾�

        // �������� ���׸����� �����ϰ�, ������ �� �ʷϻ� ���׸����� �غ�
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

                // ȸ�� ó��
                if (Input.GetKey(KeyCode.Q))
                {
                    transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime); // ��ȸ��
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // ��ȸ��
                }

                if (shelfRenderer != null)
                {
                    // �ٸ� ���̾���� �浹 üũ
                    if (!Physics.CheckSphere(snapPosition, collisionCheckRadius, allOtherLayersMask))
                    {
                        shelfRenderer.material = greenMaterial; // ��ġ ������ �� �ʷϻ�

                        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ������ ��ġ
                        {
                            PlaceShelf(hit);
                        }
                    }
                    else
                    {
                        shelfRenderer.material = redMaterial; // �ٸ� ���̾�� �浹 �� ������
                    }
                }
            }
            else
            {
                // ��ġ�� �� ���� ���
                transform.position = Vector3.zero; // ������ �ʴ� ��ġ�� �̵�
                if (shelfRenderer != null)
                {
                    shelfRenderer.material = originalMaterial; // ���� ������
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) // ���
            {
                CancelPlacement();
            }
        }
    }

    void PlaceShelf(RaycastHit hit)
    {
        // 'Placing' ���̾ ��Ұ� �ٸ� ���̾�� �浹���� ���� ���� ��ġ
        if (!Physics.CheckSphere(transform.position, collisionCheckRadius, allOtherLayersMask))
        {
            isPlacing = false;
            transform.parent = null; // �÷��̾� �տ��� �и�
            transform.position = new Vector3(transform.position.x, hit.point.y + placementHeightOffset, transform.position.z);
            if (shelfRenderer != null)
            {
                shelfRenderer.material = originalMaterial; // ��ġ �Ŀ��� ���� ������ ����
            }
            Destroy(this); // �� ��ũ��Ʈ ����
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
            GameManager.Instance.AddMoney(shelf.Price); // ȯ��
        }
        else
        {
            Debug.LogError("Shelf component not found for refund!");
        }
        Destroy(gameObject); // ������ ������Ʈ ����
    }
}