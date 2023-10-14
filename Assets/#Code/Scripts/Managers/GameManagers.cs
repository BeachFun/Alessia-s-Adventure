using UnityEngine;
using System.Collections.Generic;
using System.IO;

[RequireComponent(typeof(GameProgressManager))]

public class GameManagers : ManagersInitializator
{
	public static GameProgressManager GameProgress { get; private set; }
    public static DataSerializer DataSaver { get; private set; }

	private void Awake()
	{
        DontDestroyOnLoad(gameObject);

        // Инициализация менеджеров
        GameProgress = GetComponent<GameProgressManager>();
        DataSaver = new DataSerializer(Path.Combine(Application.persistentDataPath, "data.gd"));

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
