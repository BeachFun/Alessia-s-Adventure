using System.Collections;
using TimeSpan = System.TimeSpan;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour, IGameManager
{
    [SerializeField] private string sceneNameToBack;

    [Header("Score System")]
    [SerializeField] private int scoreOnCompleted = 1000;
    [SerializeField] private int oneSecondScore = 100;
    [SerializeField] private int oneKillEnemyScore = 210;
    [SerializeField] private int oneDiamondCollectScore = 150;

    [Header("References")]
    [SerializeField] private Player player;

    private int _collectedDiamonds = 0;
    private int _enemyKillCounter = 0;
    private float _lastTimeScale;
    private float _levelStartTime;

    private (int, int, int) _scores;
    private LevelData _levelData;


    public ManagerStatus Status { get; private set; }
    public int CollectedDiamonds
    {
        get => _collectedDiamonds;
        private set
        {
            _collectedDiamonds = value;
            Messenger<int>.Broadcast(GameEvents.DIAMOND_CHANGED, value);
        }
    }
    public int EnemyKillCounter { get => _enemyKillCounter; set => _enemyKillCounter = value; }
    internal static int LevelIndex { get; set; }


    private void Start()
    {
        StartCoroutine(Startup());
    }

    private void OnDestroy()
    {
        if (Status == ManagerStatus.Started)
        {
            Messenger.RemoveListener(GameEvents.ENEMY_KILLED, OnEnemyKilled);
            Messenger.RemoveListener(GameEvents.LEVEL_COMPLETE, OnLevelComplete);
            Messenger.RemoveListener(GameEvents.LEVEL_FAILED, OnLevelFailed);
            Messenger.RemoveListener(GameEvents.LEVEL_EXIT, OnLevelExit);
            Messenger<string, int>.RemoveListener(GameEvents.ITEM_COLLECTED, OnItemCollected);
        }
    }

    public IEnumerator Startup()
    {
        Status = ManagerStatus.Initializing;

        yield return null;

        Messenger.AddListener(GameEvents.ENEMY_KILLED, OnEnemyKilled);
        Messenger.AddListener(GameEvents.LEVEL_COMPLETE, OnLevelComplete);
        Messenger.AddListener(GameEvents.LEVEL_FAILED, OnLevelFailed);
        Messenger.AddListener(GameEvents.LEVEL_EXIT, OnLevelExit);
        Messenger<string, int>.AddListener(GameEvents.ITEM_COLLECTED, OnItemCollected);

        Status = ManagerStatus.Started;

        _levelStartTime = Time.time;
    }

    private void OnItemCollected(string itemName, int count)
    {
        if (itemName == ItemNames.Dagger)
        {
            player.DaggerCount += count;
        }
        if (itemName == ItemNames.SmallDiamond)
        {
            CollectedDiamonds++;
        }
        if (itemName == ItemNames.BigDiamond)
        {
            CollectedDiamonds += 5;
        }
        if (itemName == ItemNames.Heart)
        {
            player.Heal(25); // TODO: переделать
        }
    }

    private void OnEnemyKilled() => EnemyKillCounter++;

    private void OnLevelPassed(bool isCompleted)
    {
        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // Collecting data
        float timeElapsed = Time.time - _levelStartTime;
        Time.timeScale = _lastTimeScale;

        _levelData = GameManagers.GameProgress[LevelIndex];

        var timeAccess = TimeSpan.Parse(_levelData.TimeAccess);
        var timeCompleted = TimeSpan.FromSeconds((double)timeElapsed);

        // Grouping data
        _levelData.IsComplete = isCompleted;
        _levelData.TimeComplete = timeCompleted.ToString();
        _levelData.EnemiesKillCounter = EnemyKillCounter;
        _levelData.CollectedDiamondCount = CollectedDiamonds;

        _scores.Item1 = oneSecondScore * (isCompleted && timeCompleted < timeAccess ? (timeAccess - timeCompleted).Seconds : 0);
        _scores.Item2 = EnemyKillCounter * oneKillEnemyScore;
        _scores.Item3 = CollectedDiamonds * oneDiamondCollectScore;

        _levelData.BestScore = _scores.Item1 + _scores.Item2 + _scores.Item3;
    }

    private void OnLevelComplete()
    {
        OnLevelPassed(true);

        // Additional collecting and grouping data
        _levelData.IsComplete = true;
        _levelData.BestScore += scoreOnCompleted;

        SaveData();

        Messenger<bool, LevelData, (int, int, int)>.Broadcast(GameEvents.LEVEL_END_SCREEN_OPENED, true, _levelData, _scores);
    }

    private void OnLevelFailed()
    {
        OnLevelPassed(false);

        SaveData();

        Messenger<bool, LevelData, (int, int, int)>.Broadcast(GameEvents.LEVEL_END_SCREEN_OPENED, false, _levelData, _scores);
    }

    private void SaveData()
    {
        LevelData lastLevelData = GameManagers.GameProgress[LevelIndex];

        if (GameManagers.GameProgress is not null && (lastLevelData.IsComplete || _levelData.IsComplete))
        {
            GameManagers.GameProgress[LevelIndex] = lastLevelData.Union(_levelData);
        }
        else
        {
            Debug.Log("Не удалось ссохранить данные.");
        }
    }

    private void OnLevelExit()
    {
        SceneManager.LoadScene(sceneNameToBack);
    }
}