using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LevelManager))]

public class LevelManagers : ManagersInitializator
{
	public static LevelManager GameProgress { get; private set; }


    private void Awake()
	{
        DontDestroyOnLoad(gameObject);

        InitializeFields();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(GameProgress);

        StartCoroutine(StartupManagers());
	}

    private protected override void OnDestroy()
    {
        base.OnDestroy();

        GameProgress = null;
	}

    // Инициализация менеджеров
    private void InitializeFields()
    {
        GameProgress = GetComponent<LevelManager>();
    }
}
