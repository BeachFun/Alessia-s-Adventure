using UnityEngine;

[System.Serializable]

/// <summary>
/// Требования к прохождению уровня
/// </summary>
internal struct LevelData
{
    [SerializeField] private int level;
    [SerializeField] private string name;
    [SerializeField] private string timeAccess;
    [SerializeField] private int enemiesIntoLevel;

    private bool isComplete;
    private string timeComplete;
    private int enemiesKillCounter;
    private int collectedDiamondCount;


    internal LevelData(int levelIndex, string name)
    {
        this = new LevelData();

        Level = levelIndex;
        Name = name;
        timeAccess = System.TimeSpan.Zero.ToString();
    }

    public int Level { get => level; set => level = value; }
    public string Name { get => name; set => name = value; }
    public string TimeAccess { get => timeAccess; set => timeAccess = value; }
    public int EnemiesIntoLevel { get => enemiesIntoLevel; set => enemiesIntoLevel = value; }
    public bool IsComplete { get => isComplete; set => isComplete = value; }
    public string TimeComplete { get => timeComplete; set => timeComplete = value; }
    public int EnemiesKillCounter { get => enemiesKillCounter; set => enemiesKillCounter = value; }
    public int CollectedDiamondCount { get => collectedDiamondCount; set => collectedDiamondCount = value; }
}
