using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Управляющий/контроллер всеми диспетчерами.
/// </summary>
public abstract class ManagersInitializator : MonoBehaviour
{
    public ManagerStatus Status { get; private protected set; }

    private protected List<IGameManager> _startSequence;


	private protected virtual void OnDestroy()
	{
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
				if (manager.Status == ManagerStatus.Started)
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
		Messenger.Broadcast(GameEvents.ALL_MANAGERS_STARTED);
		Debug.Log("All managers started up");
	}
}