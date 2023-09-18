using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationButton : MonoBehaviour
{
    public void OnClickL()
    {
        if (!Board.instance.isInRotation)
        {
            Board.instance.isInRotation = true;
        }
        Board.instance.RotateProcess(1);
        
        //RoundCount.instance.AddCount(1);
    }

    public void OnClickR()
    {
        if (!Board.instance.isInRotation)
        {
            Board.instance.isInRotation = true;
        }
        Board.instance.RotateProcess(-1);
        //RoundCount.instance.AddCount(1);
    }

}
