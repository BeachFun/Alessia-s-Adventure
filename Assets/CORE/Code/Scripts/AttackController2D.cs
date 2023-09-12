using System.Linq;
using System.Collections;
using UnityEngine;
using Action = System.Action;

[RequireComponent(typeof(Animator))]

public class AttackController2D : MonoBehaviour
{
    [SerializeField] private bool attackOn;
    [SerializeField] private bool isAttackAllOnZone;
    [SerializeField] private int attackDamage;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float aggressionDistance;
    [SerializeField] private float playerCheckDistance;
    [SerializeField] private string enemyTag;
    [SerializeField] private AttackZonesData[] attackZones;

    private bool _attackOn;
    private bool _isRotate;
    private int _attackZoneIndex;
    private Vector2[] _directionsToZones;
    private Animator _animator;


    public bool AttackOn { get => attackOn; set => attackOn = value; }
    public bool IsRotating { get => _isRotate; set => _isRotate = value; }


    public event Action ActionBeforeAttack;
    public event Action ActionAfterAttack;


    private void Awake()
    {
        _directionsToZones = new Vector2[attackZones.Length];

        for (int i = 0; i < attackZones.Length; i++)
        {
            _directionsToZones[i] = transform.position.x + attackZones[i].Distance > transform.position.x 
                ? Vector2.right 
                : Vector2.left;
        }
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!AttackOn || !_attackOn) return;

        for (int i = 0; i < attackZones.Length; i++)
        {
            Vector2 direction = IsRotating ? -_directionsToZones[i] : _directionsToZones[i];

            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, attackZones[i].ZoneSize, 0, direction)
                .Where(e => e.transform.tag == enemyTag)
                .ToArray();

            if (hits.Length > 0)
            {
                ActionBeforeAttack.Invoke();
                _attackOn = false;
                _attackZoneIndex = i;
                Attack();
                return;
            }
        }

        _attackZoneIndex = -1;
    }

    private void OnDrawGizmos()
    {
        if (attackZones.Length == 0) return;

        Gizmos.color = Color.red;

        for (int i = 0; i < attackZones.Length; i++)
        {
            Vector3 center = transform.position;
            center.x += IsRotating ? -(attackZones[i].Distance / 2) : (attackZones[i].Distance / 2);

            Gizmos.DrawWireCube(center, attackZones[i].ZoneSize);
        }
    }

    public void Attack()
    {
        if (_attackZoneIndex != -1) _animator.SetTrigger(attackZones[_attackZoneIndex].AnimationName);
    }

    private void Damage()
    {
        Vector2 direction = IsRotating ? -_directionsToZones[_attackZoneIndex] : _directionsToZones[_attackZoneIndex];

        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, attackZones[_attackZoneIndex].ZoneSize, 0, direction)
                .Where(e => e.transform.tag == enemyTag)
                .ToArray();

        if (hits.Length > 0)
            if (isAttackAllOnZone) DamageAllOnZone(hits);
            else DamageOneOnZone(hits);

        StartCoroutine(AttackRecoverRoutine());
    }

    private void DamageOneOnZone(RaycastHit2D[] hits)
    {
        Transform enemyTransform = hits[0].transform;

        enemyTransform.GetComponent<Character>().Hurt(attackDamage);
    }

    private void DamageAllOnZone(RaycastHit2D[] hits)
    {
        Transform[] enemiesTransforms = hits.Select(e => e.transform).ToArray();

        enemiesTransforms.ToList().ForEach(e => e.GetComponent<Character>().Hurt(attackDamage));
    }

    private IEnumerator AttackRecoverRoutine()
    {
        ActionAfterAttack.Invoke();

        yield return new WaitForSeconds(timeBetweenAttacks);

        _attackOn = true;
    }
}
