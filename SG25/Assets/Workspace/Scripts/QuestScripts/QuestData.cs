using MyGame.GuestSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestData", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questId;          // ����Ʈ ID
    public string questTitle;       // ����Ʈ ����
    [TextArea] public string questDescription; // ����Ʈ ����
    public QuestType questType;     // ����Ʈ ���� (��: Click, Collect ��)
    public int targetItemId;        // ��ǥ ������ ID (Ŭ���̳� ������ ���)
    public int requiredAmount;      // ��ǥ ���� (��: Ŭ�� Ƚ��, ���� ����)
    public int rewardAmount;        // ���� �ݾ�
}
