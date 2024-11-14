using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.GuestSystem
{
    public interface IQuestCondition
    {
        bool IsMet();

        void Initialize();

        float GetProgress();

        string GetDescription();

    }
}
