using MyGame.GuestSystem;
using System;

public class CShelfStockQuestCondition : IQuestCondition
{
    private int targetItemId;
    private int requiredClicks;
    private int currentShelfStock;
    private bool isCompleted;

    public CShelfStockQuestCondition(int targetItemId, int requiredClicks)
    {
        this.targetItemId = targetItemId;
        this.requiredClicks = requiredClicks;
        this.currentShelfStock = 0;
        this.isCompleted = false;
    }

    // 진열대 아이템 넣을때 호출되는 메서드
    public void ShelfStock(int itemId)
    {
        if (itemId == targetItemId && !isCompleted)
        {
            currentShelfStock++;
            if (currentShelfStock >= requiredClicks)
            {
                isCompleted = true;
            }
        }
    }

    // IQuestCondition 인터페이스 구현
    public bool IsMet()
    {
        return isCompleted;
    }

    public void Initialize()
    {
        currentShelfStock = 0;
        isCompleted = false;
    }

    public float GetProgress()
    {
        return (float)currentShelfStock / requiredClicks;
    }

    public string GetDescription()
    {
        return $"Click {targetItemId} {requiredClicks} times. Progress: {currentShelfStock}/{requiredClicks}";
    }
}