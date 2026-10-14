using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    public AudioClip mainMenuMusic;
    public AudioClip gamePlayMusic;
    public AudioClip buttonHoverClip;
    public AudioClip buttonClickClip;
    public AudioClip towerPlaceClip;
    public AudioClip enemyDestroyClip;
    public AudioClip missionCompleteClip;
    public AudioClip gameOverClip;
    public AudioClip pauseClip;
    public AudioClip unPauseClip;
    public AudioClip speedSlowClip;
    public AudioClip speedNormalClip;
    public AudioClip speedFastClip;
    public AudioClip panelToggleClip;
    public AudioClip warningClip;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            musicSource.volume = 0.2f;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayTowerPlaced() => PlaySound(towerPlaceClip);
    public void PlayEnemyDestroy() => PlaySound(enemyDestroyClip);
    public void PlayButtonClick() => PlaySound(buttonClickClip);
    public void PlayButtonHover() => PlaySound(buttonHoverClip);
    public void PlayMissionComplete() => PlaySound(missionCompleteClip);
    public void PlayGameOver() => PlaySound(gameOverClip);
    public void PlayPause() => PlaySound(pauseClip);
    public void PlayUnPause() => PlaySound(unPauseClip);
    public void PlaySpeedSlow() => PlaySound(speedSlowClip);
    public void PlaySpeedNormal() => PlaySound(speedNormalClip);
    public void PlaySpeedFast() => PlaySound(speedFastClip);
    public void PlayPanelToggle() => PlaySound(panelToggleClip);
    public void PlayWarning() => PlaySound(warningClip);

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
}
