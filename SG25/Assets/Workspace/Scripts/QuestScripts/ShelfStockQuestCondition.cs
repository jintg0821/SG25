using MyGame.QuestSystem;
using System;
using UnityEngine;

public class ShelfStockQuestCondition : IQuestCondition
{
    private int targetItemId;
    private int targetItemType;
    private int requiredStocks;
    private int currentShelfStock;
    private bool isCompleted;

    public ShelfStockQuestCondition(int targetItemId, int targetItemType ,int requiredClicks)
    {
        this.targetItemId = targetItemId;
        this.targetItemType = targetItemType;
        this.requiredStocks = requiredClicks;
        this.currentShelfStock = 0;
        this.isCompleted = false;
    }

    // ������ ������ ������ ȣ��Ǵ� �޼���
    public void ItemShelfStock(int itemId)
    {
        if (itemId == targetItemId && !isCompleted)
        {
            currentShelfStock++;
            Debug.Log($"���� ���� ���� {currentShelfStock}");
            if (currentShelfStock >= requiredStocks)
            {
                isCompleted = true;
                Debug.Log(isCompleted);
            }
        }
    }

    public void ItemTypeShelfStock(int itemType)
    {
        if (itemType == targetItemType && !isCompleted)
        {
            currentShelfStock++;
            if (currentShelfStock >= requiredStocks)
            {
                isCompleted = true;
            }
        }
    }

    public bool IsMet() => currentShelfStock >= requiredStocks;

    public void Initialize()
    {
        currentShelfStock = 0;
        isCompleted = false;
    }

    public float GetProgress()
    {
        return (float)currentShelfStock / requiredStocks;
    }

    public string GetDescription()
    {
        return $"Click {targetItemId} {requiredStocks} times. Progress: {currentShelfStock}/{requiredStocks}";
    }
}