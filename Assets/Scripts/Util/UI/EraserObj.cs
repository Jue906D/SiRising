using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EraserObj : MonoBehaviour
{

    Vector3 MousePosition;
    // Start is called before the first frame update
    void OnEnable()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void Update()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(MousePosition.x, MousePosition.y, 0);
    }

    void OnMouseDown()
    {
        Vector3Int IntPosition = new Vector3Int(
            (int)Math.Round(this.transform.position.x - 0.5 - Board.instance.transform.position.x, 0),
            (int)Math.Round(this.transform.position.y - 0.5 - Board.instance.transform.position.y, 0),
            0
        );
        Board.instance.RecursionErase(IntPosition.y, IntPosition.x, Element.None);
        gameObject.SetActive(false);
    }
}
