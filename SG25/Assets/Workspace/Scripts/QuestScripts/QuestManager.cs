// QuestManager.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using MyGame.GuestSystem;
using MyGame.QuestSystem;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> allQuests = new Dictionary<string, Quest>();  // ��� ����Ʈ
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();  // ���� ���� ����Ʈ
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();  // �Ϸ�� ����Ʈ

    public event Action<Quest> OnQuestStarted;
    public event Action<Quest> OnQuestCompleted;

    private void Start()
    {
        InitializeQuests();
    }

    private void InitializeQuests()
    {
        //�̰� ����Ʈ�� ��� �����̴�
        // Ŭ�� ����Ʈ ���� �߰�
        var clickQuest = new Quest("Q003", "Cube Click Quest", "Click the cube 3 times", QuestType.Click, 1);
        clickQuest.AddCondition(new ClickQuestCondition(0, 3));  // "Cube"�� CubeClickHandler�� itemId�� ��ġ

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
        Debug.Log("����Ʈ ����");
    }

    public void OnItemClicked(int itemId)
    {
        // activeQuests.Values�� List�� �����Ͽ� �ݺ������� �����ϰ� ����� �� �ְ� �մϴ�.
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

        Debug.Log($"Quest completed: {quest.Title}");  // ����Ʈ �Ϸ� �޽��� ���
    }

}
