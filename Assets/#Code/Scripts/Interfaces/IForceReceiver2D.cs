using UnityEngine;

public interface IForceReceiver2D // TODO: Реализовать в контроллере передвижения для жестких тел
{
    /// <summary>
    /// Перемещение от текущей позиции
    /// </summary>
    /// <param name="direction">Вектор движения</param>
    void Move(Vector2 direction);
    /// <summary>
    /// Добавление движущей силы
    /// </summary>
    /// <param name="force">Вектор движения</param>
    /// <param name="mode">Режим движения</param>
    void AddForce(Vector2 force, ForceMode2D mode);
    void AddForceAtPosition(Vector2 force, Vector2 position, ForceMode2D mode);
}
