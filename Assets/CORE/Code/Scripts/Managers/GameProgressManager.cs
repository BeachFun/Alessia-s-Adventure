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

        // TODO: реализовать загрузку данных, которая будет заменять levelDatas

        Status = ManagerStatus.Started;
    }

    internal LevelData this[int levelIndex]
    {
        get => levelDatas[levelIndex];
        set => levelDatas[levelIndex] = value;
    }
}