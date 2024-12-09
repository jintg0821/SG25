using MyGame.QuestSystem;
using System;
using UnityEngine;

public class CalculateQuestCondition : IQuestCondition
{
    private int requiredAmount;
    private int currentrequiredAmount;
    private bool isCompleted;

    public CalculateQuestCondition(int requiredAmount)
    {
        this.requiredAmount = requiredAmount;
        this.currentrequiredAmount = 0;
        this.isCompleted = false;
    }

    // ������ ������ ������ ȣ��Ǵ� �޼���
    public void Calculate()
    {
        if (!isCompleted)
        {
            currentrequiredAmount++;
            Debug.Log($"���� ����� Ƚ�� {currentrequiredAmount}");
            if (currentrequiredAmount >= requiredAmount)
            {
                isCompleted = true;
                Debug.Log(isCompleted);
            }
        }
    }

    public bool IsMet() => currentrequiredAmount >= requiredAmount;

    public void Initialize()
    {
        currentrequiredAmount = 0;
        isCompleted = false;
    }

    public float GetProgress()
    {
        return (float)currentrequiredAmount / requiredAmount;
    }

    public string GetDescription()
    {
        return $"Calculate {requiredAmount} times. Progress: {currentrequiredAmount}/{requiredAmount}";
    }
}