using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(InventoryManager))]

/// <summary>
/// Управляющий/контроллер всеми диспетчерами.
/// </summary>
public class Managers : MonoBehaviour
{
    // TODO: превратить класс в локатор служб или создать отдельный от класса локатор служб

    public static InventoryManager Inventory { get; private set; }


    private List<IGameManager> _startSequence;


	void Awake()
	{
        //DontDestroyOnLoad(gameObject);

        // Инициализация менеджеров
        Inventory = new InventoryManager();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Inventory);

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
			}

			yield return null;
		}

		Debug.Log("All managers started up");
	}
}
