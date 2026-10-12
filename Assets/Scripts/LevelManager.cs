using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public LevelData[] allLevels;
    public LevelData CurrentLevel { get; private set; }

    void Awake()
    {
        if(Instance != null && Instance == this)
        {
            Destroy(gameObject);
        }

        Instance = this;

        CurrentLevel = allLevels[0];
    }
}
