using System;using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorkSystem: MonoBehaviour
{
    public struct Work
    {
        public float StartTime;
        public float EndTime;

        public Action WorkAction;

        public Work(float startTime, float endTime , Action workAction)
        {
            StartTime = startTime;
            EndTime = endTime;
            WorkAction = workAction;
        }

        //need progress
        public Work(float startTime, Action workAction)
        {
            StartTime = startTime;
            EndTime = -1f;
            WorkAction = workAction;
        }
    }

    public static Dictionary<string,WorkSystem> WorkSystemDict = new Dictionary<string,WorkSystem>();

    public static WorkSystem GetWorkSystem(string name)
    {
        WorkSystem tmp;
        if (WorkSystemDict.TryGetValue(name, out tmp))
        {
            return tmp;
        }
        else
        {
            Debug.LogError("! No WorkSystem named " + name);
            return null;
        }
    }

    [SerializeField]public string Name;
    [SerializeField]public List<Work> WorkList;

    void Awake()
    {
        if (!WorkSystemDict.TryAdd(Name, this))
        {
            Debug.LogError("! Duplicate name " + name);
        }
        WorkList = new List<Work>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < WorkList.Count; i++)
        {
            if (WorkList[i].StartTime <= Time.time)
            {
                WorkList[i].WorkAction.Invoke();
                WorkList.RemoveAt(i);
            }
        }
    }

    public void AddWork(float WaitTime, Action action)
    {
        float startTime = Time.time + WaitTime;
        for (int i = 0; i < WorkList.Count; i++)
        {
            if (WorkList[i].StartTime > startTime)
            {
                WorkList.Insert(i, new Work(startTime, action));
                Debug.Log(Name + " add work at " + (i+1));
                return;
            }
        }
        WorkList.Add(new Work(startTime, action));
        Debug.Log(Name + " add work at " + WorkList.Count);
        return;
    }
}
