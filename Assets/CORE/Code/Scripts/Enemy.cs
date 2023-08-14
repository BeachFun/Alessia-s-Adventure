using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Characteristics")]
    [SerializeField] protected int hp = 5;
    [SerializeField] protected int atk = 3;
    [SerializeField] protected int def = 1;

    [Header("Enemy Settings")]
    [SerializeField] protected float timeBetweenAttacks;
    [SerializeField] protected float atkSpeed = 1;
    [SerializeField] protected float moveSpeed = 0f;

    [Header("Enemy class Components")]
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D physic;
    [SerializeField] protected Collider2D collider2D;


    protected bool isBusy;


    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        physic = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
    }


    public virtual void Hurt(int number)
    {
        hp -= (number - def);
    }

    protected virtual void Death()
    {

    }

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
