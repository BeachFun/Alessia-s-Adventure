using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(PatrolMovingAl))]
public class PatrolMovingAlEditor : Editor
{
    private PatrolMovingAl movingAl;
    private SerializedObject serializedMovingAl;

    private SerializedProperty isOn;
    private SerializedProperty mode;
    private SerializedProperty movementAlgoritm;
    private SerializedProperty startPosition;
    private SerializedProperty endPosition;
    private SerializedProperty rotateSeconds;
    private SerializedProperty moveSpeed;

    public void OnEnable()
    {
        movingAl = (PatrolMovingAl)target;
        serializedMovingAl = new SerializedObject(movingAl);

        isOn = serializedMovingAl.FindProperty("isOn");
        mode = serializedMovingAl.FindProperty("mode");
        movementAlgoritm = serializedMovingAl.FindProperty("movementAlgoritm");
        startPosition = serializedMovingAl.FindProperty("startPosition");
        endPosition = serializedMovingAl.FindProperty("endPosition");
        rotateSeconds = serializedMovingAl.FindProperty("rotateSeconds");
        moveSpeed = serializedMovingAl.FindProperty("moveSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedMovingAl.Update();

        EditorGUILayout.PropertyField(isOn, new GUIContent("Включен"));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Настройка", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mode, new GUIContent("Режим перемещения"));
        EditorGUILayout.PropertyField(movementAlgoritm, new GUIContent("Вариант перемещения"));

        if (movementAlgoritm.enumValueIndex == (int)PatrolMovingAl.MovementAlgoritm.PositionToPosition)
        {
            EditorGUILayout.PropertyField(startPosition, new GUIContent("Начальная позиция"));
            EditorGUILayout.PropertyField(endPosition, new GUIContent("Конечная позиция"));
        }
        else if (movementAlgoritm.enumValueIndex == (int)PatrolMovingAl.MovementAlgoritm.StartToPosition)
        {
            EditorGUILayout.PropertyField(endPosition, new GUIContent("Конечная позиция"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(rotateSeconds, new GUIContent("Время разворота"));
        EditorGUILayout.PropertyField(moveSpeed, new GUIContent("Скорость перемещения"));

        if (serializedMovingAl.hasModifiedProperties)
        {
            serializedMovingAl.ApplyModifiedProperties();
            SetObjectDirty(movingAl.gameObject);
        }
    }

    public static void SetObjectDirty(GameObject obj)
    {
        try
        {
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(obj.scene);
        }
        catch { }
    }
}