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
        movingAl.isOn = EditorGUILayout.Toggle("Включен", movingAl.isOn);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Настройка", EditorStyles.boldLabel);
        movingAl.mode = (PatrolMovingAl.MovementMode)EditorGUILayout.EnumPopup("Режим перемещения", movingAl.mode);
        movingAl.movementAlgoritm = (PatrolMovingAl.MovementAlgoritm)EditorGUILayout.EnumPopup("Вариант перемещения", movingAl.movementAlgoritm);

        if (movingAl.movementAlgoritm == PatrolMovingAl.MovementAlgoritm.PositionToPosition)
        {
            movingAl.startPosition = EditorGUILayout.Vector2Field("Начальная позиция", movingAl.startPosition);
            movingAl.endPosition = EditorGUILayout.Vector2Field("Конечная позиция", movingAl.endPosition);
        }

        if (movingAl.movementAlgoritm == PatrolMovingAl.MovementAlgoritm.StartToPosition)
        {
            movingAl.endPosition = EditorGUILayout.Vector2Field("Конечная позиция", movingAl.endPosition);
        }

        EditorGUILayout.Space();
        movingAl.rotateSeconds = EditorGUILayout.FloatField("Время разворота", movingAl.rotateSeconds);
        movingAl.moveSpeed = EditorGUILayout.FloatField("Скорость перемещения", movingAl.moveSpeed);

        if (GUI.changed) SetObjectDirty(movingAl.gameObject);
    }

    public static void SetObjectDirty(GameObject obj)
    {
        EditorUtility.SetDirty(obj);
        EditorSceneManager.MarkSceneDirty(obj.scene);
    }
}