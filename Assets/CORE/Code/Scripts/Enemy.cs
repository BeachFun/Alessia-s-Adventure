using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Enemy : MonoBehaviour
{
    [Space][Header("Enemy Settings")]

    [Header("Characteristics")]
    [SerializeField] protected int hp = 5;
    [SerializeField] protected int atk = 3;
    [SerializeField] protected int def = 1;

    [Header("Enemy Settings")]
    [Tooltip("Скорость нанесения урона (удара)")]
    [SerializeField] protected float hurtSpeed = 1;
    [Tooltip("Время между атаками")]
    [SerializeField] protected float timeBetweenAttacks = 1;
    [SerializeField] protected float moveSpeed = 0f;

    [Header("Enemy class Components")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D physic;


    protected bool isBusy;


    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        physic = GetComponent<Rigidbody2D>();
    }


    public virtual void Hurt(int numAtk)
    {
        hp -= (numAtk - def);

        isBusy = true;
        animator.SetTrigger("hit");
        isBusy = false;
    }

    protected virtual void Death()
    {

    }

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}

public enum MoveDirection { Left, Right }
