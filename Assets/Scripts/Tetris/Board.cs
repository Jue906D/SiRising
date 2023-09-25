using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public static Board instance;

    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Vector2 offset1 = new Vector2(0, 0);
    [SerializeField] Vector2 offset2 = new Vector2(0, -3);
    [SerializeField] Vector2 offset3 = new Vector2(0, -6);
    public Tilemap tilemap { get; private set; }        //绘制tilemap
    public Piece activePiece { get; private set; }      //当前激活的块

    public TetrominoData[] tetrominoes;                 // 所有块数据
    public Vector2Int boardSize = new Vector2Int(10, 10);       //板WH
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);     //块生成位置
    public Dictionary<TileBase, Element> TileElemDict;  //tile to elem 
    public Dictionary<Element, TileBase> ElemTileDict;  //elem to tile

    [SerializeField] public GameObject HideBoard;
    public DragOnPiece CurDragPiece;

    public bool isInRotation = false;

    public List<int> elemList;

    [SerializeField] public GameObject[] PiecesAvailable;
    [SerializeField] public Vector2 DragLimitL;
    [SerializeField] public Vector2 DragLimitR;


    public RectInt Bounds {                 //块存在的有效矩形区域，即边界
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        instance = this;
        isInRotation =false;
        elemList = new List<int>(5);
        PiecesAvailable = new GameObject[3];
        tilemap = GetComponentInChildren<Tilemap>();//fix
        activePiece = GetComponentInChildren<Piece>();//fix
        TileElemDict = new Dictionary<TileBase, Element>();
        ElemTileDict = new Dictionary<Element, TileBase>();
        for (int i = 0; i < tetrominoes.Length; i++) {  //初始化原型块数据
            tetrominoes[i].Initialize();
            TileElemDict.TryAdd(tetrominoes[i].tile, tetrominoes[i].elem);
            ElemTileDict.TryAdd(tetrominoes[i].elem, tetrominoes[i].tile);
            Debug.Log(tetrominoes[i].tile+" "+ TileElemDict[tetrominoes[i].tile]);
        }
    }

    private void Start()
    {
        ResourceRepo.instance.CalTotalWeight();
        SpawnPiece(offset1,0);
        SpawnPiece(offset2,1);
        SpawnPiece(offset3, 2);
    }

    private void Update()
    {
        //ClearLines();
    }

    public void OldSpawnPiece()//生成块，自定义生成规则，自定义游戏结束规则
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];           //等概率随机原型块

        activePiece.Initialize(this, spawnPosition, data);  //原型块实例化给唯一激活块

        if (IsValidPiecePosition(activePiece, spawnPosition)) {  //生成位置阻挡，游戏结束
            Show(activePiece);
        } else {
            GameOver();
        }
    }

    public Element RandColor(int grass, int water, int fire, int earth)
    {
        int total = fire + grass + water + earth;
        int r = Random.Range(0, total);
        int t = 0;
        while (true)
        {
            t += grass;
            if (r < t)
            {
                //Debug.Log("chooseColor " + Element.Grass.ToString() + " from " + t + " in " + total);
                return Element.Grass;
            }
            t += water;
            if (r < t)
            {
                //Debug.Log("chooseColor " + Element.Water + " from " + t + " in " + total);
                return Element.Water;
            }
            t += fire;
            if (r < t)
            {
                //Debug.Log("chooseColor " + Element.Fire + " from " + t + " in " + total);
                return Element.Fire;
            }
            t += earth;
            if (r < t)
            {
                //Debug.Log("chooseColor " + Element.Earth + " from " + t + " in " + total);
                return Element.Earth;
            }
        }
    }

    public void RotateBoard(int index)
    {
        TileBase[][] tmpMap = new TileBase[boardSize.y][];
        for (int i = -boardSize.y / 2; i < boardSize.y / 2; i++)
        {
            tmpMap[i + boardSize.y / 2] = new TileBase[boardSize.x];
            for (int j = -boardSize.x / 2; j < boardSize.x / 2; j++)
            {
                Vector3Int position = new Vector3Int(j, i, 0);
                tmpMap[i + boardSize.y / 2][j + boardSize.x / 2] = tilemap.GetTile(position);
                if (tmpMap[i + boardSize.y / 2][j + boardSize.x / 2] != null)
                {
                    //Debug.Log(tmpMap[i + boardSize.y / 2][j + boardSize.x / 2].name);
                }
            }
        }
        //raw: tmpMap[i][j] (0,0)
        //new: tileMap(0,9)
        //tileMap(col,row) = tmpMap[row][boundsize.x-1-col]
        tilemap.ClearAllTiles();
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            //-5 to 4
            for (int row = Bounds.yMin; row < Bounds.yMax; row++)
            {
                //-5 to 4
                Vector3Int position = new Vector3Int(col, row, 0);
                if (index > 0)
                {
                    TileBase cur = tmpMap[boardSize.x - 1 - col - boardSize.x / 2][row + boardSize.y / 2];
                    //elemList[(int)TileElemDict[cur]]++; 
                    if (cur != null)
                    {
                        //Debug.Log("Rotate From (" + row + "," + col + ") To (" + (row + boardSize.y / 2) + "," + (boardSize.x - 1 - col - boardSize.x / 2) + ")");
                        tilemap.SetTile(position, cur);
                    }
                }
                else
                {
                    TileBase cur = tmpMap[col + boardSize.x / 2][boardSize.y - 1 - row - boardSize.y / 2];
                    //elemList[(int)TileElemDict[cur]]++; 
                    if (cur != null)
                    {
                        //Debug.Log("Rotate From (" + row + "," + col + ") To (" + (row + boardSize.y / 2) + "," + (boardSize.x - 1 - col - boardSize.x / 2) + ")");
                        tilemap.SetTile(position, cur);
                    }
                }

            }
        }
    }

    IEnumerator RotateVFX()
    {
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator RotateCoroutine(int index)
    {
        yield return StartCoroutine(RotateVFX());
        RotateBoard(index);
        //yield return StartCoroutine(RotateVFX());
        if (!HideBoard.activeSelf)
        {
            HideBoard.SetActive(true);
        }
    }

    public void RotateProcess(int index)
    {
        StartCoroutine(RotateCoroutine(index));
    }


    /*
     mirror
     tilemap.ClearAllTiles();
        for (int col = Bounds.xMin; col < Bounds.xMax; col++)
        {
            //-5 to 4
            for (int row = Bounds.yMin; row < Bounds.yMax; row++)
            {
                //-5 to 4
                Vector3Int position = new Vector3Int(col, row, 0);
                TileBase cur = tmpMap[row + boardSize.y / 2][boardSize.x - 1 - col - boardSize.x / 2];
                //elemList[(int)TileElemDict[cur]]++;
                if (cur != null)
                {
                    Debug.Log("Rotate From (" + row + "," + col + ") To (" + (row + boardSize.y / 2) + "," + (boardSize.x - 1 - col - boardSize.x / 2) + ")");
                    tilemap.SetTile(position, cur);
                }
            }
        }
     *
     */
    public void SpawnPiece(Vector2 offset,int index)//生成块，自定义生成规则，自定义游戏结束规则
    {
        int BlockRandom = RandBlock(ResourceRepo.instance.BlockWeights, ResourceRepo.instance.TotalWeight);
        int ElementRandom = Random.Range(0, 3);
        GameObject piece = null;
        while (true)
        {
            if (BlockRandom < ResourceRepo.SpWeightDict.Count)
            {
                ObjectPool.SpecialBlock sp = (ObjectPool.SpecialBlock)BlockRandom;
                Debug.Log(BlockRandom + " " + sp);
                DragOnPiece tmp = ObjectPool.GetPrefabByEnum(sp).GetComponent<DragOnPiece>();
                Element elem = RandColor(tmp.GrassWeight, tmp.WaterWeight, tmp.FireWeight, tmp.EarthWeight);
                piece = ObjectPool.GetObject((ObjectPool.SpecialBlock)BlockRandom, elem);
                break;
            }
            else
            {
                BlockRandom -= ResourceRepo.SpWeightDict.Count;
            }
            if (BlockRandom < ResourceRepo.TrWeightDict.Count)
            {
                ObjectPool.ThreeBlock tr = (ObjectPool.ThreeBlock)BlockRandom;
                //Debug.Log(BlockRandom + " " + tr);
                DragOnPiece tmp = ObjectPool.GetPrefabByEnum(tr).GetComponent<DragOnPiece>();
                Element elem = RandColor(tmp.GrassWeight, tmp.WaterWeight, tmp.FireWeight, tmp.EarthWeight);
                piece = ObjectPool.GetObject((ObjectPool.ThreeBlock)BlockRandom, elem);
                break;
            }
            else
            {
                BlockRandom -= ResourceRepo.TrWeightDict.Count;
            }
            if (BlockRandom < ResourceRepo.FrWeightDict.Count)
            {
                ObjectPool.FourBlock fr = (ObjectPool.FourBlock)BlockRandom;
                //Debug.Log(BlockRandom + " " + (ObjectPool.FourBlock)BlockRandom);
                DragOnPiece tmp = ObjectPool.GetPrefabByEnum(fr).GetComponent<DragOnPiece>();
                Element elem = RandColor(tmp.GrassWeight, tmp.WaterWeight, tmp.FireWeight, tmp.EarthWeight);
                piece = ObjectPool.GetObject((ObjectPool.FourBlock)BlockRandom, elem);
                break;
            }
            else
            {
                BlockRandom -= ResourceRepo.FrWeightDict.Count;
            }
            if (BlockRandom < ResourceRepo.FvWeightDict.Count)
            {
                ObjectPool.FiveBlock fv = (ObjectPool.FiveBlock)BlockRandom;
                //Debug.Log(BlockRandom + " " + fv);
                DragOnPiece tmp = ObjectPool.GetPrefabByEnum(fv).GetComponent<DragOnPiece>();
                Element elem = RandColor(tmp.GrassWeight, tmp.WaterWeight, tmp.FireWeight, tmp.EarthWeight);
                piece = ObjectPool.GetObject((ObjectPool.FiveBlock)BlockRandom, elem);
                break;
            }
        }
        


        piece.transform.position = spawnPosition + new Vector3(offset.x, offset.y, 0);
        piece.GetComponent<DragOnPiece>().Offset = offset;
        piece.GetComponent<DragOnPiece>().index = index;
        PiecesAvailable[index] = piece;
        piece.SetActive(true);
    }

    public int RandBlock(List<int> rate,int total)
    {
        //Debug.Log("Rate count "+rate.Count+" total "+total);
        int r = Random.Range(0, total);
        int t = 0;
        for (int i = 0; i < rate.Count; i++)
        {
            t += rate[i];
            //Debug.Log("List" + i + " " + rate[i]);
            if (r < t)
            {
                //Debug.Log("choose " + i + " from "+t+" in "+total);
                return i;
            }
        }
        Debug.Log("error");
        return 0;
    }

    public void GameOver()      //游戏结束
    {
        tilemap.ClearAllTiles();

        //结束判断
    }

    public void Show(Piece piece)        //根据成功生成的piece数据，显示tile在map上
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Hide(Piece piece)      //取消tile显示
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPiecePosition(Piece piece, Vector3Int position)   //判断是否生成失败
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // 出圈了
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // 重叠了
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public List<Vector3Int> IsValidSpritePosition(List<Vector3> cells,int rotate,Vector3 position)   //判断位置是否正确
    {
        RectInt bounds = Bounds;
        List<Vector3Int> tmp = new List<Vector3Int>();
        // The position is only valid if every cell is valid
        for (int i = 0; i < cells.Count; i++)
        {
            Vector3 floatposition = (RotatePos(cells[i],"Z",rotate*90) + position);
            Vector3Int tilePosition = new Vector3Int(
                (int)Math.Round(floatposition.x - this.transform.position.x -0.5, 0),
                (int)Math.Round(floatposition.y - this.transform.position.y -0.5, 0),
                0
                );
            //Debug.Log(tilePosition);
            // 出圈了
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return null;
            }

            // 重叠了
            if (tilemap.HasTile(tilePosition))
            {
                return null;
            }

            tmp.Add(tilePosition);
        }

        return tmp;
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

    public void ClearLines()                    //遍历行判断是否可消除，从下往上
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)                 //判断此行是否能够消除，tile全满即可
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)                              //消除行，并下落
    {
        RectInt bounds = Bounds;
        elemList.Clear();
        for (int i = 0; i < 5; i++)
        {
            elemList.Add(0);
        }
        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            TileBase cur = tilemap.GetTile(position);
            elemList[(int)TileElemDict[cur]]++;
            //Debug.Log("addelem "+ (int)TileElemDict[cur]);
            tilemap.SetTile(position, null);
        }
        //传递elem改变
        //Debug.Log(boardSize.y/2 - row - 1 + "行增加");

        battleSystem.RefreshDisplayElemInLine(true, boardSize.y/2-row-1, elemList);
        
        //每列下移动一格
        /*
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
        */
    }

    public int RecursionErase(int row, int col,Element element)
    {
        if (row < Bounds.yMin || col < Bounds.xMin || row > Bounds.yMax || col > Bounds.xMax)
        {
            return 0;
        }
        Element curElem;
        int sum = 0;
        Vector3Int position = new Vector3Int(col, row, 0);
        if (tilemap.GetTile(position) == null)
        {
            return 0;
        }
        else
        {
            curElem = TileElemDict[tilemap.GetTile(position)];
        }
        
        if (element == Element.None)
        {
            element = curElem;
        }
        if(curElem == element)
        {
            tilemap.SetTile(position,null);
            sum += 1;
            sum += RecursionErase(row - 1, col, element);
            sum += RecursionErase(row + 1, col, element);
            sum += RecursionErase(row, col - 1, element);
            sum += RecursionErase(row, col + 1, element);
            return sum;
        }
        else
        {
            return 0;
        }
    }

}
