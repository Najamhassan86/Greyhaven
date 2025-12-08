#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Hanzzz.MeshDemolisher
{

[CustomEditor(typeof(PointGenerator))]
[CanEditMultipleObjects]
public class PointGeneratorEditor : Editor
{
    private PointGenerator pointGenerator;

    private SerializedProperty generateOnChange;
    private SerializedProperty pointPrefab;
    private SerializedProperty pointsParent;
    private SerializedProperty pointCount;
    private SerializedProperty pointScale;
    private SerializedProperty pointDomainType;
    private SerializedProperty length0;
    private SerializedProperty length1;
    private SerializedProperty length2;
    private SerializedProperty domainGameObject;

    private void OnEnable()
    {
        //Null check to prevent errors when object is destroyed
        if (target == null)
        {
            return;
        }

        pointGenerator = (PointGenerator)target;

        //Additional null check
        if (pointGenerator == null)
        {
            return;
        }

        //Check if serializedObject is valid
        if (serializedObject == null || serializedObject.targetObject == null)
        {
            return;
        }

        pointGenerator.OnEnable();

        generateOnChange = serializedObject.FindProperty(nameof(pointGenerator.generateOnChange));
        pointPrefab = serializedObject.FindProperty(nameof(pointGenerator.pointPrefab));
        pointsParent = serializedObject.FindProperty(nameof(pointGenerator.pointsParent));
        pointCount = serializedObject.FindProperty(nameof(pointGenerator.pointCount));
        pointScale = serializedObject.FindProperty(nameof(pointGenerator.pointScale));
        pointDomainType = serializedObject.FindProperty(nameof(pointGenerator.pointDomainType));
        length0 = serializedObject.FindProperty(nameof(pointGenerator.length0));
        length1 = serializedObject.FindProperty(nameof(pointGenerator.length1));
        length2 = serializedObject.FindProperty(nameof(pointGenerator.length2));
        domainGameObject = serializedObject.FindProperty(nameof(pointGenerator.domainGameObject));
    }

    private void OnDisable()
    {
        //Clean up references when inspector is disabled
        //This helps prevent null reference errors
        pointGenerator = null;
    }

    public override void OnInspectorGUI()
    {
        //Null checks to prevent errors
        if (target == null || pointGenerator == null || serializedObject == null || serializedObject.targetObject == null)
        {
            EditorGUILayout.HelpBox("Target object is null or destroyed.", MessageType.Warning);
            return;
        }

        serializedObject.Update();

        EditorGUILayout.PropertyField(generateOnChange, new GUIContent("Generate On Change"));

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(pointPrefab, new GUIContent("Point Prefab"));
        if(EditorGUI.EndChangeCheck() && pointGenerator != null) 
        {
            pointGenerator.OnPointPrefabChange((GameObject)pointPrefab.objectReferenceValue);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(pointsParent, new GUIContent("Points Parent"));
        if(EditorGUI.EndChangeCheck() && pointGenerator != null) 
        {
            pointGenerator.OnPointsParentChange((Transform)pointsParent.objectReferenceValue);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(pointCount, new GUIContent("Point Count"));
        if(EditorGUI.EndChangeCheck() && pointGenerator != null) 
        {
            pointGenerator.OnPointCountChange(pointCount.intValue);
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(pointScale, new GUIContent("Point Scale"));
        if(EditorGUI.EndChangeCheck() && pointGenerator != null) 
        {
            pointGenerator.OnPointScaleChange(pointScale.floatValue);
        }

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(pointDomainType, new GUIContent("Point Domain Type"));
        EditorGUILayout.PropertyField(length0, new GUIContent("Length 0"));
        EditorGUILayout.PropertyField(length1, new GUIContent("Length 1"));
        EditorGUILayout.PropertyField(length2, new GUIContent("Length 2"));
        EditorGUILayout.PropertyField(domainGameObject, new GUIContent("Domain Game Object"));

        serializedObject.ApplyModifiedProperties();
        if(EditorGUI.EndChangeCheck() && pointGenerator != null)
        {
            pointGenerator.OnGenerationSchemeChange();
        }

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate") && pointGenerator != null)
        {
            pointGenerator.Generate();
        }
        if(GUILayout.Button("Clear") && pointGenerator != null)
        {
            pointGenerator.Clear();
        }
        EditorGUILayout.EndHorizontal();
    }
}

}
#endif
