using MyGame.GuestSystem;
using MyGame.QuestSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestData", menuName = "ScriptableObjects/QuestData")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string questTitle;
    public string questDescription;
    public QuestType questType;
    public int targetItemId;
    public int requiredAmount;
    public int rewardAmount; // 보상으로 지급할 경험치나 아이템 수량

    private void InitializeQuests()
    {
        foreach (var questData in questDataList)
        {
            var quest = new Quest(questData.questId, questData.questTitle, questData.questDescription, questData.questType, 1);
            quest.AddCondition(new ClickQuestCondition(questData.targetItemId, questData.requiredAmount));

            // 기존 보상 외에 게임머니 보상 추가
            quest.AddReward(new MoneyReward(questData.rewardAmount));

            allQuests.Add(quest.Id, quest);
        }
    }

}
