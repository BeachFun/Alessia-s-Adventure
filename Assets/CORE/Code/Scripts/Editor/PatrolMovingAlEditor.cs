using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(PatrolMovingAl))]
public class PatrolMovingAlEditor : Editor
{
    private PatrolMovingAl movingAl;

    public void OnEnable()
    {
        movingAl = (PatrolMovingAl)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        movingAl.IsOn = EditorGUILayout.Toggle("Включен", movingAl.IsOn);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Настройка", EditorStyles.boldLabel);
        movingAl.MoveMode = (PatrolMovingAl.MovementMode)EditorGUILayout.EnumPopup("Режим перемещения", movingAl.MoveMode);
        movingAl.MoveAlgoritm = (PatrolMovingAl.MovementAlgoritm)EditorGUILayout.EnumPopup("Вариант перемещения", movingAl.MoveAlgoritm);

        if (movingAl.MoveAlgoritm == PatrolMovingAl.MovementAlgoritm.EdgeToEdge &&
            movingAl.MoveMode == PatrolMovingAl.MovementMode.Walking)
        {
            movingAl.MoveDirection = EditorGUILayout.Vector2Field("Направление движения", movingAl.MoveDirection);
        }

        if (movingAl.MoveAlgoritm == PatrolMovingAl.MovementAlgoritm.PositionToPosition)
        {
            movingAl.StartPosition = EditorGUILayout.Vector2Field("Начальная позиция", movingAl.StartPosition);
            movingAl.EndPosition = EditorGUILayout.Vector2Field("Конечная позиция", movingAl.EndPosition);
        }

        if (movingAl.MoveAlgoritm == PatrolMovingAl.MovementAlgoritm.StartToPosition)
        {
            movingAl.EndPosition = EditorGUILayout.Vector2Field("Конечная позиция", movingAl.EndPosition);
        }

        EditorGUILayout.Space();
        movingAl.RotateSeconds = EditorGUILayout.FloatField("Время разворота", movingAl.RotateSeconds);
        movingAl.MoveSpeed = EditorGUILayout.FloatField("Скорость перемещения", movingAl.MoveSpeed);

        if (GUI.changed) SetObjectDirty(movingAl.gameObject);
    }

    public static void SetObjectDirty(GameObject obj)
    {
        EditorUtility.SetDirty(obj);
        EditorSceneManager.MarkSceneDirty(obj.scene);
    }
}