using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.QuestSystem
{
    public interface IQuestCondition
    {
        bool IsMet();

        void Initialize();

        float GetProgress();

        string GetDescription();

    }
}
