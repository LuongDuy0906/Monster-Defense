using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text resourcesText;
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private GameObject towerCardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private TowerData[] towers;
    [SerializeField] private GameObject noResourcesText;
    private List<GameObject> activeCards = new List<GameObject>();
    private Platform _currentPlatform;

    void OnEnable()
    {
        SpawnManager.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnResourcesChanged += UpdateResourcesText;
        Platform.OnPlatformClicked += HandlePlatformClicked;
        TowerCard.OnTowerSelected += HandleTowerSelected;
    }

    void OnDisable()
    {
        SpawnManager.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnResourcesChanged -= UpdateResourcesText;
        Platform.OnPlatformClicked -= HandlePlatformClicked;
        TowerCard.OnTowerSelected -= HandleTowerSelected;
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";
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
        GameManager.Instance.SetTimeScale(1);
    }

    private void HandleTowerSelected(TowerData data)
    {
        if(GameManager.Instance.Resources >= data.cost)
        {
            GameManager.Instance.SpendResource(data.cost);
            _currentPlatform.PlaceTower(data);

        } else
        {
            StartCoroutine(ShowNoResourcesMessage());
        }
        HideTowerPanel();
    }

    private IEnumerator ShowNoResourcesMessage()
    {
        noResourcesText.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        noResourcesText.SetActive(false);
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
}
