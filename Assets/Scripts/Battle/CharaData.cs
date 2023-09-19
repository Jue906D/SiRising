using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaData
{
    private const int ElementCount = 5;
    public List<int> ElementValues;
    public bool isMine = true;

    //data
    [SerializeField] public float CurHp;
    [SerializeField] public float MaxHp;
    [SerializeField] public float ReHp;
    [SerializeField] public float Ap;
    [SerializeField] public float As;

    [SerializeField] public int curOccupIndex; //只允许变小

    public GridSlot CurTacticSlot;
    public GridSlot CurBattleSlot;

    public CharaData(GridSlot curSlot)
    {
        ElementValues = new List<int>(5);
        for (int i = 0; i < 5; i++)
        {
            ElementValues.Add(0);
        }

        CurTacticSlot = curSlot;
    }

    public CharaData(CharaData m_data, GridSlot curSlot)
    {
        ElementValues = new List<int>(m_data.ElementValues);
        isMine = m_data.isMine;
        CurHp = m_data.CurHp;
        MaxHp = m_data.MaxHp;
        ReHp = m_data.ReHp;
        Ap = m_data.Ap;
        As = m_data.As;
        curOccupIndex = m_data.curOccupIndex;
        CurTacticSlot = null;
        CurBattleSlot = curSlot;
    }

    public void AddElementsByArray(List<int> elements) //元素增幅
    {
        for (int i = 0; i < elements.Count; i++)
        {
            //Debug.Log("Add " + elements[i] + " in " + i);
            ElementValues[i] += elements[i];
        }
    }

    public void RefreshData(Occupation occup)
    {
        MaxHp = occup.Hp + 5 * ElementValues[(int)Element.Water] + 2 * ElementValues[(int)Element.Earth];
        CurHp = MaxHp;
        Ap = occup.Ap + 2 * ElementValues[(int)Element.Fire] + 1 * ElementValues[(int)Element.Grass];
        As = occup.As + 1 * ElementValues[(int)Element.Fire] + 3 * ElementValues[(int)Element.Earth];
        ReHp = occup.Rp + 1 * ElementValues[(int)Element.Water] + 3 * ElementValues[(int)Element.Grass];
    }

    public void ElementRefreshData(Occupation occup)
    {
        float tmp = occup.Hp + 5 * ElementValues[(int)Element.Water] + 2 * ElementValues[(int)Element.Earth];
        CurHp += tmp - MaxHp;
        MaxHp = tmp;
        Ap = occup.Ap + 2 * ElementValues[(int)Element.Fire] + 1 * ElementValues[(int)Element.Grass];
        As = occup.As + 1 * ElementValues[(int)Element.Fire] + 3 * ElementValues[(int)Element.Earth];
        ReHp = occup.Rp + 1 * ElementValues[(int)Element.Water] + 3 * ElementValues[(int)Element.Grass];
    }

    public void TryLevelUp()
    {
        int tempIndex = -1;
        for (int i = curOccupIndex; i >= 0; i--)
        {
            if (i == curOccupIndex)
            {
                continue;
            }

            if (ResourceRepo.instance.LevelPriority[i].IsAvailable(this))
            {
                tempIndex = i;
            }
        }

        if (tempIndex >= 0)//需要升级
        {
            Debug.Log("level Up" + ResourceRepo.instance.LevelPriority[tempIndex].name);
            CurTacticSlot.CurOccup.LevelUpExitGrid();
            curOccupIndex = tempIndex;
            CurTacticSlot.ChangeOccuption(ResourceRepo.instance.LevelPriority[curOccupIndex]);
            this.RefreshData(CurTacticSlot.CurOccup);
            CurTacticSlot.CurOccup.LevelUpEnterGrid();
        }
    }
    

}
