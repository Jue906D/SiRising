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

    void Start()
    {
        for (int i = 0; i < BattleSystem.instance.MyBattleSlots.Length; i++)
        {
            for (int j = 0; j < BattleSystem.instance.MyBattleSlots[i].Length; j++)
            {
                BattleSystem.instance.MyBattleSlots[i][j].StartBattle();      //×´Ì¬³õÊ¼»¯
                BattleSystem.instance.EnemyBattleSlots[i][j].StartBattle();
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < BattleSystem.instance.MyBattleSlots.Length; i++)
        {
            for (int j = 0; j < BattleSystem.instance.MyBattleSlots[i].Length; j++)
            {
                BattleSystem.instance.MyBattleSlots[i][j].TryAttack();      //ÏÈ¹¥»÷£¬Ö»¼õÑª
                BattleSystem.instance.EnemyBattleSlots[i][j].TryAttack();   //
                
                BattleSystem.instance.MyBattleSlots[i][j].TryRecover();     
                BattleSystem.instance.EnemyBattleSlots[i][j].TryRecover();

                BattleSystem.instance.MyBattleSlots[i][j].TryDeath();
                BattleSystem.instance.EnemyBattleSlots[i][j].TryDeath();

                BattleSystem.instance.MyBattleSlots[i][j].TryMoveForward();        //²¹Î»
                BattleSystem.instance.EnemyBattleSlots[i][j].TryMoveForward();
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
