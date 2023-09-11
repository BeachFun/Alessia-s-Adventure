using UnityEngine;

[System.Serializable]
internal struct AttackZonesData
{
    [SerializeField] private float distance;
    [SerializeField] private Vector2 zoneSize;
    [SerializeField] private string animationName;

    public float Distance
    {
        get => distance;
        set => distance = value;
    }

    public Vector2 ZoneSize
    {
        get => zoneSize;
        set => zoneSize = value;
    }

    public string AnimationName
    {
        get => animationName;
        set => animationName = value;
    }
}