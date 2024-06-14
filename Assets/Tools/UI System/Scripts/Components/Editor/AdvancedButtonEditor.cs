using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(AdvanceButton))]
[CanEditMultipleObjects]
public class AdvancedButtonEditor : ButtonEditor
{
    private SerializedProperty buttonType;
    private SerializedProperty onHold;
    private SerializedProperty onRelease;
    private SerializedProperty onClickToggle;
    private SerializedProperty toggleGroup;
    private SerializedProperty toggleIndicator;


    protected override void OnEnable()
    {
        base.OnEnable();
        buttonType = serializedObject.FindProperty("buttonType");
        onHold = serializedObject.FindProperty("onHold");
        onRelease = serializedObject.FindProperty("onRelease");
        onClickToggle = serializedObject.FindProperty("onClickToggle");
        toggleGroup = serializedObject.FindProperty("toggleGroup");
        toggleIndicator = serializedObject.FindProperty("toggleIndicator");
    }

    public override void OnInspectorGUI()
    {
        AdvanceButton targetMyButton = (AdvanceButton)target;

        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(buttonType);

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("Button Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(onHold);

        EditorGUILayout.Space(20);
        EditorGUILayout.PropertyField(onRelease);

        EditorGUILayout.Space(20);
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.PropertyField(onClickToggle);
        EditorGUILayout.PropertyField(toggleGroup);
        EditorGUILayout.PropertyField(toggleIndicator);
        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}
