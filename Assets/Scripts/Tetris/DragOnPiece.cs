using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System;

public class DragOnPiece : MonoBehaviour//, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    [SerializeField] public GameObject[] Parts;
    [SerializeField] public TileBase SingleTile = null;
    [SerializeField]public List<Vector3> cells;
    [SerializeField] public List<TileBase> cellsTile;

    [SerializeField] public int RotationIndex;

    //[SerializeField] public bool isInGame = true;
    [SerializeField] public int weight = 1;
    [SerializeField] public int FireWeight = 25;
    [SerializeField] public int GrassWeight = 25;
    [SerializeField] public int WaterWeight = 25;
    [SerializeField] public int EarthWeight = 25;

    public bool isWeighted = false;
    public bool isDragging = false;
    [SerializeField]public Element elem;

    public int index;
    


    void Awake()
    {
        //添加CanvasGroup组件用于在拖拽是忽略自己，从而检测到被交换的图片.
        //cg = this.gameObject.AddComponent<CanvasGroup>();

        //rt = this.GetComponent<RectTransform>();


        //tempParent = GameObject.Find("Canvas").transform;

        //cells = new List<Vector3>(cells.Count);

        //cellsTile = new List<TileBase>(cellsTile.Count
        if (Parts.Length != 0 && Parts.Length > cells.Count)
        {
            for (int i = 0; i < Parts.Length; ++i)
            {
                if (i < cells.Count)
                {
                    cells[i] = Parts[i].transform.localPosition;
                }
                else
                {
                    cells.Add(Parts[i].transform.localPosition);
                }
            }
        }
        //if(isInGame)
        //   ResourceRepo.WeightDict.TryAdd(gameObject.name,weight);
    }

    void OnEnable()
    {
        RotationIndex = 0;
        isDragging = false;
        if (SingleTile != null)
        {
            for (int i = 0; i < Parts.Length; ++i)
            {
                if (i < cellsTile.Count)
                {
                    cellsTile[i] = SingleTile;
                }
                else
                {
                    cellsTile.Add(SingleTile);
                }
            }
        }
    }

    void Update()
    {
        MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.RightControl))
        {
            //Board.instance.RotateBoard(1);
            //Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightShift))
        {
            //Rotate(1);
        }
    }

    public void SetColor(Element element)
    {
        //Debug.Log("SetColor "+element.ToString());
        if (element == Element.None)
        {
            element = Element.Fire;
        }
        SingleTile = Board.instance.ElemTileDict[element];
        for (int i = 0; i < cellsTile.Count; ++i)
        {
            cellsTile[i] = SingleTile;
        }
        for (int i = 0; i < Parts.Length; i++)
        {
            SpriteRenderer sp = Parts[i].GetComponent<SpriteRenderer>();
            elem = element;
            switch (elem)
            {
                default:
                case Element.Fire:
                    sp.sprite = ResourceRepo.instance.Fire;
                    break;
                case Element.Grass:
                    sp.sprite = ResourceRepo.instance.Grass;
                    break;
                case Element.Water:
                    sp.sprite = ResourceRepo.instance.Water;
                    break;
                case Element.Earth:
                    sp.sprite = ResourceRepo.instance.Earth;
                    break;
            }
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
        if (IsPointerOverGameObject(Input.mousePosition))
        {
            //Debug.Log("Drag Start");
            isDragging = true;
            //Debug.Log("begin drag");
            rawPosition = transform.position;
            Board.instance.CurDragPiece = this;
            distance = new Vector3(transform.position.x, transform.position.y, 0) -
                       new Vector3(MousePosition.x, MousePosition.y, 0);
        }
        
    }

    private void OnMouseDrag()
    {
        if (IsPointerOverGameObject(Input.mousePosition)&& isDragging)
        {
            //Debug.Log("Drag");
            transform.position = new Vector3(MousePosition.x, MousePosition.y, 0) + distance;
        }
            
    }

    private void OnMouseUp()
    {
        if (isDragging == true)
        {
            //Debug.Log("Drag Over");
            isDragging = false;

            List<Vector3Int> cellsPosition = Board.instance.IsValidSpritePosition(cells, RotationIndex, this.transform.position);
            if (cellsPosition != null)
            {
                for (int i = 0; i < cells.Count; i++)
                {
                    //Debug.Log(cellsPosition[i]);
                    Board.instance.tilemap.SetTile(cellsPosition[i], cellsTile[i]);
                }
                RoundCount.instance.AddCount(1);
                Board.instance.SpawnPiece(Offset,index);
                Board.instance.ClearLines();
                ObjectPool.ReturnObject(this.gameObject, tetro);
            }
            else
            {
                transform.position = rawPosition;
            }
            Board.instance.CurDragPiece = null;
        }
            //Debug.Log("end drag");
        
    }


    /// <summary>
    /// Raises the begin drag event.
    /// </summary>
    /*
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
            Board.instance.SpawnPiece(new Vector2(0,0));
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
        
        //拖拽结束时启用检测.
        //cg.blocksRaycasts = true;

        //检测是否完成拼图.
        //if (GameManager.CheckWin())
        //{
        //    Debug.Log("Win!!!");
        //}

    }
    */
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

    /// <summary>
    /// 传入 Input.mousePosition
    /// </summary>
    /// <param name="screenPosition"></param>
    /// <returns></returns>
    public bool IsPointerOverGameObject(Vector2 screenPosition)
    {
        //实例化点击事件
        PointerEventData eventDataCurrentPosition = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        //将点击位置的屏幕坐标赋值给点击事件
        eventDataCurrentPosition.position = new Vector2(screenPosition.x, screenPosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        //向点击处发射射线
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);


        return results.Count <= 1;

    }

    private void OnDisable()
    {
        transform.Rotate(Vector3.forward, -90 * RotationIndex);
    }
}
