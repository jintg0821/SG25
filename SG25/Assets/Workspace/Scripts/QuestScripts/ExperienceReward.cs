using UnityEngine;

namespace MyGame.QuestSystem
{
    public class ExperienceReward : IQuestReward
    {
        private int moneyAmount;

        public ExperienceReward(int amount)
        {
            this.moneyAmount = amount;
        }

        public void Grant(GameObject player)
        {
            // ���� ���� ���� ���� (���⼱ �α׷� ó��)
            Debug.Log($"{moneyAmount}�� ����!");
            UIManager.Instance.IncreaseMoneyText(moneyAmount);
            // ����: player.GetComponent<PlayerStats>().AddExperience(experienceAmount);
        }

        public string GetDescription()
        {
            return $"{moneyAmount}��";
        }
    }
}
