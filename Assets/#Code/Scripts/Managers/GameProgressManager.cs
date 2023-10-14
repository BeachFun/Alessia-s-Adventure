using System.Collections;
using UnityEngine;

public class GameProgressManager : MonoBehaviour, IGameManager
{
    [Tooltip("Настроить на кол-во уровней в игре")]
    [SerializeField] private LevelData[] levelDatas;


    public ManagerStatus Status { get; private set; }


    public IEnumerator Startup()
    {
        Status = ManagerStatus.Initializing;

        yield return null;

        var data = GameManagers.DataSaver.Load<LevelData[]>("levelData");
        if (data is not null && data.Length != 0)
            levelDatas = data;

        Status = ManagerStatus.Started;
    }

    internal LevelData this[int levelIndex]
    {
        get => levelDatas[levelIndex];
        set
        {
            levelDatas[levelIndex] = value;
            GameManagers.DataSaver.Dump(levelDatas, "levelData");
        }
    }
}