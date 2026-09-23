using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    public GameObject MainMenuPanel;
    public GameObject PauseMenuPanel;
    public GameObject SettingsPanel;

    [Header("Settings UI Controls")]
    public Slider VolumeSlider;

    private bool m_IsPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Start in Main Menu state
        ShowMainMenu();
    }

    private void Update()
    {
        // Toggle Pause Menu with Escape key using the New Input System
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (MainMenuPanel != null && MainMenuPanel.activeSelf) return;

            if (m_IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ShowMainMenu()
    {
        Time.timeScale = 0f;
        if (MainMenuPanel != null) MainMenuPanel.SetActive(true);
        if (PauseMenuPanel != null) PauseMenuPanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
    }

    public void StartNewGame()
    {
        if (MainMenuPanel != null) MainMenuPanel.SetActive(false);
        if (PauseMenuPanel != null) PauseMenuPanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);

        Time.timeScale = 1f;
        m_IsPaused = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }
    }

    public void PauseGame()
    {
        m_IsPaused = true;
        Time.timeScale = 0f;
        if (PauseMenuPanel != null) PauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        m_IsPaused = false;
        Time.timeScale = 1f;
        if (PauseMenuPanel != null) PauseMenuPanel.SetActive(false);
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (SettingsPanel != null) SettingsPanel.SetActive(false);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}