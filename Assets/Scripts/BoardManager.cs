using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject ContainedObject;
    }

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    private Grid m_Grid;
    private List<Vector2Int> m_EmptyCellsList;

    public int Width = 8;
    public int Height = 8;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;

    [Header("Exit Configuration")]
    public ExitCellObject ExitCellPrefab;

    [Header("Wall Configuration")]
    public WallObject[] WallPrefabs;

    [Header("Food Configuration")]
    public FoodObject[] FoodPrefabs;
    public int MinFood = 2;
    public int MaxFood = 6;

    [Header("Enemy Configuration")]
    public Enemy[] EnemyPrefabs;
    public int MinEnemy = 1;
    public int MaxEnemy = 2;

    [Header("Item Configuration")]
    public CellObject[] ItemPrefabs;

    // Dynamically calculates board size based on current level
    public void SetBoardSize(int level)
    {
        int sizeBonus = Mathf.Min((level - 1) / 2, 12);
        Width = 8 + sizeBonus;
        Height = 8 + sizeBonus;
    }

    public void Init(int level)
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponentInChildren<Grid>();

        m_EmptyCellsList = new List<Vector2Int>();
        m_BoardData = new CellData[Width, Height];

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                Tile tile;
                m_BoardData[x, y] = new CellData();

                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    m_BoardData[x, y].Passable = false;
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[x, y].Passable = true;

                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }

                if (m_Tilemap != null)
                {
                    m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        m_EmptyCellsList.Remove(new Vector2Int(1, 1));

        // Place Exit Cell in upper-right corner dynamically
        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
        if (ExitCellPrefab != null)
        {
            AddObject(Instantiate(ExitCellPrefab), endCoord);
            m_EmptyCellsList.Remove(endCoord);
        }

        GenerateWall(level);
        GenerateFood(level);
        GenerateEnemy(level);
        GenerateItems();
    }

    public void Clean()
    {
        if (m_BoardData == null)
            return;

        int arrayWidth = m_BoardData.GetLength(0);
        int arrayHeight = m_BoardData.GetLength(1);

        for (int y = 0; y < arrayHeight; ++y)
        {
            for (int x = 0; x < arrayWidth; ++x)
            {
                var cellData = m_BoardData[x, y];
                if (cellData != null && cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }
                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];
        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
        obj.Init(coord);
    }

    void GenerateWall(int level)
    {
        if (WallPrefabs == null || WallPrefabs.Length == 0) return;

        int minWalls = Width + (level / 2);
        int maxWalls = Width + 4 + level;
        int wallCount = Random.Range(minWalls, maxWalls);

        for (int i = 0; i < wallCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) break;

            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);

            WallObject prefabToInstantiate = WallPrefabs[Random.Range(0, WallPrefabs.Length)];

            if (prefabToInstantiate != null)
            {
                WallObject newWall = Instantiate(prefabToInstantiate);
                AddObject(newWall, coord);
            }
        }
    }

    void GenerateFood(int level)
    {
        if (FoodPrefabs == null || FoodPrefabs.Length == 0) return;

        int foodPenalty = (level - 1) / 3;
        int minFood = Mathf.Max(1, MinFood - foodPenalty);
        int maxFood = Mathf.Max(2, MaxFood - foodPenalty);

        int foodCount = Random.Range(minFood, maxFood + 1);

        for (int i = 0; i < foodCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) break;

            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);

            FoodObject prefabToInstantiate = FoodPrefabs[Random.Range(0, FoodPrefabs.Length)];

            if (prefabToInstantiate != null)
            {
                FoodObject newFood = Instantiate(prefabToInstantiate);
                AddObject(newFood, coord);
            }
        }
    }

    void GenerateEnemy(int level)
    {
        if (EnemyPrefabs == null || EnemyPrefabs.Length == 0) return;

        int enemyCount = Random.Range(MinEnemy, MaxEnemy + 1);

        for (int i = 0; i < enemyCount; ++i)
        {
            if (m_EmptyCellsList.Count == 0) break;

            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);

            Enemy prefabToInstantiate = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)];

            if (prefabToInstantiate != null)
            {
                Enemy newEnemy = Instantiate(prefabToInstantiate);
                AddObject(newEnemy, coord);
            }
        }
    }

    void GenerateItems()
    {
        if (ItemPrefabs == null || ItemPrefabs.Length == 0) return;

        if (m_EmptyCellsList.Count > 0)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);

            CellObject itemPrefab = ItemPrefabs[Random.Range(0, ItemPrefabs.Length)];

            if (itemPrefab != null)
            {
                CellObject newObj = Instantiate(itemPrefab);
                AddObject(newObj, coord);
            }
        }
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        if (m_Tilemap != null)
        {
            m_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
        }
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        if (m_Tilemap == null) return null;
        return m_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        if (m_Grid == null) return Vector3.zero;
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return m_BoardData[cellIndex.x, cellIndex.y];
    }
}