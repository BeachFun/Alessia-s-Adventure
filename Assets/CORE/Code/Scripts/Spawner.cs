using UnityEngine;
using UnityEngine.Events;

public abstract class Spawner : MonoBehaviour
{
    public UnityEvent onSpawn;

    public void Spawn(GameObject objectToSpawn)
    {
        SpawnObject(objectToSpawn);
        onSpawn?.Invoke();
    }

    protected abstract void SpawnObject(GameObject objectToSpawn);
}