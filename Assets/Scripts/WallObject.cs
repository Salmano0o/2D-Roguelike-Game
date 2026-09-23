using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    public Tile ObstacleTile;
    public Tile DamagedTile; // Added for Challenge 2
    public int MaxHealth = 3;

    private int m_HealthPoint;
    private Tile m_OriginalTile;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        m_HealthPoint = MaxHealth;

        // Store the tile underneath before overwriting it with the obstacle tile
        m_OriginalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
        GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
    }

    public override bool PlayerWantsToEnter()
    {
        m_HealthPoint -= 1;

        // Trigger the attack animation on the player when hitting a wall
        GameManager.Instance.PlayerController.Attack();

        if (m_HealthPoint > 0)
        {
            // Challenge 2: Swap to damaged tile sprite when HP hits 1
            if (m_HealthPoint == 1 && DamagedTile != null)
            {
                GameManager.Instance.BoardManager.SetCellTile(m_Cell, DamagedTile);
            }

            return false;
        }

        // Restore original ground tile and destroy WallObject instance
        GameManager.Instance.BoardManager.SetCellTile(m_Cell, m_OriginalTile);
        Destroy(gameObject);

        return true;
    }
}