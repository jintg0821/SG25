// QuestData.cs
using MyGame.GuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestData", menuName = "ScriptableObjects/QuestData")]
public class QuestData : ScriptableObject
{
    public string ID;
    public string Title;
    public string Description;
    public QuestType QuestType;
    public QuestStatus QuestStatus;
    public int Day;

    public string targetItemId;  // 클릭해야 하는 아이템 ID
    public int requiredClicks;   // 클릭해야 하는 횟수
}
