using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Occupation : MonoBehaviour
{
    public enum Occup
    {
        Base0,      //�װ�

        ApRp1_Sword,//���ؽ�1
        Ba1_Sword,  //ƽ�⽣1
        HpSp1_Sword,//Ѫ�ٽ�1
        HpSp1_Shield,//Ѫ�ٶ�1
        Ba_core,       //ƽ��1

        ApRp2_Sword    //���ؽ�2




    }

    [SerializeField] public GameObject Display;
    [SerializeField] public Occup occup;

    [Space]
    [SerializeField] public float Hp;
    [SerializeField] public float Rp;
    [SerializeField] public float Ap;
    [SerializeField] public float As;

    [SerializeField] public int Priority;

    [SerializeField] public int FireReq;
    [SerializeField] public int WaterReq;
    [SerializeField] public int GrassReq;
    [SerializeField] public int EarthReq;

    [SerializeField]public List<int> ElementReq;

    [SerializeField] public Animator anim;
    [SerializeField] public Slider HpSlider;

    [SerializeField] public List<Vector2Int> AttackArrangeList;

    public float AttackTime;
    public float RecoverTime;

    void Awake()
    {
        Hide();
        //AttackArrangeList = new List<Vector2Int>();
    }
    void OnEnable()
    {
        AttackTime = -20f;
        RecoverTime = -20f;
    }
    void ElemDataInit()
    {
        ElementReq.Clear();
        ElementReq.Add(0);
        ElementReq.Add(GrassReq);
        ElementReq.Add(WaterReq);
        ElementReq.Add(FireReq);
        ElementReq.Add(EarthReq);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        anim.enabled = true;
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        Display.SetActive(true);
    }

    public void Hide()
    {
        anim.enabled = false;
        Display.SetActive(false);
    }

    public void GetToken()
    {
        FlashColor(1f, new Color(255,255,0,255));
    }

    private Color c_color;

    void FlashColor(float time,Color color)
    {
        c_color = Display.GetComponent<Image>().color;
        //�ֱ��Ӧ��R,G,B,͸����
        Display.GetComponent<Image>().color = color;
        Invoke("ResetColor", time);
    }

    void ResetColor()
    {
        Display.GetComponent<Image>().color = c_color;
    }

    public bool IsAvailable(CharaData data)
    {
        if (data == null)
        {
            return false;
        }

        ElementReq = new List<int>();
        ElemDataInit();

        for (int i = 1; i < 5; i++)
        {
            //Debug.Log(data.ElementValues[i]);
            //Debug.Log(ElementReq[i]);

            if (data.ElementValues[i] 
                < 
                ElementReq[i])
            {
                return false;
            }
        }
        return true;
    }

    public void LevelUpExitGrid()
    {
        FlashColor(0.1f, new Color(255, 255, 0, 255));
    }

    public void LevelUpEnterGrid()
    {
        this.Show();
        FlashColor(0.1f, new Color(255, 0, 0, 255));
    }
}
