using System.Collections;
using UnityEngine;
using Action = System.Action;

public class ShootSystem2D : MonoBehaviour
{
    [SerializeField] private protected bool shootOn;
    [SerializeField] private protected int shootDamage;
    [SerializeField] private protected float timeBetweenShoots;
    [SerializeField] private protected float shootSpeed;
    [SerializeField] private protected string shootAnimationName;
    [SerializeField] private protected Projectile2D projectile;

    private protected bool _shootOn;
    private protected Vector3 _enemyDirection;
    private float _speedMultiply;

    public bool ShootOn { get => shootOn; set => shootOn = value; }


    public event Action ActionAfterShoot;


    public void ReleaseProjectile(Vector2 direction, float speedMultiply)
    {
        if (!ShootOn || !_shootOn) return;
        _shootOn = false;

        _enemyDirection = direction;
        _speedMultiply = speedMultiply;

        ReleaseProjectile();
    }

    // Метод для системы Mecanim. Вызывать в момент выстрела.
    private protected void ReleaseProjectile()
    {
        Projectile2D projectile = Instantiate(this.projectile, this.transform.position, new Quaternion(0f, 0f, 0f, 0f));
        projectile.Power = shootDamage;
        projectile.AddForce(_enemyDirection, shootSpeed * _speedMultiply);

        StartCoroutine(ShootRecoverRoutine());
    }

    private IEnumerator ShootRecoverRoutine()
    {
        ActionAfterShoot.Invoke();

        yield return new WaitForSeconds(timeBetweenShoots);

        _shootOn = true;
    }
}
