using MyGame.QuestSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame.QuestSystem
{
    public class Quest
    {
        // ����Ʈ�� �⺻ ����
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestType Type { get; set; }
        public QuestStatus Status { get; set; }
        public PRODUCTTYPE ProductType { get; set; }
        //public int Day { get; set; } // Ư�� ����Ʈ �䱸��

        private List<IQuestCondition> conditions; // ����Ʈ �Ϸ� ���� ���
        private List<IQuestReward> rewards;       // ����Ʈ ���� ���

        // ������: ����Ʈ�� �⺻ ������ �ʱ�ȭ
        public Quest(string id, string title, string description, QuestType type, PRODUCTTYPE productType)//, int day)
        {
            Id = id;
            Title = title;
            Description = description;
            Type = type;
            Status = QuestStatus.NotStarted; // �ʱ� ����: ���۵��� ����
            ProductType = productType;
            //Day = day;

            conditions = new List<IQuestCondition>();
            rewards = new List<IQuestReward>();
            ProductType = productType;
        }

        // ���� �� ���� �߰� �޼���
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
            return rewards; // ����Ʈ�� �߰��� ��� ���� ��ȯ
        }

        // ����Ʈ ����
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

        // ����Ʈ �Ϸ� ���� Ȯ��
        public bool CheckCompletion()
        {
            if (Status != QuestStatus.InProgress) return false;

            return conditions.All(c => c.IsMet());
        }

        public void Complete(GameObject player)                 //����Ʈ�� �Ϸ��ϰ� ������ �����ϴ� �޼���
        {
            if (Status != QuestStatus.InProgress) return;
            if (!CheckCompletion()) return;

            foreach (var reward in rewards)
            {
                reward.Grant(player);
            }

            Status = QuestStatus.Completed;
        }

        // ����Ʈ ���൵ ��ȯ
        public float GetProgress()
        {
            if (conditions.Count == 0) return 0;
            return conditions.Average(c => c.GetProgress());
        }

        // ��� ���� ���� ��ȯ
        public List<string> GetConditionDescriptions()
        {
            return conditions.Select(c => c.GetDescription()).ToList();
        }

        // ��� ���� ���� ��ȯ
        public List<string> GetRewardDescriptions()
        {
            return rewards.Select(r => r.GetDescription()).ToList();
        }
    }
}
