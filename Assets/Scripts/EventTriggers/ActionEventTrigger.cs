using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class ActionEvent : ISearchFilterable
{
    [FoldoutGroup("Event")] public string id;
    [FoldoutGroup("Event")] public UnityEvent action;

    [FoldoutGroup("Event")]
    [Button]
    public void CopyID()
    {
    #if UNITY_EDITOR
        EditorGUIUtility.systemCopyBuffer = id;
    #endif
    }

    public bool IsMatch(string searchString)
    {
        return id.ToLower().Contains(searchString.ToLower());
    }
}

public class ActionEventTrigger : MonoBehaviour
{
    [SerializeField, Searchable(FilterOptions = SearchFilterOptions.ISearchFilterableInterface)]
    private ActionEvent[] _actions;

    public void TriggerAction(string id)
    {
        for (int i = 0; i < _actions.Length; i++)
        {
            if (_actions[i].id == id)
            {
                _actions[i].action.Invoke();
                return;
            }
        }
    }
}
