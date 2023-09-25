using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public ParticleSystem ps;
    [SerializeField] public ObjectPool.VFX vfx;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ps != null)
        {
            if (ps.isStopped)
            {
                ObjectPool.ReturnObject(gameObject,vfx);
            }
        }
    }
}
