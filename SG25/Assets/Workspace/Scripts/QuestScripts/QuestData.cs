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
    public int rewardAmount; // �������� ������ ����ġ�� ������ ����
}
