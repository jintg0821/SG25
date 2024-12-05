using MyGame.GuestSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestData", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questId;          // 퀘스트 ID
    public string questTitle;       // 퀘스트 제목
    [TextArea] public string questDescription; // 퀘스트 설명
    public QuestType questType;     // 퀘스트 유형 (예: Click, Collect 등)
    public int targetItemId;        // 목표 아이템 ID (클릭이나 수집할 대상)
    public int requiredAmount;      // 목표 조건 (예: 클릭 횟수, 수집 개수)
    public int rewardAmount;        // 보상 금액
}
