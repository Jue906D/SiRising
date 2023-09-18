using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    [SerializeField] public Image OccupImage;
    [SerializeField] public TextMeshProUGUI OccupText;

    [SerializeField] public TextMeshProUGUI SlideCurHp;
    [SerializeField] public TextMeshProUGUI SlideMaxHp;
    [SerializeField] public Slider HpSlider;

    [SerializeField] public TextMeshProUGUI MaxHp;
    [SerializeField] public TextMeshProUGUI Rp;
    [SerializeField] public TextMeshProUGUI Ap;
    [SerializeField] public TextMeshProUGUI Sp;

    [SerializeField] public TextMeshProUGUI Grass;
    [SerializeField] public TextMeshProUGUI Water;
    [SerializeField] public TextMeshProUGUI Fire;
    [SerializeField] public TextMeshProUGUI Earth;

    [SerializeField] public GridSlot slot; 

    void OnEnable()
    {
        Refresh();

    }


    public void Refresh()
    {
        if (slot.CurOccup == null)
        {
            return;
        }

        OccupImage.sprite = slot.CurOccup.Display.GetComponent<Image>().sprite;
        OccupText.text = slot.CurOccup.name;

        SlideCurHp.text = slot.data.CurHp.ToString();
        SlideMaxHp.text = slot.data.MaxHp.ToString();
        HpSlider.value = slot.data.CurHp/slot.data.MaxHp;

        MaxHp.text = slot.data.MaxHp.ToString();
        Rp.text = slot.data.ReHp.ToString();
        Ap.text = slot.data.Ap.ToString();
        Sp.text = slot.data.As.ToString();


        Grass.text = slot.data.ElementValues[1].ToString();
        Water.text = slot.data.ElementValues[2].ToString();
        Fire.text = slot.data.ElementValues[3].ToString();
        Earth.text = slot.data.ElementValues[4].ToString();
    }

    public void SetPos(int i,int j)
    {
        RectTransform rect = transform.GetComponent<RectTransform>();
        Vector3 tmp;
        if (slot.IsTactic)
        {
            tmp = new Vector3(
                BattleSystem.instance.TacticInfoOffset.x + j * BattleSystem.instance.TacticInfoStepLength.x,
                BattleSystem.instance.TacticInfoOffset.y + i * BattleSystem.instance.TacticInfoStepLength.y,
                0
            );
        }
        else if(slot.IsMine)
        {
            tmp = new Vector3(
                BattleSystem.instance.MyBattleInfoOffset.x + i * BattleSystem.instance.MyBattleInfoStepLength.x,
                BattleSystem.instance.MyBattleInfoOffset.y + j * BattleSystem.instance.MyBattleInfoStepLength.y,
                0
            );
        }
        else
        {  
            tmp = new Vector3(
                BattleSystem.instance.EnemyBattleInfoOffset.x + i * BattleSystem.instance.EnemyBattleInfoStepLength.x,
                BattleSystem.instance.EnemyBattleInfoOffset.y + j * BattleSystem.instance.EnemyBattleInfoStepLength.y,
                0
            );
        }
        
        Vector2 pivot = new Vector2(0,1);//(0,1)
        //右侧出框转左
        if (tmp.x + 600 > 960)
        {
            pivot.x = 1;
            if (tmp.x - 600 < -960)//左侧出框转偏移
            {
                pivot.x = (tmp.x + 600 - 960) / 600;
            }
        }
        //下侧出框转上
        if (tmp.y - 600 < -540)
        {
            pivot.y = 0;
            if (tmp.y + 600 > 540)//上侧仍出框转偏移
            {
                pivot.y = 1 + (tmp.y - 600 + 540) / 600;
            }
        }

        rect.anchoredPosition3D = tmp;
        rect.pivot = pivot;
        //float width = rect.sizeDelta.x;
        //float height = rect.sizeDelta.y;
        /*
        Vector2 pivot = new Vector2();

        if (pos.x + width <= Screen.width) // 优先靠右
        {
            Debug.Log("右");
            pivot.x = 0;
        }
        else // 左
        {
            Debug.Log("作");
            pivot.x = 1;
        }

        if (pos.y - height >= 0) // 优先靠下
        {
            Debug.Log("下");
            pivot.y = 1;
        }
        else // 上
        {
            Debug.Log("上");
            pivot.y = 0;
        }
        
        rect.pivot = pivot;*/
    }
}
