using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public UIDocument UIDoc;

    public TurnManager TurnManager { get; private set; }

    private int m_FoodAmount;
    private Label m_FoodLabel;
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    private int m_CurrentLevel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensure time is unpaused on initialization
        Time.timeScale = 1f;
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        if (UIDoc != null && UIDoc.rootVisualElement != null)
        {
            m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
            m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
            if (m_GameOverPanel != null)
            {
                m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");
            }
        }

        // Automatically start the game if no UIManager exists in the scene
        if (UIManager.Instance == null)
        {
            StartNewGame();
        }
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;

        if (m_GameOverPanel != null)
        {
            m_GameOverPanel.style.visibility = Visibility.Hidden;
        }

        m_CurrentLevel = 1;
        m_FoodAmount = 20;

        if (m_FoodLabel != null)
        {
            m_FoodLabel.text = "Food : " + m_FoodAmount;
        }

        // Clean existing board BEFORE scaling dimensions
        if (BoardManager != null)
        {
            BoardManager.Clean();

            // Set dimensions and parameters for level 1
            UpdateLevelDifficulty();

            // Initialize board with current level
            BoardManager.Init(m_CurrentLevel);
        }

        if (PlayerController != null)
        {
            PlayerController.Init();
            PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        }
    }

    public void NewLevel()
    {
        m_CurrentLevel++;

        // Clean previous level BEFORE setting new dimensions
        if (BoardManager != null)
        {
            BoardManager.Clean();

            // Update grid size and difficulty for the new level
            UpdateLevelDifficulty();

            // Re-initialize board with updated level parameters
            BoardManager.Init(m_CurrentLevel);
        }

        if (PlayerController != null)
        {
            PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        }
    }

    private void UpdateLevelDifficulty()
    {
        // 1. Set dynamic grid size based on level
        BoardManager.SetBoardSize(m_CurrentLevel);

        // 2. Increase enemy count on higher levels
        BoardManager.MinEnemy = 1 + (m_CurrentLevel / 3);
        BoardManager.MaxEnemy = 2 + (m_CurrentLevel / 2);
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;

        if (m_FoodLabel != null)
        {
            m_FoodLabel.text = "Food : " + m_FoodAmount;
        }

        if (m_FoodAmount <= 0)
        {
            if (PlayerController != null)
            {
                PlayerController.GameOver();
            }

            if (m_GameOverPanel != null)
            {
                m_GameOverPanel.style.visibility = Visibility.Visible;
                if (m_GameOverMessage != null)
                {
                    m_GameOverMessage.text = "Game Over!\n\nSurvived " + m_CurrentLevel + " days\n\nPress [Enter] to restart";
                }
            }
        }
    }
}