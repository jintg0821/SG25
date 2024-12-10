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
            // 실제 보상 지급 로직 (여기선 로그로 처리)
            Debug.Log($"{moneyAmount}원 지급!");
            UIManager.Instance.IncreaseMoneyText(moneyAmount);
            // 예시: player.GetComponent<PlayerStats>().AddExperience(experienceAmount);
        }

        public string GetDescription()
        {
            return $"{moneyAmount}원";
        }
    }
}
