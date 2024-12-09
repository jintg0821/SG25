using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MyGame.QuestSystem
{
    public interface IQuestReward        //퀘스트 보상을 정의하는 기본 인터페이스
    {
        void Grant(GameObject player);  //플레이어에게 보상을 지급하는 함수
        string GetDescription();        //보상에 대한 설명을 반환하는 함수

    }

}
