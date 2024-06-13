using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AssetBank<>), true)]
public class AssetBankEditor : Editor
{
    SerializedProperty listProperty;
    GUILayoutOption[] verticalBoxOption;

    private void OnEnable()
    {
        listProperty = serializedObject.FindProperty("_valueList");
        verticalBoxOption = new GUILayoutOption[] { GUILayout.MaxHeight(50) };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DisplayAudioList();
        DisplayButtons();
        serializedObject.ApplyModifiedProperties();
        CheckForDuplicates();
        serializedObject.ApplyModifiedProperties();
    }

    public void DisplayAudioList()
    {
        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);

            GUI.backgroundColor = (i % 2 == 0) ? Color.blue : Color.white;

            if (element.FindPropertyRelative("Asset").objectReferenceValue == null) GUI.backgroundColor = Color.red;

            EditorGUILayout.BeginVertical("box", verticalBoxOption);

            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                listProperty.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                return;
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.PropertyField(element.FindPropertyRelative("Key"), new GUIContent("Key", "Put ID Here"));

            element.FindPropertyRelative("ShowFoldOut").boolValue =  EditorGUILayout.Foldout(element.FindPropertyRelative("ShowFoldOut").boolValue, "Properties");

            if (element.FindPropertyRelative("ShowFoldOut").boolValue)
            {
                EditorGUILayout.PropertyField(element.FindPropertyRelative("Info"), new GUIContent("Info", "Put Info Here"));

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("Group", "Enable if you want multiple assets here"));
                element.FindPropertyRelative("IsGroup").boolValue = EditorGUILayout.Toggle(GUIContent.none, element.FindPropertyRelative("IsGroup").boolValue);
                EditorGUILayout.EndHorizontal();

                if (element.FindPropertyRelative("IsGroup").boolValue)
                {
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("GroupAsset"), new GUIContent("Group Assets", "Put Group Assets Here"));
                }
                else
                {
                    EditorGUILayout.PropertyField(element.FindPropertyRelative("Asset"), new GUIContent("Asset", "Put Asset Here"));
                }
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        }
    }

    public void DisplayButtons()
    {
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add Asset Slot"))
        {
            listProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Remove last Asset Slot"))
        {
            listProperty.arraySize--;
            serializedObject.ApplyModifiedProperties();
        }
    }

    public void CheckForDuplicates()
    {
        for (int i = 0; i < listProperty.arraySize; i++)
        {
            SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
            string id = element.FindPropertyRelative("Key").stringValue;
            for (int j = 0; j < listProperty.arraySize; j++)
            {
                if (i == j) continue;
                SerializedProperty element2 = listProperty.GetArrayElementAtIndex(j);
                string id2 = element2.FindPropertyRelative("Key").stringValue;
                if (id == id2)
                {
                    EditorGUILayout.HelpBox("Asset with same ID already exists", MessageType.Error);
                    return;
                }
            }
        }
    }
}