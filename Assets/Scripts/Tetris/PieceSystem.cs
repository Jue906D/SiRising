using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System;

public class PieceSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //Piece
    [SerializeField] public Tetromino tetro;

    public Vector2 Offset;

    //记录下自己的父物体.
    private Vector3 rawPosition;

    //Panel，使拖拽是显示在最上方.
    Transform tempParent;

    //CanvasGroup cg;
    RectTransform rt;

    //记录鼠标位置.
    Vector3 MousePosition;
    Vector3 distance;

    [SerializeField]public List<Vector3> cells;
    [SerializeField] public List<TileBase> cellsTile;

    [SerializeField] public int RotationIndex;

    void Awake()
    {
        //添加CanvasGroup组件用于在拖拽是忽略自己，从而检测到被交换的图片.
        //cg = this.gameObject.AddComponent<CanvasGroup>();

        //rt = this.GetComponent<RectTransform>();
        

        //tempParent = GameObject.Find("Canvas").transform;

        //cells = new List<Vector3>(cells.Count);

        //cellsTile = new List<TileBase>(cellsTile.Count);
    }

    void OnEnable()
    {
        RotationIndex = 0;
    }

    void Update()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Rotate(1);
        }
    }

    private void Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = RotationIndex;

        // Rotate all of the cells using a rotation matrix
        RotationIndex = Wrap(RotationIndex + direction, 0, 4);
        if (direction > 0)
        {
            transform.Rotate(Vector3.forward, 90);
            
        }
        else
        {
            transform.Rotate(Vector3.back, 90);
        }
    }

    public static Vector3 RotatePos(Vector3 pos, string axis, float angle)
    {
        float axis_x = 0, axis_y = 0, axis_z = 0;
        switch (axis)
        {
            case "X":
                axis_x = 1;
                break;
            case "Y":
                axis_y = 1;
                break;
            case "Z":
                axis_z = 1;
                break;
        }
        float q_x, q_y, q_z, q_w;
        float radian = angle / 2 * Mathf.PI / 180;
        q_w = Mathf.Cos(radian);
        q_x = axis_x * Mathf.Sin(radian);
        q_y = axis_y * Mathf.Sin(radian);
        q_z = axis_z * Mathf.Sin(radian);
        Quaternion q = new Quaternion(q_x, q_y, q_z, q_w);
        pos = q * pos;
        return pos;
    }

    private void OnMouseDown()
    {
        //Debug.Log("begin drag");
        rawPosition = transform.position;
        distance = new Vector3(transform.position.x, transform.position.y,0) -
                   new Vector3(MousePosition.x, MousePosition.y,0);
    }

    private void OnMouseDrag()
    {
        transform.position = new Vector3(MousePosition.x,MousePosition.y,0) + distance;
    }

    private void OnMouseUpAsButton()
    {
        //Debug.Log("end drag");
        List<Vector3Int> cellsPosition = Board.instance.IsValidSpritePosition(cells,RotationIndex, this.transform.position);
        if (cellsPosition != null)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                //Debug.Log(cellsPosition[i]);
                Board.instance.tilemap.SetTile(cellsPosition[i], cellsTile[i]);
            }
            ObjectPool.ReturnObject(this.gameObject, tetro);
            string tmp = (RoundCount.instance.Count.text.ToString());
            tmp = (Convert.ToInt32(tmp) + 1).ToString();
            RoundCount.instance.Count.text = tmp;
            //Board.instance.SpawnPiece(Offset);
        }
        else
        {
            transform.position = rawPosition;
        }
    }
    /// <summary>
    /// Raises the begin drag event.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("DragBegin！");
        //拖拽开始时记下自己的父物体.
        rawPosition = transform.position;

        //拖拽开始时禁用检测.
        //cg.blocksRaycasts = false;

        //this.transform.SetParent(tempParent);
    }

    /// <summary>
    /// Raises the drag event.
    /// </summary>
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //推拽是图片跟随鼠标移动.
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, Input.mousePosition, eventData.enterEventCamera, out MousePosition);
        transform.position = MousePosition;
    }

    /// <summary>
    /// Raises the end drag event.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {

        List<Vector3Int> cellsPosition = Board.instance.IsValidSpritePosition(cells,RotationIndex, this.transform.position);
        if (cellsPosition != null)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                Board.instance.tilemap.SetTile(cellsPosition[i], cellsTile[i]);
            }
            ObjectPool.ReturnObject(this.gameObject,tetro);
            //Board.instance.SpawnPiece(new Vector2(0,0));
        }
        else
        {
            transform.position = rawPosition;
        }
        //获取鼠标下面的物体.
        //GameObject target = eventData.pointerEnter;
        //如果能检测到物体.
        /*
        if (target)
        {
            GameManager.SetParent(this.transform, target.transform, myParent);
        }
        else
        {
            this.transform.SetParent(myParent);
            this.transform.localPosition = Vector3.zero;
        }
        */
        //拖拽结束时启用检测.
        //cg.blocksRaycasts = true;

        //检测是否完成拼图.
        //if (GameManager.CheckWin())
        //{
        //    Debug.Log("Win!!!");
        //}

    }
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void OnDisable()
    {
        transform.Rotate(Vector3.forward, -90 * RotationIndex);
    }
}
