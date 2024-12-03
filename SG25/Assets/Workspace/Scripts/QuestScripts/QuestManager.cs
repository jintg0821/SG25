// QuestManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MyGame.GuestSystem;
using MyGame.QuestSystem;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();  // 모든 퀘스트
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();  // 진행 중인 퀘스트
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();  // 완료된 퀘스트

    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;

    private void Start()
    {
        InitializeQuests();
    }

    private void InitializeQuests()
    {
        //이게 리스트에 없어서 추측이다
        // 클릭 퀘스트 예시 추가
        var clickQuest = new Quest("Q003", "Cube Click Quest", "Click the cube 3 times", QuestType.Click, 1);
        clickQuest.AddCondition(new ClickQuestCondition(0, 3));  // "Cube"는 CubeClickHandler의 itemId와 일치

        allQuests.Add(clickQuest.Id, clickQuest);
        StartQuest("Q003");

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
        Debug.Log("퀘스트 시작");
    }

    public void OnItemClicked(int itemId)
    {
        // activeQuests.Values를 List로 복사하여 반복문에서 안전하게 사용할 수 있게 합니다.
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

        Debug.Log($"Quest completed: {quest.Title}");  // 퀘스트 완료 메시지 출력
    }

}
