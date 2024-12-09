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
            // GameManager를 통해 플레이어에게 돈 지급
            GameManager.Instance.AddMoney(moneyAmount, null);
            Debug.Log($"게임머니 {moneyAmount} 지급!");
        }

        public string GetDescription()
        {
            return $"{moneyAmount} 게임머니";
        }
    }
}
