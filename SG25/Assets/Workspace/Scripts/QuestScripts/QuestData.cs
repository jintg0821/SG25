using MyGame.QuestSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestData", menuName = "ScriptableObjects/QuestData")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string questTitle;
    public string questDescription;
    public QuestType questType;
    public PRODUCTTYPE productType;
    public int targetItemId;
    public int targetItemType;
    public int requiredAmount;
    public int rewardAmount; // 보상으로 지급할 경험치나 아이템 수량
}
