using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(InventoryManager))]

/// <summary>
/// Управляющий/контроллер всеми диспетчерами.
/// </summary>
public class LevelManagers : MonoBehaviour
{
	public ManagerStatus Status { get; private set; }

	public static InventoryManager Inventory { get; private set; }


	private List<IGameManager> _startSequence;


	private void Awake()
	{
        //DontDestroyOnLoad(gameObject);

        // Инициализация менеджеров
        Inventory = new InventoryManager();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Inventory);

        StartCoroutine(StartupManagers());
	}

    private void OnDestroy()
    {
		Inventory = null;

		Status = ManagerStatus.Shutdown;
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

		Status = ManagerStatus.Started;
		Debug.Log("All managers started up");
	}
}
