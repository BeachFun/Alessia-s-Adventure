using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Enemy : Character
{
    public enum Bool { True, False}


    [Space][Header("Enemy Settings")]

    [Header("Characteristics")]
    [SerializeField] protected int hp = 5;
    [SerializeField] protected int atk = 3;
    [SerializeField] protected int def = 1;

    [Header("Enemy Settings")]
    [Tooltip("Время между атаками")]
    [SerializeField] protected float timeBetweenAttacks = 1;

    [Header("Enemy class Components")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D physic;
    [SerializeField] protected Collider2D _collider;


    protected bool _isBusy;
    protected Coroutine _freezeRotation;


    protected Bool IsBusy
    {
        get => _isBusy ? Bool.True : Bool.False;
        set => _isBusy = value == Bool.True ? true : false;
    }


    protected virtual void OnEnable()
    {
        _freezeRotation = StartCoroutine(FreezeRotationLoop());
    }

    protected virtual void OnDisable()
    {
        StopCoroutine(_freezeRotation);
    }


    public virtual void Hurt(int numAtk)
    {
        hp = numAtk > def ? hp - (numAtk - def) : hp;

        if (hp < 0) Death();
        else animator.SetTrigger("hit");
    }

    protected virtual void Death()
    {
        Destroy(this.gameObject); // TODO: Улучшить метод смерти
    }

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    protected IEnumerator FreezeRotationLoop()
    {
        while (true)
        {
            this.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }
}
