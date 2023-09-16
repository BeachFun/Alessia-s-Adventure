using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(AutoShootController2D))]
internal class AutoShootController2DEditor : Editor
{
    private AutoShootController2D attackController;
    private SerializedObject serializedAttackController;

    private SerializedProperty shootOn;
    private SerializedProperty shootDamage;
    private SerializedProperty shootMinDistance;
    private SerializedProperty shootMaxDistance;
    private SerializedProperty timeBetweenShoots;
    private SerializedProperty shootSpeed;
    private SerializedProperty shootAnimationName;
    private SerializedProperty enemyTag;
    private SerializedProperty detectionMode;
    private SerializedProperty projectile;
    private SerializedProperty enemyTransform;

    public void OnEnable()
    {
        attackController = target as AutoShootController2D;
        serializedAttackController = new SerializedObject(attackController);

        shootOn = serializedAttackController.FindProperty("shootOn");
        shootDamage = serializedAttackController.FindProperty("shootDamage");
        shootMinDistance = serializedAttackController.FindProperty("shootMinDistance");
        shootMaxDistance = serializedAttackController.FindProperty("shootMaxDistance");
        timeBetweenShoots = serializedAttackController.FindProperty("timeBetweenShoots");
        shootSpeed = serializedAttackController.FindProperty("shootSpeed");
        shootAnimationName = serializedAttackController.FindProperty("shootAnimationName");
        enemyTag = serializedAttackController.FindProperty("enemyTag");
        detectionMode = serializedAttackController.FindProperty("detectionMode");
        projectile = serializedAttackController.FindProperty("projectile");
        enemyTransform = serializedAttackController.FindProperty("enemyTransform");
    }

    public override void OnInspectorGUI()
    {
        serializedAttackController.Update();

        EditorGUILayout.PropertyField(shootOn, new GUIContent("Включен"));
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Настройка", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(shootDamage, new GUIContent("урон"));

        if (detectionMode.enumValueIndex == (int)VisionMode.Known)
        {
            EditorGUILayout.PropertyField(shootMinDistance, new GUIContent("min дистанция", "Минимальная дистанция для выстрела"));
            EditorGUILayout.PropertyField(shootMaxDistance, new GUIContent("max дистанция", "Максимальная дистанция для выстрела"));
        }

        EditorGUILayout.PropertyField(timeBetweenShoots, new GUIContent("время перезарядки"));
        EditorGUILayout.PropertyField(shootSpeed, new GUIContent("скорость выстрела"));
        EditorGUILayout.PropertyField(shootAnimationName, new GUIContent("название тригера", "Название триггера для вызова анимации выстрела в аниматоре"));
        EditorGUILayout.PropertyField(enemyTag, new GUIContent("тэг врага"));
        EditorGUILayout.PropertyField(detectionMode, new GUIContent("способ обнаружения"));
        EditorGUILayout.PropertyField(projectile, new GUIContent("снаряд"));
        EditorGUILayout.Space();

        if (detectionMode.enumValueIndex == (int)VisionMode.Known)
        {
            EditorGUILayout.PropertyField(enemyTransform, new GUIContent("ссылка на врага"));
        }

        if (serializedAttackController.hasModifiedProperties)
        {
            serializedAttackController.ApplyModifiedProperties();
            SetObjectDirty(attackController.gameObject);
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
