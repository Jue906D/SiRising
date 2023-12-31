using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Occupation : MonoBehaviour
{
    public enum Occup
    {
        Base0,      //白板

        ApRp1_Sword,//攻回剑1
        Ba1_Sword,  //平衡剑1
        HpSp1_Sword,//血速剑1
        HpSp1_Shield,//血速盾1
        Ba_core,       //平衡1

        ApRp2_Sword    //攻回剑2

    }


    [SerializeField] public int DataKey;

    [SerializeField] public int AttackMode = 0;
    //0 无
    //1 前1
    //2 后1
    //3 固定后 十字
    //4 邻接 宽3
    //5 前1行

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

    [SerializeField] public Transform Particles;
    [SerializeField] public ObjectPool.VFX AttackVFX;
    [SerializeField] public ObjectPool.VFX TeleportVFX;

    public float AttackTime;
    public float RecoverTime;

    public Sprite RawTacticDisplayImage;
    //public Sprite RawBattleDisplayImage;

    void Awake()
    {
        Hide();
        RawTacticDisplayImage = Display.GetComponent<Image>().sprite;
        
        //AttackArrangeList = new List<Vector2Int>();
    }
    void OnEnable()
    {
        AttackTime = -20f;
        RecoverTime = -20f;
    }

    public void RefreshData()
    {
        FireReq = DataLoader.instance.config.Fire[DataKey];
        WaterReq = DataLoader.instance.config.Water[DataKey];
        GrassReq = DataLoader.instance.config.Grass[DataKey];
        EarthReq = DataLoader.instance.config.Earth[DataKey];

        Ap = DataLoader.instance.config.Attack[DataKey];
        As = DataLoader.instance.config.AttackSpeed[DataKey];
        Hp = DataLoader.instance.config.Health[DataKey];
        Rp = DataLoader.instance.config.Recharge[DataKey];

        AttackMode = DataLoader.instance.config.AttackMode[DataKey];
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

    void Start()
    {
        RefreshData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(bool isTactic,bool isMine)
    {
        if (isTactic)
        {
            anim.enabled = false;
            Display.GetComponent<Image>().sprite = RawTacticDisplayImage;
            Color tmpColor = Display.GetComponent<Image>().color;
            Display.GetComponent<Image>().color = new Color(
                255f, 255f, 255f, 255f
            );
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            Display.SetActive(true);
        }
        else
        {
            if (isMine && Display.transform.localScale.x > 0)
            {
                this.Display.transform.localScale = new Vector3(-this.Display.transform.localScale.x,
                    this.Display.transform.localScale.y, this.Display.transform.localScale.z);
            }
            anim.enabled = true;
            anim.SetTrigger("Reset");
            Color tmpColor = Display.GetComponent<Image>().color;
            Display.GetComponent<Image>().color = new Color(
                255f, 255f, 255f, 255f
            );
            this.transform.localScale = new Vector3(1f, 1f, 1f);
            Display.SetActive(true);
        }
    }

    public void Hide()
    {
        anim.enabled = false;
        if (this.Display.transform.localScale.x < 0)
        {
            this.Display.transform.localScale = new Vector3(-this.Display.transform.localScale.x,
                this.Display.transform.localScale.y, this.Display.transform.localScale.z);
        }
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
        //分别对应着R,G,B,透明度
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
        this.Show(true,true);
        FlashColor(0.1f, new Color(255, 0, 0, 255));
    }
}
