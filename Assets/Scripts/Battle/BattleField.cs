using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    public static BattleField instance;
    [SerializeField] public float BattleTime;
    [SerializeField] public GameObject Timer;
    [SerializeField] public Tp MyTp;
    [SerializeField] public Tp EnemyTp;
    public float RestTime;
    public int OndeadNum;
    public int MyIsdeadNum;
    public int EnemyIsdeadNum;


    void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        RestTime = BattleTime;
        Timer.SetActive(true);
    }

    void Start()
    {
        EnemyIsdeadNum = MyIsdeadNum = OndeadNum = 0;
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
        if (RestTime > 1e-4)
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
        }
        
        RestTime-=Time.deltaTime;
        if (RestTime < 1e-4 && OndeadNum ==0)
        {
            for (int i = 0; i < BattleSystem.instance.MyBattleSlots.Length; i++)
            {
                for (int j = 0; j < BattleSystem.instance.MyBattleSlots[i].Length; j++)
                {
                    BattleSystem.instance.MyBattleSlots[i][j].StopBattle();
                    BattleSystem.instance.EnemyBattleSlots[i][j].StopBattle();
                }
            }

            BattleSystem.instance.MyBattleCost -= MyIsdeadNum;
            BattleSystem.instance.EnemyBattleCost -= EnemyIsdeadNum;
            if (BattleSystem.instance.EnemyBattleCost <= 0)
            {
                Debug.Log("Win!!");
                return;
            }
            else if (BattleSystem.instance.MyBattleCost <= 0)
            {
                Debug.Log("GameOver!!");
                return;
            }
            else
            {
                BattleSystem.instance.BattleOver();
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnDisable()
    {
        Timer.SetActive(false);
    }

}
