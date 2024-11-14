// CubeClickHandler.cs
using UnityEngine;

public class CubeClickHandler : MonoBehaviour
{
    private string itemId = "Cube";  // �������� ���� ID�� ���� (����Ʈ�� ��ġ�ϵ���)

    private void OnMouseDown()
    {
        // QuestManager �ν��Ͻ��� �����ͼ� OnItemClicked ȣ��
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.OnItemClicked(itemId);
        }
    }
}
