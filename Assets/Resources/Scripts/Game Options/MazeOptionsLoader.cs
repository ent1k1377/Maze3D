using UnityEngine;

public class MazeOptionsLoader : MonoBehaviour
{
    [Space(3)]
    [Header("Maze Settings")]
    [SerializeField] [Range(4, 24)] int _mazeHeight;
    [SerializeField] [Range(4, 24)] int _mazeWidth;

    [Space(3)]
    [Header("Chunk Settings")]
    [SerializeField] [Range(0, 10)] int _additionalWallsToDestroy;
    [SerializeField] [Range(0, 24)] int _radiusOfSpawnArea;

    [Space(3)]
    [Header("Coins Settings")]
    [SerializeField] [Range(1, 10)] int _minCoinsInPack;
    [SerializeField] [Range(1, 10)] int _maxCoinsInPack;

    [Space(3)]
    [Header("Seed Settings")]
    [SerializeField] int _seedNumber;


    void Awake()
    {
        MazeOptions.MazeWidth = _mazeWidth;
        MazeOptions.MazeHeight = _mazeHeight;

        MazeOptions.AdditionalWallsToDestroy = _additionalWallsToDestroy;
        MazeOptions.RadiusOfSpawnAreaInChunk = _radiusOfSpawnArea;

        MazeOptions.MinCoinsInPack = _minCoinsInPack;
        MazeOptions.MaxCoinsInPack = _maxCoinsInPack;

        //SavePrefs.SeedNumber = _seedNumber;
        //SavePrefs.SetLevelSeed();
    }
}
