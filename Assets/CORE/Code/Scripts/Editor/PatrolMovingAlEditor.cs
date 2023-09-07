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
        EditorGUILayout.LabelField("Настройка");
        movingAl.MovementMode = (PatrolMovingAl.Mode)EditorGUILayout.EnumPopup("Режим перемещения", movingAl.MovementMode);

        if (movingAl.MovementMode == PatrolMovingAl.Mode.PointToPoint)
        {
            movingAl.StartPosition = EditorGUILayout.Vector2Field("Начальная позиция", movingAl.StartPosition);
            movingAl.EndPosition = EditorGUILayout.Vector2Field("Конечная позиция", movingAl.EndPosition);
        }

        if (movingAl.MovementMode == PatrolMovingAl.Mode.StartToPoint)
        {
            movingAl.EndPosition = EditorGUILayout.Vector2Field("Конечная позиция", movingAl.EndPosition);
        }

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