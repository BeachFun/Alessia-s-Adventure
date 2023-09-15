using UnityEngine;

[System.Serializable]
internal struct AttackData
{
    [SerializeField] private string attackName;
    [SerializeField] private int id;

    public string AttackName
    {
        get => attackName;
        set => attackName = value;
    }

    public int Id
    {
        get => id;
        set => id = value;
    }
}
