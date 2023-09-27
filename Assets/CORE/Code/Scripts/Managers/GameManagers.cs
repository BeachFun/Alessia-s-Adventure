using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GameProgressManager))]

public class GameManagers : Managers
{
	public static GameProgressManager GameProgress { get; private set; }


	private void Awake()
	{
        DontDestroyOnLoad(gameObject);

        // Инициализация менеджеров
        GameProgress = GetComponent<GameProgressManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(GameProgress);

        StartCoroutine(StartupManagers());
	}

    private protected override void OnDestroy()
    {
        base.OnDestroy();

        GameProgress = null;
	}
}
