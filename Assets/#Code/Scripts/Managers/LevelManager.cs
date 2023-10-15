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
    private LevelData _levelData;
    // При завершении
    private (int, int, int) _scores;


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
    internal string LevelName { get; private set; }
    internal LevelData CurrentLevelData { get => _levelData; private set => _levelData = value; }
    internal float TimePassed { get => Time.time - _levelStartTime; }

    private void OnDestroy()
    {
        if (Status == ManagerStatus.Started)
        {
            Messenger.RemoveListener(GameEvents.ENEMY_KILLED, OnEnemyKilled);
            Messenger.RemoveListener(GameEvents.LEVEL_COMPLETE, OnLevelComplete);
            Messenger.RemoveListener(GameEvents.LEVEL_FAILED, OnLevelFailed);
            Messenger.RemoveListener(GameEvents.LEVEL_EXIT, OnLevelExit);
            Messenger.RemoveListener(GameEvents.LEVEL_RESTART, LevelRestart);
            Messenger<string, int>.RemoveListener(GameEvents.ITEM_COLLECTED, OnItemCollected);
        }
    }

    public IEnumerator Startup()
    {
        Status = ManagerStatus.Initializing;

        yield return null;

        _levelData = LevelIndex != -1 ? GameManagers.GameProgress[LevelIndex] : default(LevelData);
        LevelName = _levelData.LevelName;

        Messenger.AddListener(GameEvents.ENEMY_KILLED, OnEnemyKilled);
        Messenger.AddListener(GameEvents.LEVEL_COMPLETE, OnLevelComplete);
        Messenger.AddListener(GameEvents.LEVEL_FAILED, OnLevelFailed);
        Messenger.AddListener(GameEvents.LEVEL_EXIT, OnLevelExit);
        Messenger.AddListener(GameEvents.LEVEL_RESTART, LevelRestart);
        Messenger<string, int>.AddListener(GameEvents.ITEM_COLLECTED, OnItemCollected);

        _levelStartTime = Time.time;
        Messenger.Broadcast(GameEvents.GAME_STARTED);

        Status = ManagerStatus.Started;
    }

    private void OnItemCollected(string itemName, int count)
    {
        if (itemName == ItemNames.Dagger)
        {
            GameManagers.Audio.PlaySound(AudioClipPool.Instance["Dagger Collect"], 0.2f);
            player.DaggerCount += count;
        }
        if (itemName == ItemNames.SmallDiamond)
        {
            GameManagers.Audio.PlaySound(AudioClipPool.Instance["Diamond Collect"], 0.15f);
            CollectedDiamonds++;
            _levelData.CollectedDiamondCount = CollectedDiamonds;
        }
        if (itemName == ItemNames.BigDiamond)
        {
            GameManagers.Audio.PlaySound(AudioClipPool.Instance["Diamond Collect"], 0.25f);
            CollectedDiamonds += 5;
            _levelData.CollectedDiamondCount = CollectedDiamonds;
        }
        if (itemName == ItemNames.Heart)
        {
            player.Heal(25); // TODO: переделать
        }
    }

    private void OnEnemyKilled()
    {
        EnemyKillCounter++;
        _levelData.EnemiesKillCounter = EnemyKillCounter;
    }

    private void OnLevelPassed(bool isCompleted)
    {
        GameManagers.Audio.StopMusic();

        _lastTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        // Collecting data
        float timeElapsed = Time.time - _levelStartTime;
        Time.timeScale = _lastTimeScale;

        var timeAccess = TimeSpan.Parse(_levelData.TimeAccess);
        var timeCompleted = TimeSpan.FromSeconds((double)timeElapsed);

        // Grouping data
        _levelData.IsComplete = isCompleted;
        _levelData.TimeComplete = timeCompleted.ToString();

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

        Messenger<bool, LevelData, (int, int, int)>.Broadcast(GameEvents.LEVEL_PASS_DATA_COLLECTED, true, _levelData, _scores);
    }

    private void OnLevelFailed()
    {
        OnLevelPassed(false);

        SaveData();

        Messenger<bool, LevelData, (int, int, int)>.Broadcast(GameEvents.LEVEL_PASS_DATA_COLLECTED, false, _levelData, _scores);
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

    public void LevelRestart()
    {
        GameManagers.Audio.StopMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}