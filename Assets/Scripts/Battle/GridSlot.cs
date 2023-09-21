using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Authentication;
using UnityEngine;
using UnityEngine.UIElements;

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
        if (IsDead)
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
        if (IsDead)
        {
            return;
        }
        DisplayInfo();
    }

    void OnMouseExit()
    {
        if (IsDead)
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
        CharaObj = ObjectPool.GetObject(Type);
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(this.transform,false);
        CharaObj.transform.localPosition = Vector3.zero;
        data.curOccupIndex = ResourceRepo.instance.LevelPriority.Count - 1;
        CharaObj.SetActive(true);
    }

    public void Init<T>(T Type, CharaData m_data)
    {
        IsDead = false; //һ����Ա
        
        m_data.CurBattleSlot = this;
        data = m_data;
        CharaObj = ObjectPool.GetObject(Type);
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(this.transform, false);
        CharaObj.transform.localPosition = Vector3.zero;
        CharaObj.SetActive(true);
    }

    public void Clear()
    {
        ObjectPool.ReturnObject(CharaObj,CurOccup.occup); 
        //CurOccup = null;
        //CharaObj = null;
        //data = null;
    }

    public void ReGen()
    {
        ObjectPool.ReturnObject(CharaObj, CurOccup.occup);
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
        CurOccup.Show();
        IsDead = false;
    }
    public void AddElementsByArray(List<int> elements) //token
    {
       data.AddElementsByArray(elements);
       data.ElementRefreshData(CurOccup);
       CurOccup.GetToken();
       data.TryLevelUp();
    }

    public void ChangeOccuption(Occupation newOccup)        //��������Occup��data�������
    {
        CharaObj.gameObject.SetActive(false);               //ʧ�ǰ�����
        ObjectPool.ReturnObject(CharaObj,CurOccup.occup);

        CharaObj = ObjectPool.GetObject(newOccup);  //����ȡ�ã���ֵ�����ظ�������λ�ã��������
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
    }           //һ��ս����ʼʱ��ʼ������
    public void TryAttack()                //ÿ֡���Թ���������������Ѫ�ɸ������ж��������ܹ���
    {
        if (IsDead)                             //�Ѿ���ʼ���������ܹ���
        {
            return;
        }
        if (CurOccup.AttackTime < -10f)                             //��ʼ����ѭ��
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
        else                                                         //������ʱ
        {
            CurOccup.AttackTime -= Time.deltaTime;
        }
    }
    public void Attack()                    //����,���˲���
    {
        for (int i = 0; i < CurOccup.AttackArrangeList.Count; i++)      //���пɹ���λ�ã�����д�λ
        {
            int ti = CurOccup.AttackArrangeList[i].y;   //?
            int tj = Location.y + CurOccup.AttackArrangeList[i].x;
            if (ti < 0 || tj < 0 || ti >= BattleSystem.instance.BattleHeight || tj >= BattleSystem.instance.BattleWidth)
            {
                continue;
            }
            if (!BattleSystem.instance.EnemyBattleSlots[ti][tj].IsDead) //������
            {
                
                if (IsMine)
                {
                    //Debug.Log("My Attack At " + ti + "," + tj);
                    BattleSystem.instance.EnemyBattleSlots[ti][tj].UnderAttack(CurOccup.Ap);
                }
                else
                {
                    //Debug.Log("Enemy Attack At " + ti + "," + tj);
                    BattleSystem.instance.MyBattleSlots[ti][tj].UnderAttack(CurOccup.Ap);
                }

            }
        }
    }
    public void UnderAttack(float point)    //�ܻ���Ѫ��������
    {
        //Debug.Log(CurOccup.name +IsMine + "" + Location.x + " " + Location.y + "Under Attack");
        CurOccup.anim.SetTrigger("UnderAttack");
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
    public void TryRecover()    //û�����ظ�����
    {
        if (IsDead)                                     //���˲���
        {
            return;
        }
        if (CurOccup.RecoverTime < -10f)
        {
            CurOccup.RecoverTime = 1f;
            CurOccup.AttackTime -= Time.deltaTime;//��ʼ�ظ�ѭ����ʱ
        }
        else if (CurOccup.RecoverTime < 1e-4)
        {
            //Debug.Log(CurOccup.name + IsMine + "" + Location.x + " " + Location.y + "TryRecover to" + data.CurHp);

            data.CurHp += data.ReHp;
            if (data.CurHp > data.MaxHp)    //��Ѫ����
            {
                data.CurHp = data.MaxHp;
            }
            CurOccup.RecoverTime = 1f;                  //���ü�ʱ
        }
        else                                           //û��ʱ�䣬������ʱ
        {
            CurOccup.RecoverTime -= Time.deltaTime;
        }
    }
    public void TryDeath()                  //��������������
    {
        if (!IsDead && data.CurHp < 1e-2)      //0.01����˿Ѫ��ֱ������,ֻ����һ��
        {
            //Death();
            //Debug.Log(CurOccup.name + IsMine + "" + Location.x + " " + Location.y + "Start Death"+data.CurHp);
            data.CurHp = 0;
            IsDead = true;             //��ʽ�������������������޷��������޷��ظ�����������Ӱ��
            BattleField.instance.OndeadNum++;
            CurOccup.anim.SetTrigger("Death");
            for (int i = Location.x + 1; i < BattleSystem.instance.BattleHeight; i++)                //3.����ȫ���Ͻ�λ��ʶ
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
    public void DeathFinal()                //�������������ʽ���٣��Ҵ�֡����ɲ�λ
    {
        Debug.Log("Death Final");
        CurOccup.Hide();         
        CurOccup.HpSlider.gameObject.SetActive(false);//1.����ʾ
        ObjectPool.ReturnObject(CurOccup.gameObject, CurOccup.occup);                            //2.��ǰ��Occup Prefab���
        
        for (int i = Location.x + 1; i < BattleSystem.instance.BattleHeight; i++)                //3.����ȫ���Ͻ�λȷ�ϱ�ʶ
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
    public void TryMoveForward()   //ǰ���˶���������ˣ�ֱ���ƶ���λ�������������е���Ҳ��ǰ�ƶ�
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
                BattleSystem.instance.MyBattleSlots[Location.x - ForwardAffirm][Location.y].ExchangeData(this);
            }
            else
            {
                BattleSystem.instance.EnemyBattleSlots[Location.x - ForwardAffirm][Location.y].ExchangeData(this);
            }
            ForwardAffirm = ForwardSign = 0;
        }
        else if (ForwardAffirm > ForwardSign)
        {
            Debug.Log("ERROR!");
        }
    }
    public void ExchangeData(GridSlot newSlot)          //��slot��ȡnewSlot����ֵ
    {
        //Obj
        CharaObj = newSlot.CharaObj;                    //Obj���ø�ֵ
        data = newSlot.data;                            //data���ø�ֵ
        CurOccup = CharaObj.GetComponent<Occupation>();     //obj�������ò���slot
        CharaObj.transform.SetParent(this.transform, false);        //obj prefab�ƶ�
        CharaObj.transform.localPosition = Vector3.zero;
        CurOccup.Show();                                //��ʾ
        //param
        IsDead = newSlot.IsDead;
        IsMine = newSlot.IsMine;
        ForwardAffirm = ForwardSign = 0;
        //�ɵķ�
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
        
    }               //һ��ս����������ʰ����

    public void Regenerate()//ֱ���滻Ϊ�µİװ壬ȫ�����ݣ���ĩλ��λʱʹ�ã�ĩλ������λ���ξ͹��켸��
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

}
