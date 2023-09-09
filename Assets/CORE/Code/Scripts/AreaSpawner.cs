using UnityEngine;

public class AreaSpawner : Spawner
{
    [SerializeField] private Vector2 spawnSize = Vector2.one;
    [SerializeField] private bool roundPosition = true;
    [SerializeField] private float roundTo = 0.5f;


    protected override void SpawnObject(GameObject objectToSpawn)
    {
        Instantiate(objectToSpawn, GetRandomPositionWithInSpawnArea(), Quaternion.identity);
    }

    private Vector2 GetRandomPositionWithInSpawnArea()
    {
        Vector2 output = transform.position;

        output.x += GetRandomFromHalfRange(spawnSize.x);
        output.y += GetRandomFromHalfRange(spawnSize.y);

        return output;
    }

    private float GetRandomFromHalfRange(float range)
    {
        float half = range / 2;
        return Random.Range(-half, half);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnSize);
    }
}