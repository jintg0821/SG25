using MyGame.GuestSystem;
using System.Collections.Generic;
using System.Linq;

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
        public int Day { get; set; } // Ư�� ����Ʈ �䱸��

        private List<IQuestCondition> conditions; // ����Ʈ �Ϸ� ���� ���
        private List<IQuestReward> rewards;       // ����Ʈ ���� ���

        // ������: ����Ʈ�� �⺻ ������ �ʱ�ȭ
        public Quest(string id, string title, string description, QuestType type, int day)
        {
            Id = id;
            Title = title;
            Description = description;
            Type = type;
            Status = QuestStatus.NotStarted; // �ʱ� ����: ���۵��� ����
            Day = day;

            conditions = new List<IQuestCondition>();
            rewards = new List<IQuestReward>();
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
