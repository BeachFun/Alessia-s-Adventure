using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour, IGameManager
{
    [SerializeField] private Player player;

    private int _collectedDiamonds = 0;

    public ManagerStatus Status { get; private set; }
    public int CollectedDiamonds
    {
        get => _collectedDiamonds;
        private set
        {
            _collectedDiamonds = value;
            Messenger<int>.Broadcast(GameEvents.DIAMOND_CHANGED, value);
        }
    }


    private void Start()
    {
        StartCoroutine(Startup());
    }

    private void OnDestroy()
    {
        if (Status == ManagerStatus.Started)
        {
            Messenger<string, int>.RemoveListener(GameEvents.ITEM_COLLECTED, OnItemCollected);
        }
    }

    public IEnumerator Startup()
    {
        Status = ManagerStatus.Initializing;

        yield return null;

        Messenger<string, int>.AddListener(GameEvents.ITEM_COLLECTED, OnItemCollected);

        Status = ManagerStatus.Started;
    }

    private void OnItemCollected(string itemName, int count)
    {
        if (itemName == ItemNames.Dagger)
        {
            player.DaggerCount += count;
        }
        if (itemName == ItemNames.SmallDiamond)
        {
            CollectedDiamonds++;
        }
        if (itemName == ItemNames.BigDiamond)
        {
            CollectedDiamonds += 5;
        }
        if (itemName == ItemNames.Heart)
        {
            player.Heal(25); // TODO: переделать
        }
    }
}