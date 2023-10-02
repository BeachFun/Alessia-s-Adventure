using System;
using UnityEngine;

[System.Serializable]

/// <summary>
/// Требования к прохождению уровня
/// </summary>
internal struct LevelData
{
    [SerializeField] private int levelIndex;
    [SerializeField] private string levelName;
    [SerializeField] private string timeAccess;
    [SerializeField] private int enemiesIntoLevel;
    [SerializeField] private int maxDiamonds;

    private bool isComplete;
    private string timeComplete;
    private int enemiesKillCounter;
    private int collectedDiamondCount;
    private int bestScore;


    internal LevelData(int levelIndex, string levelName)
    {
        this = new LevelData();

        LevelIndex = levelIndex;
        LevelName = levelName;
        timeComplete = TimeSpan.Zero.ToString();
        timeAccess = TimeSpan.Zero.ToString();
    }

    public int LevelIndex { get => levelIndex; set => levelIndex = value; }
    public string LevelName { get => levelName; set => levelName = value; }
    public bool IsComplete { get => isComplete; set => isComplete = value; }
    public string TimeComplete { get => timeComplete; set => timeComplete = value; }
    public string TimeAccess { get => timeAccess; set => timeAccess = value; }
    public int EnemiesKillCounter { get => enemiesKillCounter; set => enemiesKillCounter = value; }
    public int EnemiesIntoLevel { get => enemiesIntoLevel; set => enemiesIntoLevel = value; }
    public int CollectedDiamondCount { get => collectedDiamondCount; set => collectedDiamondCount = value; }
    public int MaxDiamonds { get => maxDiamonds; set => maxDiamonds = value; }
    public int BestScore { get => bestScore; set => bestScore = value; }


    internal LevelData Union(LevelData newLevelData)
    {
        if (newLevelData.levelIndex != levelIndex) throw new ArgumentException();

        if (newLevelData.isComplete)
        {
            isComplete = newLevelData.isComplete;
        }

        if (timeComplete is null || TimeSpan.Parse(newLevelData.timeComplete) < TimeSpan.Parse(timeComplete))
        {
            timeComplete = newLevelData.timeComplete;
        }

        if (newLevelData.enemiesKillCounter > enemiesKillCounter)
        {
            enemiesKillCounter = newLevelData.enemiesKillCounter;
        }

        if (newLevelData.collectedDiamondCount > collectedDiamondCount)
        {
            collectedDiamondCount = newLevelData.collectedDiamondCount;
        }

        if (newLevelData.bestScore > bestScore)
        {
            bestScore = newLevelData.bestScore;
        }

        return this;
    }
}
