using System.Collections;
using UnityEngine;

public class ScreenplayManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }


    private void Start()
    {
        StartCoroutine(Startup());
    }

    private void OnDestroy()
    {
        
    }

    public IEnumerator Startup()
    {
        Status = ManagerStatus.Initializing;

        yield return null;

        Status = ManagerStatus.Started;
    }
}