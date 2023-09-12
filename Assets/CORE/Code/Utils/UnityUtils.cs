using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityUtils : MonoBehaviour
{
    public static bool IsAnimationPlaying(Animator animator, string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }

    /// <summary>
    /// Возвращает длительность текущей анимации в переданном аниматоре
    /// </summary>
    /// <returns>Длителеность выраженная в секундах</returns>
    public static float AnimationPlayDuration(Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }


    /// <summary>
    /// Проверяет приблизительно равны ли две координаты друг другу
    /// </summary>
    public static bool Approximately(Vector2 point1, Vector2 point2) =>
        Mathf.Approximately(Mathf.Round(point1.x), Mathf.Round(point2.x)) && 
        Mathf.Approximately(Mathf.Round(point1.y), Mathf.Round(point2.y));

    /// <summary>
    /// Проверяет приблизительно равны ли две координаты друг другу по определенным осям декартовой системы
    /// </summary>
    public static bool Approximately(Vector2 point1, Vector2 point2, SnapAxis2D axis)
    {
        switch (axis)
        {
            case SnapAxis2D.All: return Approximately(point1, point2);
            case SnapAxis2D.X: return Mathf.Approximately(Mathf.Round(point1.x), Mathf.Round(point2.x));
            case SnapAxis2D.Y: return Mathf.Approximately(Mathf.Round(point1.y), Mathf.Round(point2.y));

            default: return false;
        }
    }

    public static bool ApproximatelyEqual(float a, float b, float tolerance)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }

    public static Vector2 Opposite(Vector2 vector)
    {
        return new Vector2(-vector.x, -vector.y);
    }
}

public enum SnapAxis2D { None, All, X, Y }
