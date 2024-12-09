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

    // 진열대 아이템 넣을때 호출되는 메서드
    public void Calculate()
    {
        if (!isCompleted)
        {
            currentrequiredAmount++;
            Debug.Log($"현재 계산한 횟수 {currentrequiredAmount}");
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