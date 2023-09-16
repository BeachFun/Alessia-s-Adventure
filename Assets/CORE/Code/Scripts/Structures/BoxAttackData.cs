using UnityEngine;

[System.Serializable]
internal struct BoxAttackData
{
    [SerializeField] private float distance;
    [SerializeField] private Vector2 zoneSize;
    [SerializeField] private int id;
    [SerializeField] private string nameAnimatorProperty;
    //[SerializeField] private AnimatorPropertyType propertyType;

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

    public int Id
    {
        get => id;
        set => id = value;
    }

    public string NameAnimatorProperty
    {
        get => nameAnimatorProperty;
        set => nameAnimatorProperty = value;
    }

    //public AnimatorPropertyType PropertyType
    //{
    //    get => propertyType;
    //    set => propertyType = value;
    //}
}

public enum AnimatorPropertyType
{
    Int,
    Float,
    Bool,
    Trigger
}