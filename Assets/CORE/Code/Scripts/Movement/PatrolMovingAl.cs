using System.Linq;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class PatrolMovingAl : MovementController2D
{
    private const float obstacleDistance = .5f;

    [SerializeField] private protected MovementAlgorithm movementAlgorithm;
    [SerializeField] private protected bool isAvoidObstacles;
    [SerializeField] private protected Vector2 startPosition;
    [SerializeField] private protected Vector2 endPosition;
    [SerializeField] private protected Vector2[] routePoints;
    [SerializeField] private protected bool isMoveBack;
    [SerializeField] private protected float rotateSeconds;

    private protected bool _fastMoveOn;
    private protected bool _isMovingOn = true;
    private protected int _routeIndex = 0;
    private protected Vector2 _destination;
    private protected SpriteRenderer _spriteRenderer;

    private readonly string[] _tagsToSkip = { "Area", "Projectile", "Player", "Collectable" };

    public bool IsRotating
    {
        get;
        set;
    } = true;
    public bool FastMoveOn
    {
        get => _fastMoveOn;
        set => _fastMoveOn = value;
    }
    public float Speed
    {
        get => _physic.velocity.magnitude / Time.fixedDeltaTime;
    }
    private bool IsReachedDestinationPosition
    {
        get
        {
            Vector2 currentPosition = transform.position;

            if (UnityUtils.Approximately(currentPosition, _destination))
                return true;

            if (isAvoidObstacles)
            {
                return Physics2D.RaycastAll(currentPosition, _destination - currentPosition, obstacleDistance)
                    .Where(e => e.transform.tag != this.tag && !_tagsToSkip.Contains(e.transform.tag))
                    .Count() > 0;
            }

            return false;
        }
    }


    private protected void Awake()
    {
        if (movementAlgorithm == MovementAlgorithm.StartToPoint) startPosition = transform.position;
        if (movementAlgorithm != MovementAlgorithm.EdgeToEdge) _destination = endPosition;

        if (isMoveBack) _routeIndex = routePoints.Length - 1;
    }

    private protected override void Start()
    {
        base.Start();

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer is null) _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (isPaused || !_isMovingOn) return;

        Vector2 direction = Vector2.zero;

        if (movementAlgorithm == MovementAlgorithm.EdgeToEdge)
        {
            Vector2? groundPoint = CalcGroundPoint();
            Vector2? farGroundPoint = CalcFarGroundPoint();

            if (groundPoint is null || farGroundPoint is null || 
                !UnityUtils.ApproximatelyEqual(groundPoint.Value.y, farGroundPoint.Value.y, 0.15f))
            {
                StartCoroutine(SlowRotate());
            }
            else
            {
                if (_spriteRenderer.flipX)
                    direction = new Vector2(-(moveSpeed * Time.fixedDeltaTime), 0f);
                else
                    direction = new Vector2((moveSpeed * Time.fixedDeltaTime), 0f);
            }
        }
        else
        {
            if (IsReachedDestinationPosition)
            {
                if (movementAlgorithm == MovementAlgorithm.Route)
                {
                    bool isDidRotate = false;

                    if (!isMoveBack && _routeIndex == routePoints.Length - 1)
                    {
                        isMoveBack = true;
                        isDidRotate = true;
                    }
                    if (isMoveBack && _routeIndex == 0)
                    {
                        isMoveBack = false;
                        isDidRotate = true;
                    }

                    _routeIndex += isMoveBack ? -1 : 1;

                    _destination = routePoints[_routeIndex];

                    if (isDidRotate) StartCoroutine(SlowRotate());
                }
                else
                {
                    StartCoroutine(SlowRotate());
                }
            }
            else
            {
                if (movementAlgorithm == MovementAlgorithm.Route)
                {
                    if (direction.x < 0)
                    {
                        _spriteRenderer.flipX = true;
                    }
                    else if (direction.x > 0)
                    {
                        _spriteRenderer.flipX = false;
                    }
                }

                Vector2 currentPosition = transform.position;
                Vector2 moveDirection = (_destination - currentPosition).normalized;

                direction = moveDirection * moveSpeed * Time.fixedDeltaTime;
            }
        }

        if (_fastMoveOn) direction *= 2;

        base.Move(direction);
    }

    private Vector2? CalcGroundPoint()
    {
        Vector2 bottomBodyPoint = new Vector2(this.transform.position.x, _collider.bounds.min.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(bottomBodyPoint, Vector2.down)
            .Where(e => e.transform.tag != this.tag && !_tagsToSkip.Contains(e.transform.tag))
            .ToArray();

        if (hits.Length > 0) return hits.First().point;
        else return null;
    }

    private Vector2? CalcFarGroundPoint()
    {
        Vector2 forwardBodyPoint, direction;

        if (_spriteRenderer.flipX)
        {
            forwardBodyPoint = new Vector2(_collider.bounds.min.x, _collider.bounds.max.y);
            direction = new Vector2(-0.3f, -1f);
        }
        else
        {
            forwardBodyPoint = new Vector2(_collider.bounds.max.x, _collider.bounds.max.y);
            direction = new Vector2(0.3f, -1f);
        }

        RaycastHit2D[] hits = Physics2D.RaycastAll(forwardBodyPoint, direction)
            .Where(e => e.transform.tag != this.tag && !_tagsToSkip.Contains(e.transform.tag))
            .ToArray();

        if (hits.Length > 0) return hits.First().point;
        else return null;
    }

    private IEnumerator SlowRotate()
    {
        base.StopMovement(SnapAxis2D.All);

        if (IsRotating)
        {
            _isMovingOn = false;
            yield return new WaitForSeconds(rotateSeconds / 1.5f);

            _spriteRenderer.flipX = !_spriteRenderer.flipX;

            if (movementAlgorithm == MovementAlgorithm.StartToPoint ||
                movementAlgorithm == MovementAlgorithm.PointToPoint)
            {
                _destination = isMoveBack ? endPosition : startPosition;
                isMoveBack = !isMoveBack;
            }

            yield return new WaitForSeconds(rotateSeconds / 3f);
            _isMovingOn = true;
        }
    }
}

public enum MovementAlgorithm
{
    EdgeToEdge,
    StartToPoint,
    PointToPoint,
    Route
}
