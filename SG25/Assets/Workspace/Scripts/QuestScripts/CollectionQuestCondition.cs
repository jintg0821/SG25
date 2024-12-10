using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.QuestSystem
{
    public class CollectionQuestCondition : IQuestCondition //�������� �����ϴ� ����Ʈ ������ ���� �ϴ� Ŭ����
    {
        private int itemid;      //�����ؾ� �� ������ ID
        private int itemType;      //�����ؾ� �� ������ ID
        private int requiredAmount; //�����ؾ� �� ������ ����
        private int currentAmount;  //������� ������ ������ ����

        public CollectionQuestCondition(int itemid, int requiredAmount)   //�����ڿ��� ������ ID�� �ʿ��� ������ ����
        {
            this.itemid = itemid;
            this.requiredAmount = requiredAmount;
            this.currentAmount = 0;
        }

        public bool IsMet() => currentAmount > requiredAmount;
        public void Initialize() => currentAmount = 0;

        public float GetProgress() => (float)currentAmount / requiredAmount;
        public string GetDescription() => $"Defeat {requiredAmount} {itemid} ({currentAmount}/{requiredAmount})";

        public void ItemCollected(int itemid)
        {
            if (this.itemid == itemid)
            {
                currentAmount++;
            }
        }
    }
}

