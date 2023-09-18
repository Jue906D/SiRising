using System.Collections;
using System.Collections.Generic;
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


    void Awake()
    {
        IsDisplayInfo = false;
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
        DisplayInfo();
    }

    void OnMouseExit()
    {
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
    }
    public void AddElementsByArray(List<int> elements) //token
    {
       data.AddElementsByArray(elements);
       data.ElementRefreshData(CurOccup);
       CurOccup.GetToken();
       data.TryLevelUp();
    }

    public void ChangeOccuption(Occupation occupation)
    {
        CharaObj.gameObject.SetActive(false);
        ObjectPool.ReturnObject(CharaObj,CurOccup.occup);

        CharaObj = ObjectPool.GetObject(occupation.occup);
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(this.transform, false);
        CharaObj.transform.localPosition = Vector3.zero;
        CharaObj.SetActive(true);
    }

    public void UnderAttack(float point)
    {
        if (data.CurHp - point < 1e-2)
        {
            //Death();
            data.CurHp -= point;
            CurOccup.anim.SetTrigger("Death");
        }
        else
        {
            data.CurHp -= point;
            CurOccup.anim.SetTrigger("UnderAttack");
        }
    }

    public IEnumerator DeathFinal()
    {
        Debug.Log("Death Final");
        CurOccup.Hide();
        ObjectPool.ReturnObject(CurOccup.gameObject,CurOccup.occup);
        for (int i = Location.x; i < BattleSystem.instance.BattleHeight -1; i++)
        {
            if (IsMine)
            {
                BattleSystem.instance.MyBattleSlots[i][Location.y].ExchangeData(BattleSystem.instance.MyBattleSlots[i][Location.y+1]);
            }
            else
            {
                BattleSystem.instance.EnemyBattleSlots[i][Location.y].ExchangeData(BattleSystem.instance.EnemyBattleSlots[i][Location.y + 1]);
            }
        }
        yield return new WaitForSeconds(0.1f);
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

        
        yield return 0;
    }

    public void ExchangeData(GridSlot newSlot)
    {
        CharaObj = newSlot.CharaObj;
        data = newSlot.data;
        CurOccup = CharaObj.GetComponent<Occupation>();
        CharaObj.transform.SetParent(this.transform, false);
        CharaObj.transform.localPosition = Vector3.zero;
    }

    public void TryAttack()
    {
        if (CurOccup.AttackTime < -10f)
        { 
            CurOccup.AttackTime = 100 / CurOccup.As;
        }else if (CurOccup.AttackTime < 1e-4)
        {
            Debug.Log("Attack");
            Attack();
            CurOccup.AttackTime = 100 / CurOccup.As;
        }
        else
        {
            CurOccup.AttackTime -= Time.deltaTime;
        }
    }

    public void StopBattle()
    {
        CurOccup.AttackTime = -20f;
        CurOccup.RecoverTime = -20f;
    }

    public void TryRecover()
    {
        if (CurOccup.RecoverTime < -10f)
        {
            CurOccup.RecoverTime = 1f;
        }
        else if(CurOccup.RecoverTime < 1e-4)
        {
            //Debug.Log("TryRecover");
            if (data.MaxHp - data.CurHp < 1e-2f)
            {
                return;
            }
            else
            {
                data.CurHp += data.ReHp;
                if (data.CurHp > data.MaxHp)
                {
                    data.CurHp = data.MaxHp;
                }
            }
            CurOccup.RecoverTime = 1f;
        }
        else
        {
            CurOccup.RecoverTime -= Time.deltaTime;
        }
       
        
    }

    public void Attack()
    {
        for (int i = 0; i < CurOccup.AttackArrangeList.Count; i++)
        {
            int ti = CurOccup.AttackArrangeList[i].y;   //?
            int tj = Location.y + CurOccup.AttackArrangeList[i].x;
            if (ti < 0 || tj <0 ||ti >=BattleSystem.instance.BattleHeight||tj >=BattleSystem.instance.BattleWidth)
            {
                continue;
            }
            BattleSystem.instance.EnemyBattleSlots[ti][tj].UnderAttack(CurOccup.Ap);
        }
    }
}
