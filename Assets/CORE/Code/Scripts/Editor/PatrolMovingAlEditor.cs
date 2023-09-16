using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(PatrolMovingAl))]
public class PatrolMovingAlEditor : Editor
{
    private PatrolMovingAl movingAl;
    private SerializedObject serializedMovingAl;

    private SerializedProperty isPaused;
    private SerializedProperty useGravity;
    private SerializedProperty jumpForce;
    private SerializedProperty movementAlgorithm;
    private SerializedProperty isAvoidObstacles;
    private SerializedProperty startPosition;
    private SerializedProperty endPosition;
    private SerializedProperty routePoints;
    private SerializedProperty isMoveBack;
    private SerializedProperty rotateSeconds;
    private SerializedProperty moveSpeed;

    public void OnEnable()
    {
        movingAl = (PatrolMovingAl)target;
        serializedMovingAl = new SerializedObject(movingAl);

        isPaused = serializedMovingAl.FindProperty("isPaused");
        useGravity = serializedMovingAl.FindProperty("useGravity");
        jumpForce = serializedMovingAl.FindProperty("jumpForce");
        movementAlgorithm = serializedMovingAl.FindProperty("movementAlgorithm");
        isAvoidObstacles = serializedMovingAl.FindProperty("isAvoidObstacles");
        startPosition = serializedMovingAl.FindProperty("startPosition");
        endPosition = serializedMovingAl.FindProperty("endPosition");
        routePoints = serializedMovingAl.FindProperty("routePoints");
        isMoveBack = serializedMovingAl.FindProperty("isMoveBack");
        rotateSeconds = serializedMovingAl.FindProperty("rotateSeconds");
        moveSpeed = serializedMovingAl.FindProperty("moveSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedMovingAl.Update();

        EditorGUILayout.PropertyField(isPaused, new GUIContent("Включен"));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Настройка", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(movementAlgorithm, new GUIContent("Способ перемещения"));

        if (movementAlgorithm.enumValueIndex == (int)MovementAlgorithm.PointToPoint)
        {
            EditorGUILayout.PropertyField(startPosition, new GUIContent("Начальная позиция"));
            EditorGUILayout.PropertyField(endPosition, new GUIContent("Конечная позиция"));
            EditorGUILayout.PropertyField(isMoveBack, new GUIContent("К начальной точке"));
        }
        else if (movementAlgorithm.enumValueIndex == (int)MovementAlgorithm.StartToPoint)
        {
            EditorGUILayout.PropertyField(endPosition, new GUIContent("Конечная позиция"));
        }
        else if (movementAlgorithm.enumValueIndex == (int)MovementAlgorithm.Route)
        {
            EditorGUILayout.PropertyField(routePoints, new GUIContent("Точки маршрута"));
            EditorGUILayout.PropertyField(isMoveBack, new GUIContent("Route reverse"));
        }

        if (movementAlgorithm.enumValueIndex != (int)MovementAlgorithm.EdgeToEdge)
        {
            EditorGUILayout.PropertyField(isAvoidObstacles, new GUIContent("Избегать препятствия"));
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(rotateSeconds, new GUIContent("Время разворота"));
        EditorGUILayout.PropertyField(moveSpeed, new GUIContent("Скорость перемещения"));

        if (useGravity.boolValue)
        {
            EditorGUILayout.PropertyField(jumpForce, new GUIContent("Сила прыжка"));
        }

        EditorGUILayout.PropertyField(useGravity, new GUIContent("Гравитация"));

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