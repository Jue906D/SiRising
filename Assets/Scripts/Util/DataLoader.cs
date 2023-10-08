using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    public static DataLoader instance;
    [SerializeField] public ConfigSO config;

    void Awake()
    {
        instance = this;
    } 
}
