using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraseButton : MonoBehaviour
{
    [SerializeField]public GameObject Eraser;
    public void OnClickE()
    {
        //Board.instance.RotateProcess(1);
        //RoundCount.instance.AddCount(1);
        if (!Board.instance.isInRotation)
        {
            Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Eraser.transform.position = new Vector3(MousePosition.x, MousePosition.y, 0);
            Eraser.gameObject.SetActive(true);
        }
    }
}
