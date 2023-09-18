using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateConfirm : MonoBehaviour
{
    [SerializeField] public GameObject HideBoard;
    public void OnClickOK()
    {
        //Board.instance.RotateProcess(1);
        //RoundCount.instance.AddCount(1);
        Board.instance.ClearLines();
        HideBoard.SetActive(false);
        if (Board.instance.isInRotation)
        {
            Board.instance.isInRotation = false;
        }
    }
}

//28 : 16
//5,10