using UnityEngine;

public class Enemy : CellObject
{
    public int Health = 3;
    public int BaseDamage = 3;

    [Header("Speed & Energy")]
    public int Speed = 10;
    public int MoveEnergyCost = 10;
    private int m_CurrentEnergy = 0;
    private int m_CurrentHealth;

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
        {
            GameManager.Instance.TurnManager.OnTick += TurnHappened;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
        {
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
        }
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = Health;
        m_CurrentEnergy = Speed;
    }

    public override bool PlayerWantsToEnter()
    {
        PlayerStats stats = GameManager.Instance.PlayerController.GetComponent<PlayerStats>();
        int damageDealt = (stats != null) ? stats.Strength : 1;

        m_CurrentHealth -= damageDealt;

        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }

        return false;
    }

    private bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    private void TurnHappened()
    {
        m_CurrentEnergy += Speed;

        while (m_CurrentEnergy >= MoveEnergyCost)
        {
            m_CurrentEnergy -= MoveEnergyCost;

            var playerCell = GameManager.Instance.PlayerController.Cell;
            int xDist = playerCell.x - m_Cell.x;
            int yDist = playerCell.y - m_Cell.y;
            int absXDist = Mathf.Abs(xDist);
            int absYDist = Mathf.Abs(yDist);

            // Attack player if adjacent
            if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
            {
                PlayerStats stats = GameManager.Instance.PlayerController.GetComponent<PlayerStats>();
                int actualDamage = (stats != null) ? stats.CalculateDamage(BaseDamage) : BaseDamage;

                GameManager.Instance.ChangeFood(-actualDamage);
                break;
            }
            else
            {
                // Path toward player
                bool moved = false;
                if (absXDist > absYDist)
                {
                    moved = TryMoveInX(xDist) || TryMoveInY(yDist);
                }
                else
                {
                    moved = TryMoveInY(yDist) || TryMoveInX(xDist);
                }

                if (!moved) break;
            }
        }
    }

    private bool TryMoveInX(int xDist)
    {
        return MoveTo(m_Cell + new Vector2Int(xDist > 0 ? 1 : -1, 0));
    }

    private bool TryMoveInY(int yDist)
    {
        return MoveTo(m_Cell + new Vector2Int(0, yDist > 0 ? 1 : -1));
    }
}