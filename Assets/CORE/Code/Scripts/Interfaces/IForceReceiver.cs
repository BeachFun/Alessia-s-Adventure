using UnityEngine;

public interface IForceReceiver // TODO: Реализовать в контроллере передвижения для жестких тел
{
    void Move(Vector3 mv);
    void AddForce(Vector3 force, ForceMode mode);
    void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode);
    void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier, ForceMode mode);
}
