// CubeClickHandler.cs
using UnityEngine;

public class CubeClickHandler : MonoBehaviour
{
    private string itemId = "Cube";  // 아이템의 고유 ID를 설정 (퀘스트와 일치하도록)

    private void OnMouseDown()
    {
        // QuestManager 인스턴스를 가져와서 OnItemClicked 호출
        QuestManager questManager = FindObjectOfType<QuestManager>();
        if (questManager != null)
        {
            questManager.OnItemClicked(itemId);
        }
    }
}
