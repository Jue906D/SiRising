using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public Dictionary<string, Queue<GameObject>> ObjDict = new Dictionary<string, Queue<GameObject>>();

    [SerializeField] public int MinionPoolCount = 100;
    [SerializeField] public int UIPoolCount = 100;
    [SerializeField] public int SlotCount = 100;

    //[SerializeField] GameObject[] characterPrefab;
    [SerializeField] GameObject Pope;
    [SerializeField] GameObject Knight;
    [SerializeField] GameObject Prist;
    [SerializeField] GameObject Celestial;
    [SerializeField] GameObject Poet;
    [SerializeField] GameObject Errant;

    [SerializeField] GameObject ElemValue;

    [SerializeField] GameObject PieceI;
    [SerializeField] GameObject PieceJ;
    [SerializeField] GameObject PieceL;
    [SerializeField] GameObject PieceO;
    [SerializeField] GameObject PieceS;
    [SerializeField] GameObject PieceT;
    [SerializeField] GameObject PieceZ;

    [SerializeField] GameObject One;

    [SerializeField] GameObject VerticalI3;
    [SerializeField] GameObject HorizontalI3;
    [SerializeField] GameObject UpLeftCorner3;
    [SerializeField] GameObject UpRightCorner3;
    [SerializeField] GameObject DownLeftCorner3;
    [SerializeField] GameObject DownRightCorner3;

    [SerializeField] GameObject VerticalI4;
    [SerializeField] GameObject HorizontalI4;
    [SerializeField] GameObject UpT4;
    [SerializeField] GameObject DownT4;
    [SerializeField] GameObject LeftT4;
    [SerializeField] GameObject RightT4;
    [SerializeField] GameObject L41;
    [SerializeField] GameObject L42;
    [SerializeField] GameObject L43;
    [SerializeField] GameObject L44;
    [SerializeField] GameObject Cube4;
    [SerializeField] GameObject S41;
    [SerializeField] GameObject S42;
    [SerializeField] GameObject S43;
    [SerializeField] GameObject S44;
    [SerializeField] GameObject J41;
    [SerializeField] GameObject J42;
    [SerializeField] GameObject J43;
    [SerializeField] GameObject J44;

    [SerializeField] GameObject Cross5;
    [SerializeField] GameObject UpT5;
    [SerializeField] GameObject DownT5;
    [SerializeField] GameObject LeftT5;
    [SerializeField] GameObject RightT5;
    [SerializeField] GameObject UpLeftCorner5;
    [SerializeField] GameObject UpRightCorner5;
    [SerializeField] GameObject DownLeftCorner5;
    [SerializeField] GameObject DownRightCorner5;

    //slot
    [SerializeField] GameObject TacticSlot;
    [SerializeField] GameObject BattleSlot;

    //occup
    [SerializeField] GameObject Base0;    //°×°å

    [SerializeField] GameObject ApRp1_Sword;//¹¥»Ø½£1
    [SerializeField] GameObject Ba1_Sword;  //Æ½ºâ½£1
    [SerializeField] GameObject HpSp1_Sword;//ÑªËÙ½£1
    [SerializeField] GameObject HpSp1_Shield;//ÑªËÙ¶Ü1
    [SerializeField] GameObject Ba_core;   //Æ½ºâ1

    [SerializeField] GameObject ApRp2_Sword;   //¹¥»Ø½£2

    [SerializeField] GameObject UA_Explosion_1;
    [SerializeField] GameObject Re_Recover_Plus;
    [SerializeField] GameObject Tp_Teleport_1;


    public enum GridSlot
    {
        TacticSlot,
        BattleSlot
    }

    public enum Minion
    {
        Pope,
        Knight,
        Prist,
        Celestial,
        Poet,
        Errant,
    }

    public enum UI
    {
        ElemValue
    }

    public enum SpecialBlock
    {
        One
    }

    public enum ThreeBlock  //6 = 2I + 4C
    {
        VerticalI3,
        HorizontalI3,
        UpLeftCorner3,
        UpRightCorner3,
        DownLeftCorner3,
        DownRightCorner3
    }

    public enum FourBlock   //15 = 2I + 4T +4L +4S +O
    {
        VerticalI4,
        HorizontalI4,
        UpT4,
        DownT4,
        LeftT4,
        RightT4,
        L41,
        L42,
        L43,
        L44,
        Cube4,
        S41,
        S42,
        S43,
        S44,
        J41,
        J42,
        J43,
        J44
    }

    public enum FiveBlock //9 = 1Cross+4C+4T
    {
        Cross5,
        UpT5,
        DownT5,
        LeftT5,
        RightT5,
        UpLeftCorner5,
        UpRightCorner5,
        DownLeftCorner5,
        DownRightCorner5
    }


    public enum VFX
    {
        UA_Explosion_1,
        Re_Recover_Plus,
        Tp_Teleport_1
    }

    void Awake()
    {
        instance = this;
        foreach (Minion minion in Enum.GetValues(typeof(Minion)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < MinionPoolCount; j++)
            {
                newQue.Enqueue(CreateObject(minion));
            }
            ObjDict.Add(minion.ToString(), newQue);
            //Debug.Log(minion.ToString());
        }
        foreach (UI ui in Enum.GetValues(typeof(UI)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < UIPoolCount; j++)
            {
                newQue.Enqueue(CreateObject(ui));
            }
            ObjDict.Add(ui.ToString(), newQue);
            //Debug.Log(minion.ToString());
        }

        foreach (Tetromino tetro in Enum.GetValues(typeof(Tetromino)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < 3; j++)
            {
                newQue.Enqueue(CreateObject(tetro));
            }
            ObjDict.Add(tetro.ToString(), newQue);
            //Debug.Log(minion.ToString());
        }

        foreach (SpecialBlock specialBlock in Enum.GetValues(typeof(SpecialBlock)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < 3; j++)
            {
                newQue.Enqueue(CreateObject(specialBlock));
            }
            ObjDict.Add(specialBlock.ToString(), newQue);
            ResourceRepo.SpWeightDict.Add(specialBlock,GetPrefabByEnum(specialBlock).GetComponent<DragOnPiece>().weight);
            //Debug.Log(minion.ToString());
        }

        foreach (ThreeBlock threeBlock in Enum.GetValues(typeof(ThreeBlock)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < 3; j++)
            {
                newQue.Enqueue(CreateObject(threeBlock));
            }
            ObjDict.Add(threeBlock.ToString(), newQue);
            ResourceRepo.TrWeightDict.Add(threeBlock,GetPrefabByEnum(threeBlock).GetComponent<DragOnPiece>().weight);
            //Debug.Log(minion.ToString());
        }

        foreach (FourBlock fourBlock in Enum.GetValues(typeof(FourBlock)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < 3; j++)
            {
                newQue.Enqueue(CreateObject(fourBlock));
            }
            ObjDict.Add(fourBlock.ToString(), newQue);
            ResourceRepo.FrWeightDict.Add(fourBlock,GetPrefabByEnum(fourBlock).GetComponent<DragOnPiece>().weight);
            //Debug.Log(minion.ToString());
        }

        foreach (FiveBlock fiveBlock in Enum.GetValues(typeof(FiveBlock)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < 3; j++)
            {
                newQue.Enqueue(CreateObject(fiveBlock));
            }
            ObjDict.Add(fiveBlock.ToString(), newQue);
            ResourceRepo.FvWeightDict.Add(fiveBlock,GetPrefabByEnum(fiveBlock).GetComponent<DragOnPiece>().weight);
            //Debug.Log(minion.ToString());
        }

        foreach (GridSlot gridSlot in Enum.GetValues(typeof(GridSlot)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < SlotCount; j++)
            {
                newQue.Enqueue(CreateObject(gridSlot));
            }
            ObjDict.Add(gridSlot.ToString(), newQue);
        }

        foreach (Occupation.Occup occup in Enum.GetValues(typeof(Occupation.Occup)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < SlotCount; j++)
            {
                newQue.Enqueue(CreateObject(occup));
            }
            ObjDict.Add(occup.ToString(), newQue);
        }

        foreach (VFX vfx in Enum.GetValues(typeof(VFX)))
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < SlotCount; j++)
            {
                newQue.Enqueue(CreateObject(vfx));
            }
            ObjDict.Add(vfx.ToString(), newQue);
        }
        //characterPrefab = new GameObject[characterPrefab.Length];
        /*for (int i = 0; i < characterPrefab.Length; i++)
        {
            Queue<GameObject> newQue = new Queue<GameObject>();
            for (int j = 0; j < PoolCount; j++)
            {
                newQue.Enqueue(Instantiate(instance.characterPrefab[i]));
            }
            ObjDict.Add(characterPrefab[i].GetType().ToString(),newQue);
            PrefabDict.Add(characterPrefab[i].GetType().ToString(),i);
            Debug.Log(characterPrefab[i].GetType().ToString());
        }*/

    }

    static GameObject CreateObject<T>(T type)
    {
        GameObject newObject;
        //bool isParentPlayer = false;
        switch (type)
        {
            default:
            case Tetromino.O:
                newObject = Instantiate(instance.PieceO);
                break;
            case Tetromino.T:
                newObject = Instantiate(instance.PieceT);
                break;
            case Tetromino.I:
                newObject = Instantiate(instance.PieceI);
                break;
            case Tetromino.J:
                newObject = Instantiate(instance.PieceJ);
                break;
            case Tetromino.L:
                newObject = Instantiate(instance.PieceL);
                break;
            case Tetromino.S:
                newObject = Instantiate(instance.PieceS);
                break;
            case Tetromino.Z:
                newObject = Instantiate(instance.PieceZ);
                break;
            case Minion.Pope:
                newObject = Instantiate(instance.Pope);
                break;
            case Minion.Knight:
                newObject = Instantiate(instance.Knight);
                break;
            case Minion.Prist:
                newObject = Instantiate(instance.Prist);
                break;
            case Minion.Celestial:
                newObject = Instantiate(instance.Celestial);
                break;
            case Minion.Poet:
                newObject = Instantiate(instance.Poet);
                break;
            case Minion.Errant:
                newObject = Instantiate(instance.Errant);
                break;
            case UI.ElemValue:
                newObject = Instantiate(instance.ElemValue);
                break;

            case SpecialBlock.One:
                newObject = Instantiate(instance.One);
                break;

            case ThreeBlock.VerticalI3:
                newObject = Instantiate(instance.VerticalI3);
                break;
            case ThreeBlock.HorizontalI3:
                newObject = Instantiate(instance.HorizontalI3);
                break;
            case ThreeBlock.UpLeftCorner3:
                newObject = Instantiate(instance.UpLeftCorner3);
                break;
            case ThreeBlock.UpRightCorner3:
                newObject = Instantiate(instance.UpRightCorner3);
                break;
            case ThreeBlock.DownLeftCorner3:
                newObject = Instantiate(instance.DownLeftCorner3);
                break;
            case ThreeBlock.DownRightCorner3:
                newObject = Instantiate(instance.UpRightCorner3);
                break;

            
            case FourBlock.VerticalI4:
                newObject = Instantiate(instance.VerticalI4);
                break;
            case FourBlock.HorizontalI4:
                newObject = Instantiate(instance.HorizontalI4);
                break;
            case FourBlock.UpT4:
                newObject = Instantiate(instance.UpT4);
                break;
            case FourBlock.DownT4:
                newObject = Instantiate(instance.DownT4);
                break;
            case FourBlock.LeftT4:
                newObject = Instantiate(instance.LeftT4);
                break;
            case FourBlock.RightT4:
                newObject = Instantiate(instance.RightT4);
                break;
            case FourBlock.L41:
                newObject = Instantiate(instance.L41);
                break;
            case FourBlock.L42:
                newObject = Instantiate(instance.L42);
                break;
            case FourBlock.L43:
                newObject = Instantiate(instance.L43);
                break;
            case FourBlock.L44:
                newObject = Instantiate(instance.L44);
                break;
            case FourBlock.Cube4:
                newObject = Instantiate(instance.Cube4);
                break;
            case FourBlock.S41:
                newObject = Instantiate(instance.S41);
                break;
            case FourBlock.S42:
                newObject = Instantiate(instance.S42);
                break;
            case FourBlock.S43:
                newObject = Instantiate(instance.S43);
                break;
            case FourBlock.S44:
                newObject = Instantiate(instance.S44);
                break;
            case FourBlock.J41:
                newObject = Instantiate(instance.J41);
                break;
            case FourBlock.J42:
                newObject = Instantiate(instance.J42);
                break;
            case FourBlock.J43:
                newObject = Instantiate(instance.J43);
                break;
            case FourBlock.J44:
                newObject = Instantiate(instance.J44);
                break;

            case FiveBlock.Cross5:
                newObject = Instantiate(instance.Cross5);
                break;
            case FiveBlock.UpT5:
                newObject = Instantiate(instance.UpT5);
                break;
            case FiveBlock.DownT5:
                newObject = Instantiate(instance.DownT5);
                break;
            case FiveBlock.LeftT5:
                newObject = Instantiate(instance.LeftT5);
                break;
            case FiveBlock.RightT5:
                newObject = Instantiate(instance.RightT5);
                break;
            case FiveBlock.UpLeftCorner5:
                newObject = Instantiate(instance.UpLeftCorner5);
                break;
            case FiveBlock.UpRightCorner5:
                newObject = Instantiate(instance.UpRightCorner5);
                break;
            case FiveBlock.DownLeftCorner5:
                newObject = Instantiate(instance.DownLeftCorner5);
                break;
            case FiveBlock.DownRightCorner5:
                newObject = Instantiate(instance.DownRightCorner5);
                break;

            case GridSlot.TacticSlot:
                newObject = Instantiate(instance.TacticSlot);
                break;
            case GridSlot.BattleSlot:
                newObject = Instantiate(instance.BattleSlot);
                break;

            case Occupation.Occup.Base0:
                newObject = Instantiate(instance.Base0);
                break;
            case Occupation.Occup.ApRp1_Sword:
                newObject = Instantiate(instance.ApRp1_Sword);
                break;
            case Occupation.Occup.Ba1_Sword:
                newObject = Instantiate(instance.Ba1_Sword);
                break;
            case Occupation.Occup.HpSp1_Sword:
                newObject = Instantiate(instance.HpSp1_Sword);
                break;
            case Occupation.Occup.HpSp1_Shield:
                newObject = Instantiate(instance.HpSp1_Shield);
                break;
            case Occupation.Occup.Ba_core:
                newObject = Instantiate(instance.Ba_core);
                break;
            case Occupation.Occup.ApRp2_Sword:
                newObject = Instantiate(instance.ApRp2_Sword);
                break;


            case VFX.UA_Explosion_1:
                newObject = Instantiate(instance.UA_Explosion_1);
                break;
            case VFX.Re_Recover_Plus:
                newObject = Instantiate(instance.Re_Recover_Plus);
                break;
            case VFX.Tp_Teleport_1:
                newObject = Instantiate(instance.Tp_Teleport_1);
                break;
        }
        newObject.SetActive(false);
        newObject.transform.SetParent(instance.transform);
        return newObject;
    }

    public static GameObject GetPrefabByEnum<T>(T type)
    {
        switch (type)
        {
            default:
                return null;
            case SpecialBlock.One:
                return instance.One;

            case ThreeBlock.VerticalI3:
                return instance.VerticalI3;
            case ThreeBlock.HorizontalI3:
                return instance.HorizontalI3;
            case ThreeBlock.UpLeftCorner3:
                return instance.UpLeftCorner3;
            case ThreeBlock.UpRightCorner3:
                return instance.UpRightCorner3;
            case ThreeBlock.DownLeftCorner3:
                return instance.DownLeftCorner3;
            case ThreeBlock.DownRightCorner3:
                return instance.UpRightCorner3;


            case FourBlock.VerticalI4:
                return instance.VerticalI4;
            case FourBlock.HorizontalI4:
                return instance.HorizontalI4;
            case FourBlock.UpT4:
                return instance.UpT4;
            case FourBlock.DownT4:
                return instance.DownT4;
            case FourBlock.LeftT4:
                return instance.LeftT4;
            case FourBlock.RightT4:
                return instance.RightT4;
            case FourBlock.L41:
                return instance.L41;
            case FourBlock.L42:
                return instance.L42;
            case FourBlock.L43:
                return instance.L43;
            case FourBlock.L44:
                return instance.L44;
            case FourBlock.Cube4:
                return instance.Cube4;
            case FourBlock.S41:
                return instance.S41;
            case FourBlock.S42:
                return instance.S42;
            case FourBlock.S43:
                return instance.S43;
            case FourBlock.S44:
                return instance.S44;
            case FourBlock.J41:
                return instance.J41;
            case FourBlock.J42:
                return instance.J42;
            case FourBlock.J43:
                return instance.J43;
            case FourBlock.J44:
                return instance.J44;

            case FiveBlock.Cross5:
                return instance.Cross5;
            case FiveBlock.UpT5:
                return instance.UpT5;
            case FiveBlock.DownT5:
                return instance.DownT5;
            case FiveBlock.LeftT5:
                return instance.LeftT5;
            case FiveBlock.RightT5:
                return instance.RightT5;
            case FiveBlock.UpLeftCorner5:
                return instance.UpLeftCorner5;
            case FiveBlock.UpRightCorner5:
                return instance.UpRightCorner5;
            case FiveBlock.DownLeftCorner5:
                return instance.DownLeftCorner5;
            case FiveBlock.DownRightCorner5:
                return instance.DownRightCorner5;

            case GridSlot.TacticSlot:
                return instance.TacticSlot;
            case GridSlot.BattleSlot:
                return instance.BattleSlot;

            case Occupation.Occup.Base0:
                return instance.Base0;
            case Occupation.Occup.ApRp1_Sword:
                return instance.ApRp1_Sword;
            case Occupation.Occup.Ba1_Sword:
                return instance.Ba1_Sword;
            case Occupation.Occup.HpSp1_Sword:
                return instance.HpSp1_Sword;
            case Occupation.Occup.HpSp1_Shield:
                return instance.HpSp1_Shield;
            case Occupation.Occup.Ba_core:
                return instance.Ba_core;
            case Occupation.Occup.ApRp2_Sword:
                return instance.ApRp2_Sword;
                  
            case VFX.UA_Explosion_1:
                return instance.UA_Explosion_1;
            case VFX.Re_Recover_Plus:
                return instance.Re_Recover_Plus;
            case VFX.Tp_Teleport_1:
                return instance.Tp_Teleport_1;
        }
    }

    public static GameObject GetObject<T>(T type,Element element)
    {
        //Debug.Log(type.ToString());
        if (instance.ObjDict[type.ToString()].Count > 0)
        {
            GameObject tmp = instance.ObjDict[type.ToString()].Dequeue();
            tmp.GetComponent<DragOnPiece>().SetColor(element);
            return tmp;
        }
        else
        {
            GameObject tmp = CreateObject(type);
            tmp.GetComponent<DragOnPiece>().SetColor(element);
            return tmp;
        }
    }

    public static GameObject GetObject<T>(T type)
    {
        //Debug.Log(type.ToString());
        if (instance.ObjDict[type.ToString()].Count > 0)
        {
            return instance.ObjDict[type.ToString()].Dequeue();
        }
        else
        {
            return CreateObject(type);
        }
    }

    public static void ReturnObject<T>(GameObject dead, T type)
    {
        dead.transform.SetParent(ObjectPool.instance.transform,false);
        dead.SetActive(false);
        instance.ObjDict[type.ToString()].Enqueue(dead);
    }
}
