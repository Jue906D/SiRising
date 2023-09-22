using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class TimeCount : MonoBehaviour
{
    public static TimeCount instance;
    [SerializeField] public TextMeshProUGUI Count;
    [SerializeField] public float CurCount;
    void Awake()
    {
        instance = this;
        //CurCount = BattleField.instance.BattleTime;
    }

    void OnEnable()
    {
        CurCount = BattleField.instance.BattleTime;
    }

    // Update is called once per frame
    void Update()
    {
        CurCount -= Time.deltaTime;
        ShowCount();
    }

    public void ShowCount()
    {
        Count.text = ((int)CurCount).ToString();
    }
}
