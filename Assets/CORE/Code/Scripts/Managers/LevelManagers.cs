using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(InventoryManager))]

public class LevelManagers : Managers
{
	public static InventoryManager Inventory { get; private set; }


	private void Awake()
	{
        // Инициализация менеджеров
        Inventory = GetComponent<InventoryManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Inventory);

        StartCoroutine(StartupManagers());
	}

    private protected override void OnDestroy()
    {
        base.OnDestroy();

        Status = ManagerStatus.Shutdown;
	}
}
