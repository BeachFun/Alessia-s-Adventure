using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent(typeof(SettingsManager))]
//[RequireComponent(typeof(PlayerManager))]
//[RequireComponent(typeof(GameManager))]
//[RequireComponent(typeof(DataManager))]
//[RequireComponent(typeof(UIManager))]

/// <summary>
/// Управляющий/контроллер всеми диспетчерами.
/// </summary>
public class Managers : MonoBehaviour
{
	// TODO: превратить класс в локатор служб или создать отдельный от класса локатор служб

	//public static SettingsManager Settings { get; private set; }
	//public static PlayerManager Player { get; private set; }
	//public static GameManager Game { get; private set; }
	//public static DataManager Data { get; private set; }
	//public static UIManager UI { get; private set; }


	private List<IGameManager> _startSequence;


	void Awake()
	{
		//DontDestroyOnLoad(gameObject);

		// Инициализация менеджеров
		//Settings = GetComponent<SettingsManager>();
		//Player = GetComponent<PlayerManager>();
		//Game = GetComponent<GameManager>();
		//Data = GetComponent<DataManager>();
		//UI = GetComponent<UIManager>();

		//_startSequence = new List<IGameManager>();
		//_startSequence.Add(Data);
		//_startSequence.Add(Settings);
		//_startSequence.Add(Player);
		//_startSequence.Add(Game);
		//_startSequence.Add(UI);

		StartCoroutine(StartupManagers());
	}

	/// <summary>
	/// Запуск всех менеджеров, привязанных к этому контроллеру
	/// </summary>
	/// <returns>Перечислитель</returns>
	public IEnumerator StartupManagers()
	{
		Debug.Log("Запуск менеджеров...");

		foreach (IGameManager manager in _startSequence)
		{
			StartCoroutine(manager.Startup());
		}

		yield return null;

		int numModules = _startSequence.Count;
		int numReady = 0;

		while (numReady < numModules)
		{
			int lastReady = numReady;
			numReady = 0;

			foreach (IGameManager manager in _startSequence)
			{
				if (manager.status == ManagerStatus.Started)
				{
					numReady++;
				}
			}

			if (numReady > lastReady)
			{
				Debug.Log("Progress: " + numReady + "/" + numModules);
				// Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
			}

			yield return null;
		}

		Debug.Log("All managers started up");
	}
}
