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

    //��¼���Լ��ĸ�����.
    private Vector3 rawPosition;

    //Panel��ʹ��ק����ʾ�����Ϸ�.
    Transform tempParent;

    //CanvasGroup cg;
    RectTransform rt;

    //��¼���λ��.
    Vector3 MousePosition;
    Vector3 distance;

    [SerializeField]public List<Vector3> cells;
    [SerializeField] public List<TileBase> cellsTile;

    [SerializeField] public int RotationIndex;

    void Awake()
    {
        //���CanvasGroup�����������ק�Ǻ����Լ����Ӷ���⵽��������ͼƬ.
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
        Debug.Log("DragBegin��");
        //��ק��ʼʱ�����Լ��ĸ�����.
        rawPosition = transform.position;

        //��ק��ʼʱ���ü��.
        //cg.blocksRaycasts = false;

        //this.transform.SetParent(tempParent);
    }

    /// <summary>
    /// Raises the drag event.
    /// </summary>
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        //��ק��ͼƬ��������ƶ�.
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
        //��ȡ������������.
        //GameObject target = eventData.pointerEnter;
        //����ܼ�⵽����.
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
        //��ק����ʱ���ü��.
        //cg.blocksRaycasts = true;

        //����Ƿ����ƴͼ.
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
