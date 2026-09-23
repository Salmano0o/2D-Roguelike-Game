using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MoveSpeed = 5.0f;

    public Vector2Int Cell => m_CellPosition;

    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_IsGameOver;
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    private Animator m_Animator;
    private PlayerStats m_Stats;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Stats = GetComponent<PlayerStats>();
    }

    public void Init()
    {
        m_IsGameOver = false;
        m_IsMoving = false;
        m_Animator.SetBool("Moving", false);

        if (m_Stats != null)
        {
            m_Stats.CurrentEnergy = m_Stats.Speed;
        }
    }

    public void GameOver()
    {
        m_IsGameOver = true;
    }

    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        MoveTo(cell, true);
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    {
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }

        m_Animator.SetBool("Moving", m_IsMoving);
    }

    public void Attack()
    {
        m_Animator.SetTrigger("Attack");
    }

    private void Update()
    {
        if (m_IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }

        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);

            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool("Moving", false);

                var cellData = m_Board.GetCellData(m_CellPosition);
                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.PlayerEntered();
                }
            }
            return;
        }

        if (m_Board == null) return;

        // Skip / Wait turn action
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ExecuteAction(0, Vector2Int.zero, false);
            return;
        }

        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }

        if (hasMoved)
        {
            int cost = (m_Stats != null) ? m_Stats.MoveEnergyCost : 10;
            ExecuteAction(cost, newCellTarget, true);
        }
    }

    private void ExecuteAction(int energyCost, Vector2Int targetCell, bool isMovement)
    {
        if (m_Stats != null && !m_Stats.CanAfford(energyCost))
        {
            Debug.Log("Not enough energy for this action!");
            return;
        }

        if (isMovement)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(targetCell);
            if (cellData != null && cellData.Passable)
            {
                if (m_Stats != null) m_Stats.SpendEnergy(energyCost);

                if (cellData.ContainedObject == null)
                {
                    MoveTo(targetCell, false);
                }
                else if (cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(targetCell, false);
                }
            }
        }
        else
        {
            int cost = (m_Stats != null) ? m_Stats.MoveEnergyCost : 10;
            if (m_Stats != null) m_Stats.SpendEnergy(cost);
        }

        // Advance world turn tick when out of energy
        if (m_Stats == null || m_Stats.CurrentEnergy < m_Stats.MoveEnergyCost)
        {
            GameManager.Instance.TurnManager.Tick();
            if (m_Stats != null)
            {
                m_Stats.AddTurnEnergy();
            }
        }
    }
}