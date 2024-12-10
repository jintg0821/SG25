using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.QuestSystem
{
    public enum QuestStatus     //����Ʈ�� ���� ���¸� ��Ÿ���� ������
    {
        NotStarted,             //����Ʈ�� ���� ���۵��� ���� ����
        InProgress,             //����Ʈ�� ���� ���� ���� ����
        Completed,              //����Ʈ�� �Ϸ�� ����
        Failed                  //����Ʈ�� ������ ����
    }

    public enum QuestType    //����Ʈ�� ������ �����ϴ� ������
    {
        Click,                // Ŭ�� �ߴ��� Ȯ���ϴ� ����Ʈ
        ShelfStocking,
        Calculate
    }
}