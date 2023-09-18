using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundCount : MonoBehaviour
{
    public static RoundCount instance;
    [SerializeField] public TextMeshProUGUI Count;
    [SerializeField] public int CurCount;
    void Awake()
    {
        instance = this;
        CurCount = 0;
    }

    void OnEnable()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCount(int count)
    {
        CurCount++;
        Count.text = CurCount.ToString();
    }
}
