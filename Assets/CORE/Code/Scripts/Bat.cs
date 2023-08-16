using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Bat : Enemy
{
    private enum BatState { Idle, Move, Attack, Rotation }


    [Space][Header("Bat Settings")]

    [Header("Moving system")]
    [SerializeField] private Vector2 finishPos;
    [SerializeField] private float rotateSeconds;

    [Header("Attack system")]
    [SerializeField] private float attackDistance;

    [Header("Bat class Components")]
    [SerializeField] private BoxCollider2D boxCollider2D;

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    private bool _isMoveBack;
    private Vector2 _startPos;
    private Vector2 _destination;
    private BatState _state;


    protected override void Start()
    {
        base.Start();

        boxCollider2D = GetComponent<BoxCollider2D>();
        Initialize();
    }

    void FixedUpdate()
    {
        if (_state != BatState.Rotation)
        {
            //Debug.Log(Vector2.Distance(playerTransform.position, this.transform.position));
            Vector2 currentPosition = this.transform.position;
            Vector2 direction = (_destination - currentPosition).normalized;
            physic.velocity = direction * moveSpeed * Time.fixedDeltaTime;

            if (UnityUtils.Approximately(currentPosition, _destination))
                StartCoroutine(SlowRotate());
        }
    }


    private void Initialize()
    {
        _startPos = this.transform.position;
        _destination = finishPos;

        Vector2 direction = (_destination - _startPos).normalized;
        if (direction.x > 0) spriteRenderer.flipX = false;
        else spriteRenderer.flipX = true;
    }


    private IEnumerator SlowRotate()
    {
        _state = BatState.Rotation;
        physic.velocity = Vector2.zero;

        yield return new WaitForSeconds(rotateSeconds / 1.5f);

        _destination = _isMoveBack ? finishPos : _startPos;
        _isMoveBack = !_isMoveBack;
        spriteRenderer.flipX = _destination.x < this.transform.position.x ? true : false;

        yield return new WaitForSeconds(rotateSeconds / 3f);

        _state = BatState.Idle;
    }
}
