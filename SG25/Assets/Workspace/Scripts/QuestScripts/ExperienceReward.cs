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
            // ���� ���� ���� ���� (���⼱ �α׷� ó��)
            Debug.Log($"����ġ {experienceAmount} ����!");
            // ����: player.GetComponent<PlayerStats>().AddExperience(experienceAmount);
        }

        public string GetDescription()
        {
            return $"{experienceAmount} ����ġ";
        }
    }
}
