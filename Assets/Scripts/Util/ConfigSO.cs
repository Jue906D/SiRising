using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigSO", menuName = "")]
public class ConfigSO : ScriptableObject
{
    [SerializeField]public int[] Water;
    [SerializeField]public int[] Grass;
    [SerializeField]public int[] Fire;
    [SerializeField]public int[] Earth;

    [SerializeField] public int[] Attack;
    [SerializeField] public int[] AttackSpeed;
    [SerializeField] public int[] Health;
    [SerializeField] public int[] Recharge;

    [SerializeField] public int[] AttackMode;

}
