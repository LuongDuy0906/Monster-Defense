using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int wavesToWin;
    public int startResources;
    public int startingLives;
    public Vector2 initialSpawnPosition;
    public WaveData[] waves;
}
