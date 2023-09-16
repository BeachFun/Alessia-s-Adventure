using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public abstract class Character : MonoBehaviour
{
    [Header("Characteristics")]
    [SerializeField] private protected int hp = 5;
    [SerializeField] private protected int def = 1;

    private protected SpriteRenderer _spriteRenderer;

    private protected virtual void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public abstract void Attack();

    public abstract void Hurt(int damage);

    public virtual void Dieth()
    {
        Destroy(this);
    }

    public virtual void Flip()
    {
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }
}
