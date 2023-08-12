using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Enemy2Controller : Enemy
{
    private float speed = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        yield return null;

    }
}
