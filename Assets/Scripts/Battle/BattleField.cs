using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    [SerializeField] public float BattleTime;
    public float RestTime;

    void OnEnable()
    {
        RestTime = BattleTime;
    }

    void Update()
    {
        for (int i = 0; i < BattleSystem.instance.MyBattleSlots.Length; i++)
        {
            for (int j = 0; j < BattleSystem.instance.MyBattleSlots[i].Length; j++)
            {
                BattleSystem.instance.MyBattleSlots[i][j].TryAttack();
                BattleSystem.instance.EnemyBattleSlots[i][j].TryAttack();
                BattleSystem.instance.MyBattleSlots[i][j].TryRecover();
                BattleSystem.instance.EnemyBattleSlots[i][j].TryRecover();
            }
        }
        RestTime-=Time.deltaTime;
        if (RestTime < 1e-4)
        {
            for (int i = 0; i < BattleSystem.instance.MyBattleSlots.Length; i++)
            {
                for (int j = 0; j < BattleSystem.instance.MyBattleSlots[i].Length; j++)
                {
                    BattleSystem.instance.MyBattleSlots[i][j].StopBattle();
                    BattleSystem.instance.EnemyBattleSlots[i][j].StopBattle();
                }
            }
            BattleSystem.instance.BattleOver();
            this.gameObject.SetActive(false);
        }
    }

}
