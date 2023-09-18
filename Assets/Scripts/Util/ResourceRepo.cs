using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRepo : MonoBehaviour
{
    public static ResourceRepo instance;

    [SerializeField] public Sprite Fire;
    [SerializeField] public Sprite Grass;
    [SerializeField] public Sprite Earth;
    [SerializeField] public Sprite Water;


    public static Dictionary<ObjectPool.SpecialBlock, int> SpWeightDict = new Dictionary<ObjectPool.SpecialBlock, int>();
    public static Dictionary<ObjectPool.ThreeBlock, int> TrWeightDict = new Dictionary<ObjectPool.ThreeBlock, int>();
    public static Dictionary<ObjectPool.FourBlock, int> FrWeightDict = new Dictionary<ObjectPool.FourBlock, int>();
    public static Dictionary<ObjectPool.FiveBlock, int> FvWeightDict = new Dictionary<ObjectPool.FiveBlock, int>();
    public List<int> BlockWeights = new List<int>();
    
    //level 
    public List<Occupation> LevelPriority = new List<Occupation>();
    

    [SerializeField] public int TotalWeight = 0;
    //[SerializeField] public int TotalBlockWeight;

    void Awake()
    {
        instance = this;
        foreach (Occupation.Occup occup in Enum.GetValues(typeof(Occupation.Occup)))
        {
            Occupation tmp = ObjectPool.GetPrefabByEnum(occup).GetComponent<Occupation>();//¿É¸Ä½ø
            bool hasFind = false;
            for (int i = 0; i < LevelPriority.Count; i++)
            {
                if (LevelPriority[i].Priority < tmp.Priority)
                {
                    LevelPriority.Insert(i,tmp);
                    Debug.Log(occup.ToString() + " add occup at " + (i + 1));
                    hasFind = true;
                    break;
                }
            }
            if (!hasFind)
            {
                LevelPriority.Add(tmp);
                Debug.Log(occup.ToString() + " add occup at " + (LevelPriority.Count));
            }
        }
    }

    public  void CalTotalWeight()
    {
        TotalWeight = 0;
        {
            foreach (var weight in SpWeightDict)
            {
                Debug.Log(weight.Key.ToString() + weight.Value);
                TotalWeight += weight.Value;
                BlockWeights.Add(weight.Value);
            }
            foreach (var weight in TrWeightDict)
            {
                Debug.Log(weight.Key.ToString() + weight.Value);
                TotalWeight += weight.Value;
                BlockWeights.Add(weight.Value);
            }
            foreach (var weight in FrWeightDict)
            {
                Debug.Log(weight.Key.ToString() + weight.Value);
                TotalWeight += weight.Value;
                BlockWeights.Add(weight.Value);
            }
            foreach (var weight in FvWeightDict)
            {
                Debug.Log(weight.Key.ToString() + weight.Value);
                TotalWeight += weight.Value;
                BlockWeights.Add(weight.Value);
            }
        }
    }

    void Update()
    {
        //TotalBlockWeight = TotalWeight;
    }

    public void LevelUpSearch(int level)
    {

    }
}
