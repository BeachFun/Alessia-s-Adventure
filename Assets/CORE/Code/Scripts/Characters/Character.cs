using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MovementController2D))]

public abstract class Character : MonoBehaviour
{
    [Header("Characteristics")]
    [SerializeField] private protected int hp = 5;
    [SerializeField] private protected int def = 1;

    private protected SpriteRenderer _spriteRenderer;
    private protected MovementController2D _movement;

    private protected virtual void Start()
    {
        _movement = GetComponent<MovementController2D>();
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
