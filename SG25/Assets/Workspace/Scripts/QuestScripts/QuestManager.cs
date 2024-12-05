using MyGame.QuestSystem;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class QuestManager : MonoBehaviour
{
    [Header("Quest Data")]
    [SerializeField] public QuestData[] questDataList;  // ScriptableObject로 관리되는 퀘스트 데이터 리스트

    private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();  // 모든 퀘스트
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();  // 진행 중인 퀘스트
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();  // 완료된 퀘스트

    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;

    private void Start()
    {
        InitializeQuests();
        questDataList = Resources.LoadAll<QuestData>("Quests");
    }

    private void InitializeQuests()
    {
        // ScriptableObject 데이터를 기반으로 퀘스트 생성
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
        Debug.Log($"퀘스트 시작: {quest.Title}");
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

    private void CompleteQuest(string questId)
    {
        if (!activeQuests.TryGetValue(questId, out Quest quest)) return;

        activeQuests.Remove(questId);
        completedQuests.Add(questId, quest);
        OnQuestCompleted?.Invoke(quest);

        Debug.Log($"퀘스트 완료: {quest.Title}");
    }
}