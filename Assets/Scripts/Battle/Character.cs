using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private const int ElementCount = 5;
    public List<int> ElementValues;
    public bool isMine = true;

    [SerializeField] public Image CharaImg;
    private Color originColor;

    public void AddElementsByArray(List<int> elements) //元素增幅
    {
        for (int i = 0; i < elements.Count; i++)
        {
            //Debug.Log("Add " + elements[i] + " in " + i);
            ElementValues[i] += elements[i];
        }

        StartCoroutine(GetElemVFX());
    }

    /*public void Show(Vector3 pos)
    { 
        GameObject cha= ObjectPool.GetObject(ObjectPool.Minion.Knight);
        cha.transform.position = pos;
        cha.SetActive(true);
    }*/

    void Awake()
    {
        ElementValues = new List<int>(ElementCount);
        originColor = CharaImg.color;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < ElementCount; i++)
        {
            ElementValues.Add(0);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetElemVFX()
    {
        FlashColor(1f);
        yield break;
    }

    void FlashColor(float time)
    {
        //分别对应着R,G,B,透明度
        CharaImg.color = new Color(255, 255, 0, 255);
        Invoke("ResetColor", time);
    }

    void ResetColor()
    {
        CharaImg.color = originColor;
    }
}
