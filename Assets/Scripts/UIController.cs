using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private TowerData[] towers;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.blue;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private GameObject missionCompletePanel;


    private List<GameObject> activeCards = new List<GameObject>();
    private Platform _currentPlatform;
    private bool _isGamePaused = false;

    void OnEnable()
    {
        SpawnManager.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnResourcesChanged += UpdateResourcesText;
        Platform.OnPlatformClicked += HandlePlatformClicked;
        TowerCard.OnTowerSelected += HandleTowerSelected;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SpawnManager.OnMissionCompleted += ShowMissionComplete;
    }

    void OnDisable()
    {
        SpawnManager.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnResourcesChanged -= UpdateResourcesText;
        Platform.OnPlatformClicked -= HandlePlatformClicked;
        TowerCard.OnTowerSelected -= HandleTowerSelected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SpawnManager.OnMissionCompleted -= ShowMissionComplete;
    }

    void Start()
    {
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";

        if(currentLives == 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateResourcesText(int currentResources)
    {
        resourcesText.text = $"Resources: {currentResources}";
    }

    private void HandlePlatformClicked(Platform platform)
    {
        _currentPlatform = platform;
        ShowTowerPanel();
    }

    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        GameManager.Instance.SetTimeScale(0);
        Platform.towerPanelOpened = true;
        PopulateTowerCard();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform.towerPanelOpened = false;
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
    }

    private void HandleTowerSelected(TowerData data)
    {
        if(_currentPlatform.transform.childCount > 0)
        {
            HideTowerPanel();
            StartCoroutine(ShowWarningMessage("This platform already has a tower"));
            return;
        }

        if(GameManager.Instance.Resources >= data.cost)
        {
            GameManager.Instance.SpendResource(data.cost);
            _currentPlatform.PlaceTower(data);

        } else
        {
            StartCoroutine(ShowWarningMessage("Not Enough Resources"));
        }
        HideTowerPanel();
    }

    private IEnumerator ShowWarningMessage(string message)
    {
        warningText.text = message; 
        warningText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        warningText.gameObject.SetActive(false);
    }

    private void PopulateTowerCard()
    {
        foreach(var card in activeCards)
        {
            Destroy(card);
        }

        activeCards.Clear();

        foreach(var data in towers)
        {
            GameObject cardGameObject = Instantiate(towerCardPrefab, cardsContainer);
            TowerCard card = cardGameObject.GetComponent<TowerCard>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
        }
    }

    private void SetGameSpeed(float timeScale)
    {
        if(pausePanel.activeSelf) return;
        HighlightSelectedSpeedButton(timeScale);
        GameManager.Instance.SetGameSpeed(timeScale);
    }

    private void UpdateButtonVisual(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;

        TMP_Text text = button.GetComponentInChildren<TMP_Text>();

        if(text != null)
        {
            text.color = isSelected ? selectedTextColor : normalTextColor;
        }
    }

    private void HighlightSelectedSpeedButton(float selectedSpeed)
    {
        UpdateButtonVisual(speed1Button, selectedSpeed == 0.2f);
        UpdateButtonVisual(speed2Button, selectedSpeed == 1f);
        UpdateButtonVisual(speed3Button, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if(towerPanel.activeSelf) return;

        if(_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        } else
        {
            pausePanel.SetActive(true);   
            _isGamePaused = true;
            GameManager.Instance.SetTimeScale(0f);
        }
    }

    public void RestartLevel()
    {
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel);
    }

    public void GoToMenu()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenuScene");
    }

     public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else 
            Application.Quit();
        #endif
    }

    private void ShowGameOver()
    {
        GameManager.Instance.SetTimeScale(0f);
        gameOverPanel.SetActive(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ShowObjective());
    }

    private IEnumerator ShowObjective()
    {
        objectiveText.text = $"Survice {LevelManager.Instance.CurrentLevel.wavesToWin} waves!";
        objectiveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        objectiveText.gameObject.SetActive(false);
    }

    private void ShowMissionComplete()
    {
        missionCompletePanel.SetActive(true);
        GameManager.Instance.SetTimeScale(0f);
    }

    public void EnterEndlessMode()
    {
        missionCompletePanel.gameObject.SetActive(false);
        GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        SpawnManager.Instance.EnableEnlessMode();
    }
}
