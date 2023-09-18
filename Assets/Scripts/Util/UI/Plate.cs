using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Plate : MonoBehaviour
{
    [SerializeField] public Element elem;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        
        DragOnPiece dop= other.gameObject.GetComponent<DragOnPiece>();
        //Debug.Log("Enter!" + dop);
        if (dop != null)
        {
            dop.SetColor(elem);
        }
        //show
    }

    void OnTriggerExit(Collider other)
    {

    }
    
    /*
    void OnMouseEnter()
    {
        if (Board.instance.CurDragPiece != null)
        {
            Board.instance.CurDragPiece.SetColor(elem);
            Debug.Log("Set Elem "+elem.ToString());
        }
        else
        {
            Debug.Log("Enter "+elem.ToString());

        }

    }
    */
}
