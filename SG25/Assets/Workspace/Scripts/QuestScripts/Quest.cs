using MyGame.QuestSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame.QuestSystem
{
    public class Quest
    {
        // 퀘스트의 기본 정보
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestType Type { get; set; }
        public QuestStatus Status { get; set; }
        public PRODUCTTYPE ProductType { get; set; }
        //public int Day { get; set; } // 특정 퀘스트 요구일

        private List<IQuestCondition> conditions; // 퀘스트 완료 조건 목록
        private List<IQuestReward> rewards;       // 퀘스트 보상 목록

        // 생성자: 퀘스트의 기본 정보를 초기화
        public Quest(string id, string title, string description, QuestType type, PRODUCTTYPE productType)//, int day)
        {
            Id = id;
            Title = title;
            Description = description;
            Type = type;
            Status = QuestStatus.NotStarted; // 초기 상태: 시작되지 않음
            ProductType = productType;
            //Day = day;

            conditions = new List<IQuestCondition>();
            rewards = new List<IQuestReward>();
            ProductType = productType;
        }

        // 조건 및 보상 추가 메서드
        public void AddCondition(IQuestCondition condition)
        {
            conditions.Add(condition);
        }

        public void AddReward(IQuestReward reward)
        {
            rewards.Add(reward);
        }

        public List<IQuestCondition> GetConditions()
        {
            return conditions;
        }

        public List<IQuestReward> GetRewards()
        {
            return rewards; // 퀘스트에 추가된 모든 보상 반환
        }

        // 퀘스트 시작
        public void Start()
        {
            if (Status == QuestStatus.NotStarted)
            {
                Status = QuestStatus.InProgress;
                foreach (var condition in conditions)
                {
                    condition.Initialize();
                }
            }
        }

        // 퀘스트 완료 여부 확인
        public bool CheckCompletion()
        {
            if (Status != QuestStatus.InProgress) return false;

            return conditions.All(c => c.IsMet());
        }

        public void Complete(GameObject player)                 //퀘스트를 완료하고 보상을 지급하는 메서드
        {
            if (Status != QuestStatus.InProgress) return;
            if (!CheckCompletion()) return;

            foreach (var reward in rewards)
            {
                reward.Grant(player);
            }

            Status = QuestStatus.Completed;
        }

        // 퀘스트 진행도 반환
        public float GetProgress()
        {
            if (conditions.Count == 0) return 0;
            return conditions.Average(c => c.GetProgress());
        }

        // 모든 조건 설명 반환
        public List<string> GetConditionDescriptions()
        {
            return conditions.Select(c => c.GetDescription()).ToList();
        }

        // 모든 보상 설명 반환
        public List<string> GetRewardDescriptions()
        {
            return rewards.Select(r => r.GetDescription()).ToList();
        }
    }
}
