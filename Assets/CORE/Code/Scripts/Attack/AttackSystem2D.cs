using System.Linq;
using System.Collections;
using UnityEngine;
using Action = System.Action;

public class AttackSystem2D : MonoBehaviour
{
    [SerializeField] private protected int attackDamage;
    [SerializeField] private protected float timeBetweenAttacks;
    [SerializeField] private protected string enemyTag;
    [SerializeField] private protected AttackZonesData[] attackZones;
    [SerializeField] private protected bool isAttackAllOnZone;

    private protected bool _attackOn;
    private protected bool _isRotated;
    private protected int _attackZoneIndex;


    public bool IsRotated
    {
        get => _isRotated;
        set => _isRotated = value;
    }
    public int CountZones
    {
        get => attackZones.Length;
    }


    public event Action ActionAfterAttack;


    private void OnDrawGizmos()
    {
        if (attackZones.Length == 0) return;

        Gizmos.color = Color.red;

        for (int i = 0; i < attackZones.Length; i++)
        {
            Vector3 center = transform.position;
            center.x += IsRotated ? -(attackZones[i].Distance / 2) : (attackZones[i].Distance / 2);

            Gizmos.DrawWireCube(center, attackZones[i].ZoneSize);
        }
    }

    public virtual void Attack(int indexZone)
    {
        if (!_attackOn) return;
        _attackOn = false;

        _attackZoneIndex = indexZone;

        Damage();
    }

    // Метод для системы Mecanim. Вызывать в момент удара.
    private protected void Damage()
    {
        RaycastHit2D[] hits = CheckZone(_attackZoneIndex);

        if (hits.Length > 0)
            if (isAttackAllOnZone) DamageAllOnZone(hits);
            else DamageOneOnZone(hits);

        StartCoroutine(AttackRecoverRoutine());
    }

    private protected RaycastHit2D[] CheckZone(int indexZone)
    {
        Vector2 direction = IsRotated ? Vector2.left : Vector2.right;

        return Physics2D.BoxCastAll(transform.position, attackZones[indexZone].ZoneSize, 0, direction)
                .Where(e => e.transform.tag == enemyTag)
                .ToArray();
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
