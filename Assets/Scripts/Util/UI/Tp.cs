using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tp : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Count;
    [SerializeField] public int CurTp;
    [SerializeField] public bool isMine;
    [SerializeField] public bool isTactic;

    void Start()
    {
        CurTp = isMine ? BattleSystem.instance.MyBattleCost : BattleSystem.instance.EnemyBattleCost;
    }
    
    void Update()
    {
        if (isTactic)
        {
            if (isMine)
            {
                CurTp = BattleSystem.instance.MyBattleCost;
            }
            else
            {
                CurTp = BattleSystem.instance.EnemyBattleCost;
            }
        }
        /*
        if (isMine)
        {
            CurTp = BattleSystem.instance.MyBattleCost;
        }
        else
        {
            CurTp = BattleSystem.instance.EnemyBattleCost;
        }
        */
        Count.text = (CurTp).ToString();
    }
}
