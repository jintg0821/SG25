using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoneyData", menuName = "ScriptableObjects/MoneyModel")]
public class MoneyData : ScriptableObject
{
    public int value;
    public GameObject moneyModel;
}
