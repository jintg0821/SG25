using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.QuestSystem
{
    public class CollectionQuestCondition : IQuestCondition //아이템을 수집하는 퀘스트 조건을 정의 하는 클래스
    {
        private int itemid;      //수집해야 할 아이템 ID
        private int itemType;      //수집해야 할 아이템 ID
        private int requiredAmount; //수집해야 할 아이템 개수
        private int currentAmount;  //현재까지 수집한 아이템 개수

        public CollectionQuestCondition(int itemid, int requiredAmount)   //생성자에서 아이템 ID와 필요한 개수를 설정
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

