using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Characteristics")]
    [SerializeField] private int health = 5;
    [SerializeField] private int power = 3;
    [SerializeField] private int defence = 1;

    [Header("Enemy class Components")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D physic;
    [SerializeField] protected Collider2D collider2D;

    [Header("References")]
    [SerializeField] private Transform player;


    protected bool isBusy;


    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        physic = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
    }


    public virtual void Hurt()
    {
        health--;
    }

    protected virtual void Death()
    {

    }

    public void Flip()
    {
        Vector3 enemyScale = transform.localScale;
        enemyScale.x *= -1;
        transform.localScale = enemyScale;
    }
}
