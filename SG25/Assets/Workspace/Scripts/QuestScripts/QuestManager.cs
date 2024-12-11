using MyGame.QuestSystem;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    [Header("Quest Data")]
    [SerializeField] public QuestData[] questDataList;  // ScriptableObject�� �����Ǵ� ����Ʈ ������ ����Ʈ

    private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();  // ��� ����Ʈ
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();  // ���� ���� ����Ʈ
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();  // �Ϸ�� ����Ʈ

    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;
    public event Action OnQuestListUpdated; // ����Ʈ ��� ���� �̺�Ʈ

    public void Start()
    {
        questDataList = Resources.LoadAll<QuestData>("Quests");
        InitializeQuests();
        Debug.Log($"Ȱ��ȭ �� ����Ʈ {activeQuests.Count}");
    }

    private void InitializeQuests()
    {
        // ScriptableObject �����͸� ������� ����Ʈ ����
        foreach (var questData in questDataList)
        {
            var quest = new Quest(questData.questId, questData.questTitle, questData.questDescription, questData.questType, questData.productType);//, 1);
            switch (questData.questType)
            {
                case QuestType.ShelfStocking:
                    quest.AddCondition(new ShelfStockQuestCondition(questData.targetItemId, questData.targetItemType, questData.requiredAmount));
                    break;
                case QuestType.Calculate:
                    quest.AddCondition(new CalculateQuestCondition(questData.requiredAmount));
                    break;
            }
            quest.AddReward(new ExperienceReward(questData.rewardAmount));
            allQuests.Add(quest.Id, quest);
            Debug.Log($"��ϵ� ����Ʈ {questData.questTitle}");
        }
    }
    public bool CanStartQuest(string questId)
    {
        if (!allQuests.TryGetValue(questId, out Quest quest)) return false;
        if (activeQuests.ContainsKey(questId)) return true;
        if (completedQuests.ContainsKey(questId)) return true;

        // ���� ����Ʈ �Ϸ� ���� Ȯ��
        foreach (var perrequistiteId in quest.GetType().GetField("prerequisiteQuestIds")?.GetValue(quest) as List<string> ?? new List<string>())
        {
            if (!completedQuests.ContainsKey(perrequistiteId)) return false;
        }

        return true;
    }

    public void StartQuest(string questId)
    {
        if (!CanStartQuest(questId)) return;

        var quest = allQuests[questId];
        quest.Start();
        activeQuests.Add(questId, quest);
        OnQuestStarted?.Invoke(quest);
        OnQuestListUpdated?.Invoke();
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
                }
            }
        }
    }

    public void ItemShelfStock(int itemId)
    {
        var activeQuestsList = activeQuests.Values.ToList();

        foreach (var quest in activeQuestsList)
        {
            foreach (var condition in quest.GetConditions())
            {
                if (condition is ShelfStockQuestCondition stockCondition)
                {
                    stockCondition.ItemShelfStock(itemId);
                }
            }
        }
    }

    public void ItemTypeShelfStock(int itemType)
    {
        var activeQuestsList = activeQuests.Values.ToList();

        foreach (var quest in activeQuestsList)
        {
            foreach (var condition in quest.GetConditions())
            {
                if (condition is ShelfStockQuestCondition stockCondition)
                {
                    stockCondition.ItemTypeShelfStock(itemType);
                }
            }
        }
    }

    public void Calculate()
    {
        var activeQuestsList = activeQuests.Values.ToList();

        foreach (var quest in activeQuestsList)
        {
            foreach (var condition in quest.GetConditions())
            {
                if (condition is CalculateQuestCondition calculateCondition)
                {
                    calculateCondition.Calculate();
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

        var player = GameObject.FindGameObjectWithTag("Player");
        quest.Complete(player);
        activeQuests.Remove(questId);
        allQuests.Remove(questId);
        completedQuests.Add(questId, quest);
        OnQuestCompleted?.Invoke(quest);
        OnQuestListUpdated?.Invoke();

        Debug.Log($"����Ʈ �Ϸ�: {quest.Title}");
    }

    public List<Quest> GetAvailableQuests()
    {
        return allQuests.Values.Where(q => CanStartQuest(q.Id)).ToList();
    }

    // ���� ���� ���� ����Ʈ ����� ��ȯ�ϴ� �޼���
    public List<Quest> GetActiveQuest()
    {
        return activeQuests.Values.ToList();
    }

    // �Ϸ�� ����Ʈ ����� ��ȯ�ϴ� �޼���
    public List<Quest> GetCompletedQuest()
    {
        return completedQuests.Values.ToList();
    }
}
