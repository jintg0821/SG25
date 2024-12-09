// ClickQuestCondition.cs
using MyGame.QuestSystem;
using System;

public class ClickQuestCondition : IQuestCondition
{
    private int targetItemId;
    private int requiredClicks;
    private int currentClicks;
    private bool isCompleted;

    public ClickQuestCondition(int targetItemId, int requiredClicks)
    {
        this.targetItemId = targetItemId;
        this.requiredClicks = requiredClicks;
        this.currentClicks = 0;
        this.isCompleted = false;
    }

    // Ŭ�� �� ȣ��Ǵ� �޼���
    public void ItemClicked(int itemId)
    {
        if (itemId == targetItemId && !isCompleted)
        {
            currentClicks++;
            if (currentClicks >= requiredClicks)
            {
                isCompleted = true;
            }
        }
    }

    // IQuestCondition �������̽� ����
    public bool IsMet()
    {
        return isCompleted;
    }

    public void Initialize()
    {
        currentClicks = 0;
        isCompleted = false;
    }

    public float GetProgress()
    {
        return (float)currentClicks / requiredClicks;
    }

    public string GetDescription()
    {
        return $"Click {targetItemId} {requiredClicks} times. Progress: {currentClicks}/{requiredClicks}";
    }
}
