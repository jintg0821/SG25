using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.GuestSystem
{
    public class ExperienceReward : IQuestReward    //경험치 보상을 구현하는 클래스
    {
        private int experienceAmount;       //보상을 지급할 경험치량

        public ExperienceReward(int amount)
        {
            this.experienceAmount = amount;
        }

        public void Grant(GameObject player)
        {
            Debug.Log($"Granted {experienceAmount}experience");
        }
        public string GetDescription() => $"{experienceAmount}ExperiencePoints";        //보상 내용을 문자열로 반환
    }


}

