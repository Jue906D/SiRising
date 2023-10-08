using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleSystem : MonoBehaviour
{

    public static BattleSystem instance;
    //队伍size
    [SerializeField] public int BattleWidth;
    [SerializeField] public int BattleHeight;

    //角色数据
    public Character[][] MyCharacters;      //[自中心起第i行][左起第j个]
    public Character[][] EnemyCharacters;
    //角色数目
    [SerializeField] public int MyCharacterCount;
    [SerializeField] public int EnemyCharacterCount;
    //战术生成位置
    [SerializeField] public Vector3 MyArmyStartPosition;
    [SerializeField] public Vector3 EnemyArmyStartPosition;
    //测试：token显示
    public TextMeshProUGUI[][][] MyElemDisplay;
    public TextMeshProUGUI[][][] EnemyElemDisplay;

    //偏移，UI偏移 ？？？？？？？
    [SerializeField] public float offset = 1.0f;
    [SerializeField] public float UIoffsetX = 28.0f;
    [SerializeField] public float UIoffsetY = 25.0f;

    //是否开始战斗
    public bool isBattleStart = false;

    //Test
    [SerializeField] public bool isTestMode = true;
    [SerializeField] public GameObject Canvas;


    //public int BattlingRow;
    //人物prefab
    [SerializeField] private ObjectPool.Minion MyInitMinion;
    [SerializeField] private ObjectPool.Minion EnemyInitMinion;

    //grid
    [SerializeField] public GameObject MyTacticGrid;

    [SerializeField] public GameObject MyBattleGrid;
    [SerializeField] public GameObject EnemyBattleGrid;

    //Slots [][]
    public GridSlot[][] MyTacticSlots;
    public GridSlot[][] MyBattleSlots;
    public GridSlot[][] EnemyBattleSlots;
    [SerializeField] public GameObject TacticSlot;
    [SerializeField] public GameObject BattleSlot;

    //VFX
    [SerializeField] public float CharaDisappearTime = 0.04f;
    [SerializeField] public float CharaAppearTime = 0.04f;
    [SerializeField] public float TacticGridMoveTime = 0.5f;
    [SerializeField] public float MediateTime = 0.5f;
    [SerializeField] public float BattleGridMoveTime = 0.5f;

    //all grid to TRANSMIT
    [SerializeField] public RectTransform TacticUI;
    [SerializeField] public RectTransform TacticContent;

    [SerializeField] public RectTransform BattleUI;
    [SerializeField] public RectTransform BattleContent;
    [SerializeField] public Vector3 TacticMoveDistance = new Vector3(0, 1200f, 0);
    [SerializeField] public Vector3 BattleMoveDistance = new Vector3(0, 1200f, 0);

    //
    public bool isBattleRoundStart;
    public bool InBattleRound;

    //Info
    [SerializeField] public GameObject TacticInfoWindow;
    [SerializeField] public GameObject BattleInfoWindow;
    public bool IsTacticWindowAvailable = true;

    //Button
    [SerializeField] public GameObject LButton;
    [SerializeField] public GameObject RButton;
    [SerializeField] public GameObject EButton;

    [SerializeField] public Vector2 TacticInfoOffset;
    [SerializeField] public Vector2 TacticInfoStepLength;
    [SerializeField] public Vector2 MyBattleInfoOffset;
    [SerializeField] public Vector2 MyBattleInfoStepLength;
    [SerializeField] public Vector2 EnemyBattleInfoOffset;
    [SerializeField] public Vector2 EnemyBattleInfoStepLength;

    [SerializeField] public int EnemyBattleCost;
    [SerializeField] public int MyBattleCost;
    

    [SerializeField] [Range(2,1000)] public int RoundsToBattle;
    [SerializeField] public GameObject BattleField;

    public bool IsFirstEnemyGen;

    //Enemy Gen
    public List<int>[] RoundsAllElementAdd;

    void Awake()
    {
        offset = 1.0f;
        instance = this;
        MyCharacters = new Character[BattleHeight][];
        EnemyCharacters = new Character[BattleHeight][];
        MyElemDisplay = new TextMeshProUGUI[BattleHeight][][];
        EnemyElemDisplay = new TextMeshProUGUI[BattleHeight][][];
        for (int i = 0; i < MyCharacters.Length; i++)
        {
            MyCharacters[i] = new Character[BattleWidth];
        }
        for (int i = 0; i < EnemyCharacters.Length; i++)
        {
            EnemyCharacters[i] = new Character[BattleWidth];
        }
        for (int i = 0; i < MyElemDisplay.Length; i++)
        {
            MyElemDisplay[i] = new TextMeshProUGUI[BattleWidth][];
            for (int j = 0; j < MyElemDisplay[i].Length; j++)
            {
                MyElemDisplay[i][j] = new TextMeshProUGUI[5];
            }
        }
        for (int i = 0; i < EnemyElemDisplay.Length; i++)
        {
            EnemyElemDisplay[i] = new TextMeshProUGUI[BattleWidth][];
            for (int j = 0; j < EnemyElemDisplay[i].Length; j++)
            {
                EnemyElemDisplay[i][j] = new TextMeshProUGUI[5];
            }
        }
        MyArmyStartPosition = new Vector3(MyArmyStartPosition.x, MyArmyStartPosition.y, MyArmyStartPosition.z);
        EnemyArmyStartPosition = new Vector3(EnemyArmyStartPosition.x, EnemyArmyStartPosition.y, EnemyArmyStartPosition.z);
        RoundsAllElementAdd = new List<int>[BattleHeight];
        for (int i = 0; i < RoundsAllElementAdd.Length; i++)
        {
            RoundsAllElementAdd[i] = new List<int>();
            for (int j = 0; j < 5; j++)
            {
                RoundsAllElementAdd[i].Add(0);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        isBattleStart = true;
        isBattleRoundStart = false;
        IsTacticWindowAvailable = true;
        IsFirstEnemyGen = true;
        SlotsInit();
        StartCoroutine(TacticCharaAppear());
    }

    // Update is called once per frame
    void Update()
    {
        if (isBattleStart)
        {
            //StartGameSpawn();
            isBattleStart = false;
        }

        if (!InBattleRound && RoundCount.instance.CurCount > 0 && RoundCount.instance.CurCount % RoundsToBattle == 0)
        {
            isBattleRoundStart =true;
        }

        if (InBattleRound && RoundCount.instance.CurCount > 0 && RoundCount.instance.CurCount % RoundsToBattle == 1)
        {
            InBattleRound = false;
        }

        if (isBattleRoundStart)
        {
            InBattleRound = true;
            isBattleRoundStart = false;
            StartCoroutine(TacticToBattle());
        }
    }

    public void MyRowBonus(int row, List<int> elementBonus)
    {
        for (int i = 0; i < MyCharacters[row].Length; i++)
        {
            MyCharacters[row][i].AddElementsByArray(elementBonus);
        }
    }

    public void EnemyRowBonus(int row, List<int> elementBonus)
    {
        for (int i = 0; i < MyCharacters[row].Length; i++)
        {
            EnemyCharacters[row][i].AddElementsByArray(elementBonus);
        }
    }

    public Vector3 GetTrans(bool isMine, int row, int line)
    {
        //Debug.Log(row+ " " + line);
        if (isMine)
        {
            return new Vector3(
                MyArmyStartPosition.x + offset * line,
                MyArmyStartPosition.y - offset * row,
                0);
        }
        else
        {
            return new Vector3(
                EnemyArmyStartPosition.x + offset * line,
                EnemyArmyStartPosition.y + offset * row,
                0);
        }
    }

    public Vector3 GetUITrans(bool isMine, int row, int line)
    {
        //Debug.Log(row+ " " + line);
        if (isMine)
        {
            return new Vector3(
                (MyArmyStartPosition.x +  line) * UIoffsetX ,
                (MyArmyStartPosition.y - row) * UIoffsetY,
                0);
        }
        else
        {
            return new Vector3(
                (EnemyArmyStartPosition.x + line) * UIoffsetX,
                (EnemyArmyStartPosition.y + row) * UIoffsetY,
                0);
        }
    }

    private void StartGameSpawn()
    {
        for (int i = 0; i < MyCharacters.Length; i++)
        {
            for (int j = 0; j < MyCharacters[i].Length; j++)//i行j列
            {
                if (!isTestMode)
                {
                    //MyCharacters[i][j] = new Character();
                    GameObject tmp = ObjectPool.GetObject(MyInitMinion);
                    //tmp.transform.position = GetTrans(true, i, j);//手动设置trans
                    //自动Grid lyaout
                    tmp.gameObject.transform.SetParent(MyTacticGrid.transform,false);
                    //Debug.Log(GetTrans(true, i, j));
                    MyCharacters[i][j] = tmp.GetComponent<Character>();
                    tmp.SetActive(true);
                }
                else
                {
                    GameObject tmp = ObjectPool.GetObject(ObjectPool.UI.ElemValue);
                    //tmp.transform.position = GetUITrans(true, i, j);
                    //for (int k = 1; k <= 4; ++k)
                    {
                        MyElemDisplay[i][j][1] = tmp.GetComponent<ElemValue>().Grass;
                        MyElemDisplay[i][j][2] = tmp.GetComponent<ElemValue>().Water;
                        MyElemDisplay[i][j][3] = tmp.GetComponent<ElemValue>().Fire;
                        MyElemDisplay[i][j][4] = tmp.GetComponent<ElemValue>().Earth;
                    }
                    tmp.transform.SetParent(Canvas.transform, true);
                    tmp.GetComponent<RectTransform>().localPosition = GetUITrans(true, i, j);
                    tmp.SetActive(true);
                }
            }
        }
        /*
        for (int i = 0; i < EnemyCharacters.Length; i++)
        {
            for (int j = 0; j < EnemyCharacters[i].Length; j++)//i行j列
            {
                if (!isTestMode)
                {
                    //MyCharacters[i][j] = new Character();
                    GameObject tmp = ObjectPool.GetObject(EnemyInitMinion);
                    tmp.transform.position = GetTrans(false, i, j);
                    //Debug.Log(GetTrans(true, i, j));
                    EnemyCharacters[i][j] = tmp.GetComponent<Character>();
                    tmp.SetActive(true);
                }
                else
                {
                    GameObject tmp = ObjectPool.GetObject(ObjectPool.UI.ElemValue);
                    //tmp.transform.position = GetUITrans(false, i, j);
                    //for (int k = 1; k <= 4; ++k)
                    {
                        EnemyElemDisplay[i][j][1] = tmp.GetComponent<ElemValue>().Grass;
                        EnemyElemDisplay[i][j][2] = tmp.GetComponent<ElemValue>().Water;
                        EnemyElemDisplay[i][j][3] = tmp.GetComponent<ElemValue>().Fire;
                        EnemyElemDisplay[i][j][4] = tmp.GetComponent<ElemValue>().Earth;
                    }
                    tmp.transform.SetParent(Canvas.transform,true);
                    tmp.GetComponent<RectTransform>().localPosition= GetUITrans(false, i, j);
                    tmp.SetActive(true);
                }
                
            }
        }
        */
    }

    public void RoundBattle()//回合战斗流程
    {
        //从近到远小兵进攻，我方和对面和同时
        for (int i = 0; i < BattleHeight; i++)
        {
            for (int j = 0; j < BattleWidth; j++)
            {

            }
        }

    }

    public void RefreshDisplayElemInLine(bool isMine, int row, List<int> elemList)
    {
        //Debug.Log(row+""+elemList.ToArray().ToString());
        if (isTestMode)
        {
            if (isMine)
            {
                for (int i = 0; i < MyElemDisplay[row].Length; i++)
                {
                    for (int j = 1; j <= 4; ++j)
                    {
                        string tmp = (MyElemDisplay[row][i][j].text.ToString());
                        tmp = (Convert.ToInt32(tmp) + elemList[j]).ToString();
                        MyElemDisplay[row][i][j].text = tmp;
                    }
                }
            }
            else
            {
                for (int i = 0; i < EnemyElemDisplay[row].Length; i++)
                {
                    for (int j = 1; j <= 4; ++j)
                    {
                        string tmp = (EnemyElemDisplay[row][i][j].text.ToString());
                        tmp = (Convert.ToInt32(tmp) + elemList[j]).ToString();
                        EnemyElemDisplay[row][i][j].text = tmp;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < MyTacticSlots[row/2].Length; i++)
            {
                MyTacticSlots[row/2][i].AddElementsByArray(elemList);
            }
        }
    }

    //slots
    private void SlotsInit()
    {
        MyTacticSlots = new GridSlot[BattleHeight][];
        MyBattleSlots = new GridSlot[BattleHeight][];
        EnemyBattleSlots = new GridSlot[BattleHeight][];

        for (int i = 0; i < MyTacticSlots.Length; i++)
        {
            MyTacticSlots[i] = new GridSlot[BattleWidth];
            for (int j = 0; j < MyTacticSlots[i].Length; j++)
            {
                MyTacticSlots[i][j] = ObjectPool.GetObject(ObjectPool.GridSlot.TacticSlot).GetComponent<GridSlot>();
                MyTacticSlots[i][j].Location = new Vector2Int(i, j);
                MyTacticSlots[i][j].gameObject.transform.SetParent(MyTacticGrid.transform, false);
                MyTacticSlots[i][j].gameObject.transform.localPosition = Vector3.zero;//?
                MyTacticSlots[i][j].gameObject.SetActive(true);//激活
                MyTacticSlots[i][j].TacticInfoWindow = TacticInfoWindow;
                MyTacticSlots[i][j].Init(Occupation.Occup.Base0);
                MyTacticSlots[i][j].IsTactic = true;
                MyTacticSlots[i][j].IsMine = true;
                MyTacticSlots[i][j].data.RefreshData(MyTacticSlots[i][j].CurOccup);
            }
        }
        for (int i = 0; i < MyBattleSlots.Length; i++)
        {
            MyBattleSlots[i] = new GridSlot[BattleWidth];
            for (int j = 0; j < MyBattleSlots[i].Length; j++)
            {
                MyBattleSlots[i][j] = ObjectPool.GetObject(ObjectPool.GridSlot.BattleSlot).GetComponent<GridSlot>();
                MyBattleSlots[i][j].Location = new Vector2Int(i, j);
                MyBattleSlots[i][j].IsMine = true;
                MyBattleSlots[i][j].gameObject.transform.SetParent(MyBattleGrid.transform, false);
                MyBattleSlots[i][j].gameObject.transform.localPosition = Vector3.zero;//?
                MyBattleSlots[i][j].gameObject.SetActive(true);//激活
            }
        }
        for (int i = 0; i < EnemyBattleSlots.Length; i++)
        {
            EnemyBattleSlots[i] = new GridSlot[BattleWidth];
            for (int j = 0; j < MyBattleSlots[i].Length; j++)
            {
                EnemyBattleSlots[i][j] = ObjectPool.GetObject(ObjectPool.GridSlot.BattleSlot).GetComponent<GridSlot>();
                EnemyBattleSlots[i][j].Location = new Vector2Int(i, j);
                EnemyBattleSlots[i][j].IsMine = false;
                EnemyBattleSlots[i][j].gameObject.transform.SetParent(EnemyBattleGrid.transform, false);
                EnemyBattleSlots[i][j].gameObject.transform.localPosition = Vector3.zero;//?
                EnemyBattleSlots[i][j].gameObject.SetActive(true);//激活
            }
        }
    }

    IEnumerator TacticToBattle()
    {
        foreach (var piece in Board.instance.PiecesAvailable)
        {
            piece.SetActive(false);
        }
        SetUIInteraction(false);
        yield return StartCoroutine(TacticCharaDisappear());
        //Debug.Log("disappear");
        StartCoroutine(SlideMove(TacticUI,TacticMoveDistance.y,Vector2.down, 50,1f));
        StartCoroutine(SlideMove(TacticContent,TacticMoveDistance.y, Vector2.down, 50, 1f));
        //Debug.Log("tactic2ok");
        StartCoroutine(SlideMove(BattleUI, BattleMoveDistance.y, Vector2.down, 50, 1f));
        yield return StartCoroutine(SlideMove(BattleContent, BattleMoveDistance.y,Vector2.down,50,1f));
        //Debug.Log("battleok");

        //Enemy
        List<int> EnemyIndex = new List<int>();
        for (int i = 0; i < BattleHeight; i++)//01234
        {
            EnemyIndex.Add(i);
        }
        EnemyIndex = RandomSort(EnemyIndex);
        Debug.Log("Random index "+ EnemyIndex[0]+ EnemyIndex[1]+ EnemyIndex[2]+ EnemyIndex[3]+ EnemyIndex[4]);

        //
        for (int i = 0; i < MyTacticSlots.Length; i++)
        {
            for (int j = 0; j < MyTacticSlots[i].Length; j++)
            {
                //My
                MyBattleSlots[i][j].TacticInfoWindow = BattleInfoWindow;                                     //window
                MyBattleSlots[i][j].Init(MyTacticSlots[i][j].CurOccup.occup, MyTacticSlots[i][j].data);      //data occup
                Debug.Log("init "+ MyTacticSlots[i][j].CurOccup.occup + " at "+ MyBattleSlots[i][j].Location);
                //Enemy
                if (IsFirstEnemyGen)
                {
                    EnemyBattleSlots[EnemyIndex[i]][j].TacticInfoWindow = BattleInfoWindow;
                    CharaData tmpData = new CharaData(MyTacticSlots[i][j].data, EnemyBattleSlots[EnemyIndex[i]][j]);
                    EnemyBattleSlots[EnemyIndex[i]][j].Init(MyTacticSlots[i][j].CurOccup.occup, tmpData);
                }
                else
                {
                    EnemyBattleSlots[EnemyIndex[i]][j].TacticInfoWindow = BattleInfoWindow;
                    EnemyBattleSlots[EnemyIndex[i]][j].AddElementsByArray(RoundsAllElementAdd[EnemyIndex[i]]);                                                                                                                                                                                                                                                                                            
                }
                MyBattleSlots[i][j].CurOccup.Hide();
                EnemyBattleSlots[EnemyIndex[i]][j].CurOccup.Hide();
            }
        }

        if (IsFirstEnemyGen)
        {
            IsFirstEnemyGen = false;
        }
        yield return StartCoroutine(BattleCharaAppear());
        IsTacticWindowAvailable = true;

        yield return new WaitForSeconds(0.5f);
        BattleField.SetActive(true);

    }

    public void BattleOver()
    {
        StartCoroutine(BattleSystem.instance.BattleToTactic());
    }

    public IEnumerator BattleToTactic()
    {
        for (int i = 0; i < RoundsAllElementAdd.Length; i++)        //unfixed
        {
            for (int j = 0; j < RoundsAllElementAdd[i].Count; j++)
            {
                RoundsAllElementAdd[i][j] = 0;
            }
        }
        IsTacticWindowAvailable = false; 
        yield return StartCoroutine(BattleCharaDisappear());
        //Debug.Log("disappear");
        for (int i = 0; i < MyTacticSlots.Length; i++)
        {
            for (int j = 0; j < MyTacticSlots[i].Length; j++)
            {
                //My
                //MyBattleSlots[i][j].TacticInfoWindow = BattleInfoWindow;
                //window
                if (!MyBattleSlots[i][j].IsDead)
                {
                    MyTacticSlots[i][j].ExchangeData(MyBattleSlots[i][j],true);
                    //MyBattleSlots[i][j].ExchangeData(MyTacticSlots[i][j],true);

                    MyTacticSlots[i][j].CurOccup.Hide();
                    Debug.Log("exchange at "+i +" "+ j);
                }
                else
                {
                    Debug.Log("Regenerate New Base Character At: "+MyTacticSlots[i][j].Location);
                    MyTacticSlots[i][j].Clear(true);
                    MyTacticSlots[i][j].EnemyReGen();
                }
                MyBattleSlots[i][j].Clear(false);    //data occup


                //Enemy
                //EnemyBattleSlots[i][j].TacticInfoWindow = BattleInfoWindow;
                if (EnemyBattleSlots[i][j].IsDead)
                {
                    EnemyBattleSlots[i][j].EnemyReGen();
                }
                EnemyBattleSlots[i][j].CurOccup.Hide();

                //EnemyBattleSlots[i][j].Clear();    //data occup
            }
        }
        StartCoroutine(SlideMove(BattleUI, BattleMoveDistance.y, Vector2.up, 50, 1f));
        StartCoroutine(SlideMove(BattleContent, BattleMoveDistance.y, Vector2.up, 50, 1f));
        Debug.Log("battleok");
        StartCoroutine(SlideMove(TacticUI, TacticMoveDistance.y, Vector2.up, 50, 1f));
        yield return StartCoroutine(SlideMove(TacticContent, TacticMoveDistance.y, Vector2.up, 50, 1f));
        Debug.Log("tacticok");
        BattleField.SetActive(false);
        yield return StartCoroutine(TacticCharaAppear());
        foreach (var piece in Board.instance.PiecesAvailable)
        {
            piece.SetActive(true);
        }
        SetUIInteraction(true);
    }

    private List<T> RandomSort<T>(List<T> list)
    {
        List<T> result = new List<T>();
        while (list.Count > 0)
        {
            int index = Random.Range(0, list.Count);
            result.Add(list[index]);
            list.RemoveAt(index);
        }
        return result;
    }

    IEnumerator TacticCharaDisappear()
    {
        //int index = 0;
        for (int i = 0; i < MyTacticSlots.Length; i++)
        {
            for (int j = 0; j < MyTacticSlots[i].Length; j++)
            {
                GameObject tmp = ObjectPool.GetObject(MyTacticSlots[i][j].CurOccup.TeleportVFX);
                tmp.transform.SetParent(MyTacticSlots[i][j].CurOccup.Particles,false); 
                tmp.SetActive(true);
                yield return new WaitForSeconds(CharaDisappearTime);
                MyTacticSlots[i][j].CurOccup.Hide();
            }
        }
    }

    IEnumerator TacticCharaAppear()
    {
        //int index = 0;
        for (int i = 0; i < MyTacticSlots.Length; i++)
        {
            for (int j = 0; j < MyTacticSlots[i].Length; j++)
            {
                GameObject tmp = ObjectPool.GetObject(MyTacticSlots[i][j].CurOccup.TeleportVFX);
                tmp.transform.SetParent(MyTacticSlots[i][j].CurOccup.Particles, false);
                tmp.SetActive(true);
                yield return new WaitForSeconds(CharaAppearTime);
                MyTacticSlots[i][j].CurOccup.Show(true,true);
            }
        }
    }

    IEnumerator BattleCharaAppear()
    {
        //int index = 0;
        for (int i = 0; i < MyBattleSlots.Length; i++)
        {
            for (int j = 0; j < MyBattleSlots[i].Length; j++)
            {
                GameObject tmp1 = ObjectPool.GetObject(MyBattleSlots[i][j].CurOccup.TeleportVFX);
                tmp1.transform.SetParent(MyBattleSlots[i][j].CurOccup.Particles, false);
                tmp1.SetActive(true);
                GameObject tmp2 = ObjectPool.GetObject(EnemyBattleSlots[i][j].CurOccup.TeleportVFX);
                tmp2.transform.SetParent(EnemyBattleSlots[i][j].CurOccup.Particles, false);
                tmp2.SetActive(true);
                yield return new WaitForSeconds(CharaAppearTime);
                //MyBattleSlots[BattleHeight -1 -j][i].CurOccup.Show();
                MyBattleSlots[i][j].CurOccup.Show(false,true);
                //EnemyBattleSlots[BattleHeight - 1 - j][i].CurOccup.Show();
                EnemyBattleSlots[i][j].CurOccup.Show(false,false);
                
            }
        }
        //MyBattleSlots[1][3].CurOccup.Hide();
    }

    IEnumerator BattleCharaDisappear()
    {
        //int index = 0;
        for (int i = 0; i < MyBattleSlots.Length; i++)
        {
            for (int j = 0; j < MyBattleSlots[i].Length; j++)
            {

                if (!MyBattleSlots[i][j].IsDead)
                {
                    GameObject tmp1 = ObjectPool.GetObject(MyBattleSlots[i][j].CurOccup.TeleportVFX);
                    tmp1.transform.SetParent(MyBattleSlots[i][j].CurOccup.Particles, false);
                    tmp1.SetActive(true);
                }
                //MyBattleSlots[BattleHeight -1 -j][i].CurOccup.Show();
                //EnemyBattleSlots[BattleHeight - 1 - j][i].CurOccup.Show();
                if (!EnemyBattleSlots[i][j].IsDead)
                {
                    GameObject tmp2 = ObjectPool.GetObject(EnemyBattleSlots[i][j].CurOccup.TeleportVFX);
                    tmp2.transform.SetParent(EnemyBattleSlots[i][j].CurOccup.Particles, false);
                    tmp2.SetActive(true);
                }

                yield return new WaitForSeconds(CharaDisappearTime);
                if (!MyBattleSlots[i][j].IsDead)
                {
                    MyBattleSlots[i][j].CurOccup.Hide();
                }
                if (!EnemyBattleSlots[i][j].IsDead)
                {
                    EnemyBattleSlots[i][j].CurOccup.Hide();
                }
            }
        }
        //MyBattleSlots[1][3].CurOccup.Hide();
    }

    public void T_TacticCharaDisappear()
    {
        //int index = 0;
        for (int i = 0; i < MyTacticSlots.Length; i++)
        {
            for (int j = 0; j < MyTacticSlots[i].Length; j++)
            {

                //MyTacticSlots[i][j] =  ;
            }
        }
    }

    public void T_TacticCharaAppear()
    {
        //int index = 0;
        for (int i = 0; i < MyTacticSlots.Length; i++)
        {
            for (int j = 0; j < MyTacticSlots[i].Length; j++)
            {

                //MyTacticSlots[i][j] =  ;
            }
        }
    }

    IEnumerator OldSlideMove(RectTransform trans,Vector3 dis,float time,float m_time)
    {
        Vector3 hasMoved = Vector3.zero;
        while(true)
        {
            Vector3 tmp = dis * (m_time / time);

            if (dis.y < 0)
            {
                if (dis.y - (tmp + hasMoved).y > 0)
                {
                    trans.anchoredPosition = new Vector2(trans.anchoredPosition.x - (dis.x - (tmp + hasMoved).x),
                        trans.anchoredPosition.y - (dis.y - (tmp + hasMoved).y));
                    break;
                }
                else
                {
                    trans.anchoredPosition = new Vector2(trans.anchoredPosition.x - tmp.x
                        , trans.anchoredPosition.y - tmp.y);
                    hasMoved += tmp;
                }
            }
            else
            {
                if (dis.y - (tmp + hasMoved).y < 0)
                {
                    trans.anchoredPosition = new Vector2(trans.anchoredPosition.x - (dis.x - (tmp + hasMoved).x),
                        trans.anchoredPosition.y - (dis.y - (tmp + hasMoved).y));
                    break;
                }
                else
                {
                    trans.anchoredPosition = new Vector2(trans.anchoredPosition.x - tmp.x
                        , trans.anchoredPosition.y - tmp.y);
                    hasMoved += tmp;
                }
            }

            Debug.Log("hasMove " + hasMoved.y);

            yield return new WaitForSeconds(m_time);
        }
        
    }

    IEnumerator SlideMove(RectTransform rect,float distance,Vector2 direct,int parts,float time)
    {
        for (int i = 0; i < parts; i++)
        {
            if (direct == Vector2.down)
            {
                rect.anchoredPosition = new Vector2(
                    rect.anchoredPosition.x,
                    rect.anchoredPosition.y - distance / parts
                );
            }
            else if (direct == Vector2.up)
            {
                rect.anchoredPosition = new Vector2(
                    rect.anchoredPosition.x,
                    rect.anchoredPosition.y + distance / parts
                );
            }
            //Debug.Log("Cur Pos " + rect.anchoredPosition);
            yield return new WaitForSeconds(time / parts);
        }
    }

    public void SetUIInteraction(bool isok)
    {
        if (!isok)
        {
            IsTacticWindowAvailable = false;
            TacticInfoWindow.SetActive(false);
            LButton.GetComponentInChildren<Button>().interactable = false;
            RButton.GetComponentInChildren<Button>().interactable = false;
            EButton.GetComponentInChildren<Button>().interactable = false;
        }
        else
        {
            IsTacticWindowAvailable = true;
            LButton.GetComponentInChildren<Button>().interactable = true;
            RButton.GetComponentInChildren<Button>().interactable = true;
            EButton.GetComponentInChildren<Button>().interactable = true;
        }
    }
}
