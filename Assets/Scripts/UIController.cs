using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;

    void OnEnable()
    {
        SpawnManager.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
    }

    void OnDisable()
    {
        SpawnManager.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $"Lives: {currentLives}";
    }
}
