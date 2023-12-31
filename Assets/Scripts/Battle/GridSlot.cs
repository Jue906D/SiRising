using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Authentication;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class GridSlot : MonoBehaviour
{
    [SerializeField]public bool IsTactic;
    [SerializeField] public bool IsMine;
    public GameObject Grid;
    public Vector2Int Location;//i,j

    [SerializeField] public GameObject CharaObj;

    [SerializeField] public CharaData data;

    [SerializeField] public GameObject TacticInfoWindow;

    public Occupation CurOccup;
    //public 
    public bool IsDisplayInfo;
    public static bool IsForcedDislayInfo = false;

    [SerializeField] public int ForwardSign;
    [SerializeField] public int ForwardAffirm;
    [SerializeField] public bool IsDead;


    void Awake()
    {
        IsDisplayInfo = false;
        ForwardSign = 0;
        IsDead = false;
        if (IsTactic)
        {
            data = new CharaData(this);
        }
        else if (IsMine)
        {

        }
    }

    void OnMouseDown()
    {
        if (IsDead || !BattleSystem.instance.IsTacticWindowAvailable)
        {
            return;
        }
        if (IsForcedDislayInfo)
        {
            IsForcedDislayInfo = false;
            DisplayInfo();
        }
        else
        {
            IsForcedDislayInfo = true;
        }
    }

    void OnMouseEnter()
    {
        if (IsDead || !BattleSystem.instance.IsTacticWindowAvailable)
        {
            return;
        }
        DisplayInfo();
    }

    void OnMouseExit()
    {
        if (IsDead || !BattleSystem.instance.IsTacticWindowAvailable)
        {
            return;
        }
        DisplayInfo();
    }

    void DisplayInfo()
    {
        //CurOccup.gameObject.GetComponentInChildren<Animator>().SetTrigger("UnderAttack");
        //Debug.Log("Display");
        if (IsForcedDislayInfo)
        {
            return;
        }

        if (TacticInfoWindow.GetComponent<InfoWindow>().slot != this)
        {
            IsDisplayInfo = false;
            TacticInfoWindow.GetComponent<InfoWindow>().slot = this;
        }
        //Debug.Log("dOWN!");
        if (BattleSystem.instance.IsTacticWindowAvailable && !IsDisplayInfo)
        {
            //TacticInfoWindow.transform.SetParent(this.transform.parent.parent.parent, false);
            TacticInfoWindow.transform.localScale = Vector3.one;
            TacticInfoWindow.GetComponent<InfoWindow>().SetPos(Location.x,Location.y);
            TacticInfoWindow.SetActive(true);
            //Debug.Log("dOWN!");
            IsDisplayInfo = true;
        }
        else
        {
            TacticInfoWindow.SetActive(false);
            IsDisplayInfo =false;
        }
    }


    public void Init<T>(T Type)
    {
        data.CurTacticSlot = this;
        CharaObj = ObjectPool.GetObject(Type);
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(this.transform,false);
        CharaObj.transform.localPosition = Vector3.zero;
        data.curOccupIndex = ResourceRepo.instance.LevelPriority.Count - 1;
        CharaObj.SetActive(true);
    }

    public void Init<T>(T Type, CharaData m_data)
    {
        IsDead = false; //一定满员
        
        m_data.CurBattleSlot = this;
        data = m_data;
        data.CurTacticSlot = this;
        CharaObj = ObjectPool.GetObject(Type);
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(this.transform, false);
        CharaObj.transform.localPosition = Vector3.zero;
        CharaObj.SetActive(true);
    }

    public void Clear(bool isDeChara)
    {
        if (CharaObj != null && isDeChara)
        {
            ObjectPool.ReturnObject(CharaObj, CurOccup.occup);
        }
        CurOccup = null;
        CharaObj = null;
        data = null;
    }

    public void MyReGen()
    {
        if (CharaObj != null)
        {
            ObjectPool.ReturnObject(CharaObj, CurOccup.occup);
        }
        CurOccup = null;
        CharaObj = null;
        data = null;

        data = new CharaData(this);
        CharaObj = ObjectPool.GetObject(Occupation.Occup.Base0);
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(transform, false);
        CharaObj.transform.localPosition = Vector3.zero;
        data.curOccupIndex = ResourceRepo.instance.LevelPriority.Count - 1;
        data.RefreshData(CurOccup);
        CharaObj.SetActive(true);
        //CurOccup.Show(true);              //!!!!!!!!
        IsDead = false;
    }

    public void EnemyReGen()
    {
        /*
        if (CharaObj != null)
        {
            ObjectPool.ReturnObject(CharaObj, CurOccup.occup);
        }*/
        CurOccup = null;
        CharaObj = null;
        data = null;

        data = new CharaData(this);
        CharaObj = ObjectPool.GetObject(Occupation.Occup.Base0);
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(transform, false);
        CharaObj.transform.localPosition = Vector3.zero;
        data.curOccupIndex = ResourceRepo.instance.LevelPriority.Count - 1;
        data.RefreshData(CurOccup);
        CharaObj.SetActive(true);
        //CurOccup.Show(true);              //!!!!!!!!
        IsDead = false;
    }
    public void AddElementsByArray(List<int> elements) //token
    {
       data.AddElementsByArray(elements);
       data.ElementRefreshData(CurOccup);
       CurOccup.GetToken();
       data.TryLevelUp();
    }

    public void ChangeOccuption(Occupation newOccup)        //仅仅更换Occup，data调用配合
    {
        CharaObj.gameObject.SetActive(false);               //失活当前，入库
        ObjectPool.ReturnObject(CharaObj,CurOccup.occup);

        CharaObj = ObjectPool.GetObject(newOccup.occup);  //重新取得，赋值，挂载父和设置位置，激活对象
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(this.transform, false);
        CharaObj.transform.localPosition = Vector3.zero;
        CharaObj.SetActive(true);
    }

    

    

    

    public void StartBattle()
    {
        IsDead = false;
        ForwardSign = 0;
        ForwardAffirm = 0;
        CurOccup.AttackTime = -20f;
        CurOccup.RecoverTime = -20f;
    }           //一轮战斗开始时初始化参数
    public void TryAttack()                //每帧尝试攻击，正攻击，减血可负，已判定死亡则不能攻击
    {
        if (IsDead)                             //已经开始死亡，不能攻击
        {
            return;
        }
        if (CurOccup.AttackTime < -10f)                             //开始攻击循环
        { 
            CurOccup.AttackTime = 100 / CurOccup.As;
            CurOccup.AttackTime -= Time.deltaTime;
        }
        else if (CurOccup.AttackTime < 1e-4)                       
        {
            Debug.Log("Attack");
            Attack();
            CurOccup.AttackTime = 100 / CurOccup.As;
        }
        else                                                         //继续计时
        {
            CurOccup.AttackTime -= Time.deltaTime;
        }
    }

    public void Attack()                    //攻击,死了不打
    {
        CurOccup.anim.SetTrigger("Attack");
        int ti,tj,tk;
        List<Vector2Int> AttackArrange = new List<Vector2Int>(5);
        AttackArrange.Clear();
        switch (CurOccup.AttackMode)
        {
            default:
            case 0:
                break;
            case 1:
                ti = 0;   //?
                tj = Location.y;
                AttackArrange.Add(new Vector2Int(ti,tj));
                break;
            case 2:
                ti = 0;
                tj = Location.y;
                if (IsMine)
                {
                    for (int i = BattleSystem.instance.BattleHeight - 1; i >= 0; --i)
                    {
                        if (!BattleSystem.instance.EnemyBattleSlots[i][tj].IsDead)
                        {
                            ti = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = BattleSystem.instance.BattleHeight - 1; i >= 0; --i)
                    {
                        if (!BattleSystem.instance.MyBattleSlots[i][tj].IsDead)
                        {
                            ti = i;
                            break;
                        }
                    }
                }
                AttackArrange.Add(new Vector2Int(ti, tj));
                break;
            case 3:
                ti = 0;
                tj = Location.y;
                tk = -1;
                if (IsMine)
                {
                    for (int i = BattleSystem.instance.BattleHeight - 1; i >= 0; --i)
                    {
                        if (!BattleSystem.instance.EnemyBattleSlots[i][tj].IsDead)
                        {
                            ti = i;
                            if (tk >= 0)
                            {
                                break;
                            }
                            else
                            {
                                tk = ti;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = BattleSystem.instance.BattleHeight - 1; i >= 0; --i)
                    {
                        if (!BattleSystem.instance.MyBattleSlots[i][tj].IsDead)
                        {
                            ti = i;
                            if (tk >= 0)
                            {
                                break;
                            }
                            else
                            {
                                tk = ti;
                            }
                        }
                    }
                }
                AttackArrange.Add(new Vector2Int(ti, tj));
                AttackArrange.Add(new Vector2Int(ti -1, tj));
                AttackArrange.Add(new Vector2Int(ti +1, tj));
                AttackArrange.Add(new Vector2Int(ti, tj -1));
                AttackArrange.Add(new Vector2Int(ti, tj +1));
                break;
            case 4:
                if (Location.x == 0) //仅接敌
                {
                    ti = 0;   //
                    tj = Location.y;
                    AttackArrange.Add(new Vector2Int(ti, tj));
                    AttackArrange.Add(new Vector2Int(ti, tj-1));
                    AttackArrange.Add(new Vector2Int(ti, tj+1));
                }
                break;
            case 5:
                ti = 0;   //
                tj = Location.y;
                AttackArrange.Add(new Vector2Int(ti, 0));
                AttackArrange.Add(new Vector2Int(ti, 1));
                AttackArrange.Add(new Vector2Int(ti, 2));
                AttackArrange.Add(new Vector2Int(ti, 3));
                AttackArrange.Add(new Vector2Int(ti, 4));
                break;
        }
        Debug.Log("AttackMode"+CurOccup.AttackMode);
        for (int i = 0; i < AttackArrange.Count; i++)      //所有可攻击位置，如果有存活单位
        {
            //int ti = CurOccup.AttackArrangeList[i].y;   //?
            //int tj = Location.y + CurOccup.AttackArrangeList[i].x;
            ti = AttackArrange[i].x;
            tj = AttackArrange[i].y;
            if (ti < 0 || tj < 0 || ti >= BattleSystem.instance.BattleHeight || tj >= BattleSystem.instance.BattleWidth)
            {
                //Debug.Log("Error range!!");
                continue;
            }
            if (IsMine)
            {
                //Debug.Log("My Attack At " + ti + "," + tj);
                if (!BattleSystem.instance.EnemyBattleSlots[ti][tj].IsDead) //存活，存在
                {
                    Debug.Log("Ap" + data.Ap);
                    BattleSystem.instance.EnemyBattleSlots[ti][tj].UnderAttack(data.Ap, CurOccup.AttackVFX);
                }
            }
            else
            {
                //Debug.Log("Enemy Attack At " + ti + "," + tj);
                if (!BattleSystem.instance.MyBattleSlots[ti][tj].IsDead) //存活，存在
                {
                    Debug.Log("Ap" + data.Ap);
                    BattleSystem.instance.MyBattleSlots[ti][tj].UnderAttack(data.Ap, CurOccup.AttackVFX);
                }

            }
        }
    }

    /*
    public void Attack()                    //攻击,死了不打
    {
        CurOccup.anim.SetTrigger("Attack");
        for (int i = 0; i < CurOccup.AttackArrangeList.Count; i++)      //所有可攻击位置，如果有存活单位
        {
            int ti = CurOccup.AttackArrangeList[i].y;   //?
            int tj = Location.y + CurOccup.AttackArrangeList[i].x;
            if (ti < 0 || tj < 0 || ti >= BattleSystem.instance.BattleHeight || tj >= BattleSystem.instance.BattleWidth)
            {
                Debug.Log("Error range!!");
                continue;
            }
            if (IsMine)
            {
                //Debug.Log("My Attack At " + ti + "," + tj);
                if (!BattleSystem.instance.EnemyBattleSlots[ti][tj].IsDead) //存活，存在
                {
                    BattleSystem.instance.EnemyBattleSlots[ti][tj].UnderAttack(data.Ap,CurOccup.AttackVFX);
                }
            }
            else
            {
                //Debug.Log("Enemy Attack At " + ti + "," + tj);
                if (!BattleSystem.instance.MyBattleSlots[ti][tj].IsDead) //存活，存在
                {
                    BattleSystem.instance.MyBattleSlots[ti][tj].UnderAttack(data.Ap,CurOccup.AttackVFX);
                }
                
            }
            
        }
    }
    */
    public void UnderAttack(float point,ObjectPool.VFX vfx)    //受击减血，无下限
    {
        //Debug.Log(CurOccup.name +IsMine + "" + Location.x + " " + Location.y + "Under Attack");
        CurOccup.anim.SetTrigger("UnderAttack");
        GameObject particle = ObjectPool.GetObject(vfx);
        particle.transform.SetParent(CurOccup.Particles,false);
        particle.SetActive(true);
        data.CurHp -= point;
        if (data.CurHp < 0)
        {
            data.CurHp = 0;
        }

        if (data.MaxHp -data.CurHp >1e-2)
        {
            CurOccup.HpSlider.gameObject.SetActive(true);
            CurOccup.HpSlider.value = data.CurHp / data.MaxHp;
        }
    }
    public void TryRecover()    //没死，回复生命
    {
        if (IsDead)                                     //死了不管
        {
            return;
        }
        if (CurOccup.RecoverTime < -10f)
        {
            CurOccup.RecoverTime = 1f;
            CurOccup.AttackTime -= Time.deltaTime;//开始回复循环计时
        }
        else if (CurOccup.RecoverTime < 1e-4)
        {
            //Debug.Log(CurOccup.name + IsMine + "" + Location.x + " " + Location.y + "TryRecover to" + data.CurHp);

            data.CurHp += data.ReHp;
            if (data.CurHp > data.MaxHp)    //满血限制
            {
                data.CurHp = data.MaxHp;
                CurOccup.HpSlider.value = data.CurHp / data.MaxHp;
                CurOccup.HpSlider.gameObject.SetActive(false);
            }
            else
            {
                CurOccup.HpSlider.value = data.CurHp / data.MaxHp;
            }
            CurOccup.RecoverTime = 1f;                  //重置计时
        }
        else                                           //没到时间，继续计时
        {
            CurOccup.RecoverTime -= Time.deltaTime;
        }
    }
    public void TryDeath()                  //尝试死亡（？）
    {
        if (!IsDead && data.CurHp < 1e-2)      //0.01以下丝血，直接死亡,只能死一次
        {
            //Death();
            //Debug.Log(CurOccup.name + IsMine + "" + Location.x + " " + Location.y + "Start Death"+data.CurHp);
            data.CurHp = 0;
            IsDead = true;             //正式死亡播放死亡动画，无法攻击，无法回复，被攻击无影响
            BattleField.instance.OndeadNum++;
            if (IsMine)
            {
                BattleField.instance.MyTp.CurTp--;
            }
            else
            {
                BattleField.instance.EnemyTp.CurTp--;
            }
            CurOccup.anim.SetTrigger("Death");
            for (int i = Location.x + 1; i < BattleSystem.instance.BattleHeight; i++)                //3.后面全部上进位标识
            {
                if (IsMine)
                {
                    BattleSystem.instance.MyBattleSlots[i][Location.y].ForwardSign++;
                }
                else
                {
                    BattleSystem.instance.EnemyBattleSlots[i][Location.y].ForwardSign++;
                }
            }
        }
    }
    public void DeathFinal()                //死亡动画完后，正式销毁，且此帧内完成补位
    {
        Debug.Log("Death Final");
        CurOccup.Hide();         
        CurOccup.HpSlider.gameObject.SetActive(false);//1.不显示本体和Hp
        ObjectPool.ReturnObject(CurOccup.gameObject, CurOccup.occup); //2.当前的Occup Prefab封存
        CharaObj = null;
        
        
        for (int i = Location.x + 1; i < BattleSystem.instance.BattleHeight; i++)                //3.后面全部上进位确认标识
        {
            if (IsMine)
            {
                BattleSystem.instance.MyBattleSlots[i][Location.y].ForwardAffirm++;
            }
            else
            {
                BattleSystem.instance.EnemyBattleSlots[i][Location.y].ForwardAffirm++;
            }
        }
        BattleField.instance.OndeadNum--;
        if (IsMine)
        {
            BattleField.instance.MyIsdeadNum++;
        }
        else
        {
            BattleField.instance.EnemyIsdeadNum++;
        }
    }
    public void TryMoveForward()   //前面人都真的死亡了，直接移动补位，不会让死亡中的人也向前移动
    {
        if (IsDead)
        {
            //Debug.Log("Has been Dead,No Movement.");
            ForwardAffirm = 0;
            return;
        }
        if(ForwardSign !=0 && ForwardAffirm == ForwardSign)
        {
            Debug.Log("Move from "+Location+" to "+ (Location.x - ForwardAffirm) + ","+ (Location.y));
            if (IsMine)
            {
                BattleSystem.instance.MyBattleSlots[Location.x - ForwardAffirm][Location.y].ExchangeData(this,false);
            }
            else
            {
                BattleSystem.instance.EnemyBattleSlots[Location.x - ForwardAffirm][Location.y].ExchangeData(this,false);
            }
            ForwardAffirm = ForwardSign = 0;
        }
        else if (ForwardAffirm > ForwardSign)
        {
            Debug.Log("ERROR!");
        }
    }
    public void ExchangeData(GridSlot newSlot, bool isTactic)          //此slot获取newSlot的数值
    {
        //Obj
        if (CharaObj != null)
        {
            ObjectPool.ReturnObject(CharaObj,CurOccup.occup);//旧的归档！
        }
        CharaObj = newSlot.CharaObj;                    //Obj引用赋值
        data = newSlot.data;
        data.CurTacticSlot = this;//data引用赋值
        CurOccup = CharaObj.GetComponent<Occupation>();     //obj代码引用插入slot
        CharaObj.transform.SetParent(this.transform, false);        //obj prefab移动
        CharaObj.transform.localPosition = Vector3.zero;
        CurOccup.Show(isTactic,IsMine);                                //显示
        //param
        IsDead = newSlot.IsDead;
        IsMine = newSlot.IsMine;
        ForwardAffirm = ForwardSign = 0;
        //旧的封
        newSlot.IsDead = true;
        newSlot.CurOccup = null;
        newSlot.CharaObj = null;
        newSlot.data = null;
    }
    public void StopBattle()
    {
        ForwardSign = 0;
        ForwardAffirm = 0;
        if (!IsDead)
        {
            CurOccup.AttackTime = -20f;
            CurOccup.RecoverTime = -20f;
            CurOccup.HpSlider.gameObject.SetActive(false);
        }
        
    }               //一轮战斗结束后收拾参数

    /*
    public void Regenerate()//直接替换为新的白板，全新数据，当末位进位时使用，末位连续进位几次就构造几次
    {
        if (IsMine)
        {
            GridSlot tmp = BattleSystem.instance.MyBattleSlots[Location.x][BattleSystem.instance.BattleHeight - 1];
            tmp.data = new CharaData(tmp);
            tmp.CharaObj = ObjectPool.GetObject(Occupation.Occup.Base0);
            tmp.CurOccup = tmp.CharaObj.GetComponent<Occupation>();
            tmp.CharaObj.transform.SetParent(tmp.transform, false);
            tmp.CharaObj.transform.localPosition = Vector3.zero;
            tmp.data.curOccupIndex = ResourceRepo.instance.LevelPriority.Count - 1;
            tmp.CharaObj.SetActive(true);
            tmp.CurOccup.Show();
            BattleSystem.instance.MyBattleCost--;
        }
        else
        {
            GridSlot tmp = BattleSystem.instance.EnemyBattleSlots[Location.x][BattleSystem.instance.BattleHeight - 1];
            tmp.data = new CharaData(tmp);
            tmp.CharaObj = ObjectPool.GetObject(Occupation.Occup.Base0);
            tmp.CurOccup = tmp.CharaObj.GetComponent<Occupation>();
            tmp.CharaObj.transform.SetParent(tmp.transform, false);
            tmp.CharaObj.transform.localPosition = Vector3.zero;
            tmp.data.curOccupIndex = ResourceRepo.instance.LevelPriority.Count - 1;
            tmp.CharaObj.SetActive(true);
            tmp.CurOccup.Show();
            BattleSystem.instance.EnemyBattleCost--;
        }
    }
    */
}
