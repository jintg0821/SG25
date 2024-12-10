using MyGame.QuestSystem;
using UnityEngine;

namespace MyGame.QuestSystem
{
    public class MoneyReward : IQuestReward
    {
        private int moneyAmount;

        public MoneyReward(int amount)
        {
            moneyAmount = amount;
        }

        public void Grant(GameObject player)
        {
            // GameManager�� ���� �÷��̾�� �� ����
            GameManager.Instance.AddMoney(moneyAmount, null);
            Debug.Log($"���ӸӴ� {moneyAmount} ����!");
        }

        public string GetDescription()
        {
            return $"{moneyAmount} ���ӸӴ�";
        }
    }
}
