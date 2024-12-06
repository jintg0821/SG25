using UnityEngine;

namespace MyGame.GuestSystem
{
    public class ExperienceReward : IQuestReward
    {
        private int experienceAmount;

        public ExperienceReward(int amount)
        {
            this.experienceAmount = amount;
        }

        public void Grant(GameObject player)
        {
            // 실제 보상 지급 로직 (여기선 로그로 처리)
            Debug.Log($"경험치 {experienceAmount} 지급!");
            // 예시: player.GetComponent<PlayerStats>().AddExperience(experienceAmount);
        }

        public string GetDescription()
        {
            return $"{experienceAmount} 경험치";
        }
    }
}
