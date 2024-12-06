using MyGame.QuestSystem;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    [Header("Quest Data")]
    [SerializeField] public QuestData[] questDataList;  // ScriptableObject�� �����Ǵ� ����Ʈ ������ ����Ʈ

    private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();  // ��� ����Ʈ
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();  // ���� ���� ����Ʈ
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();  // �Ϸ�� ����Ʈ

    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;
    public event Action OnQuestListUpdated; // ����Ʈ ��� ���� �̺�Ʈ

    private void Start()
    {
        InitializeQuests();
        questDataList = Resources.LoadAll<QuestData>("Quests");
    }

    private void InitializeQuests()
    {
        // ScriptableObject �����͸� ������� ����Ʈ ����
        foreach (var questData in questDataList)
        {
            var quest = new Quest(questData.questId, questData.questTitle, questData.questDescription, questData.questType, 1);
            quest.AddCondition(new ClickQuestCondition(questData.targetItemId, questData.requiredAmount));
            allQuests.Add(quest.Id, quest);
        }
    }

    public void QuestAcceptance(QuestData quest)
    {
        StartQuest(quest.questId);
    }

    public bool CanStartQuest(string questId)
    {
        return allQuests.ContainsKey(questId) && !activeQuests.ContainsKey(questId) && !completedQuests.ContainsKey(questId);
    }

    public void StartQuest(string questId)
    {
        if (!CanStartQuest(questId)) return;

        var quest = allQuests[questId];
        quest.Start();
        activeQuests.Add(questId, quest);
        OnQuestStarted?.Invoke(quest);
        Debug.Log($"����Ʈ ����: {quest.Title}");
    }

    public void OnItemClicked(int itemId)
    {
        var activeQuestsList = activeQuests.Values.ToList();

        foreach (var quest in activeQuestsList)
        {
            foreach (var condition in quest.GetConditions())
            {
                if (condition is ClickQuestCondition clickCondition)
                {
                    clickCondition.ItemClicked(itemId);
                    UpdateQuestProgress(quest.Id);
                }
            }
        }
    }

    public void ShelfStock(int itemId)
    {
        var activeQuestsList = activeQuests.Values.ToList();

        foreach (var quest in activeQuestsList)
        {
            foreach (var condition in quest.GetConditions())
            {
                if (condition is CShelfStockQuestCondition clickCondition)
                {
                    clickCondition.ShelfStock(itemId);
                    UpdateQuestProgress(quest.Id);
                }
            }
        }
    }

    public void UpdateQuestProgress(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out Quest quest)) return;

        if (quest.CheckCompletion())
        {
            CompleteQuest(questId);
        }
    }

    public void CompleteQuest(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out Quest quest)) return;

        activeQuests.Remove(questId);
        completedQuests.Add(questId, quest);

        Debug.Log($"����Ʈ �Ϸ�: {quest.Title}");

        OnQuestListUpdated?.Invoke(); // UI ���� �̺�Ʈ ȣ��
    }

    private void GrantRewards(Quest quest)
    {
        foreach (var reward in quest.GetRewards()) // ����Ʈ�� ���� ��� �ݺ�
        {
            reward.Grant(null); // ������ ���� (�÷��̾� ������Ʈ ���� ����)
            Debug.Log($"���� ����: {reward.GetDescription()}");
        }
    }
}
