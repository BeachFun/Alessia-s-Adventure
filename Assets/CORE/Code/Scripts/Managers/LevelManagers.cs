using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LevelManager))]

public class LevelManagers : ManagersInitializator
{
	public static LevelManager Level { get; private set; }


    private void Awake()
	{
        InitializeFields();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Level);

        StartCoroutine(StartupManagers());
	}

    private protected override void OnDestroy()
    {
        base.OnDestroy();

        Level = null;
	}

    // Инициализация менеджеров
    private void InitializeFields()
    {
        Level = GetComponent<LevelManager>();
    }
}
