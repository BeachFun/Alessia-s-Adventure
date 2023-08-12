using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Characteristics")]
    [SerializeField] private int health = 5;
    [SerializeField] private int power = 3;
    [SerializeField] private int protection = 1;

    public virtual void Attack()
    {

    }
}
