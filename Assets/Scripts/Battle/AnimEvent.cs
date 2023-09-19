using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{

    void DeathEvent()
    {
        gameObject.transform.parent.parent.gameObject.GetComponent<GridSlot>().DeathFinal();
    }
}
