using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using Random = UnityEngine.Random;

public class AssetBank<T> : ScriptableObject
{
    [SerializeField, Searchable (FilterOptions = SearchFilterOptions.ISearchFilterableInterface)] 
    protected List<AssetWithID<T>> _valueList = new List<AssetWithID<T>>();

    public T GetAsset(string key)
    {
        AssetWithID<T> valueWithID = _valueList.FirstOrDefault(x => x.Key == key);
        if (valueWithID == null)
        {
            Debug.LogError("Asset with ID: " + key + " not found in " + name);
            return default;
        }

        if (valueWithID.IsGroup && valueWithID.GroupAsset.Length > 0)
        {
            float totalChance = 0;
            foreach (AssetInGroup<T> assetInGroup in valueWithID.GroupAsset)
            {
                totalChance += assetInGroup.Chance;
            }

            float randomValue = Random.Range(0, totalChance);
            float currentChance = 0;
            foreach (AssetInGroup<T> assetInGroup in valueWithID.GroupAsset)
            {
                currentChance += assetInGroup.Chance;
                if (randomValue <= currentChance)
                {
                    return assetInGroup.Asset;
                }
            }

            return valueWithID.GroupAsset[0].Asset;
        }
        else
        {
            return valueWithID.Asset;
        }
    }

    public List<T> GetAssets(params string[] keys)
    {
        List<T> assets = new List<T>();
        foreach (string key in keys)
        {
            assets.Add(GetAsset(key));
        }
        return assets;
    }

    public T GetRandomAsset()
    {
        return _valueList[Random.Range(0, _valueList.Count)].Asset;
    }

    public List<string> GetAllID()
    {
        return _valueList.Select(x => x.Key).ToList();
    }
}

[Serializable]
public class AssetWithID<T> : ISearchFilterable
{
    [FoldoutGroup("Asset")] public string Key;
    [HideIf("IsGroup")] [FoldoutGroup("Asset")] public T Asset;
    [FoldoutGroup("Asset")] [TextArea(1, 3)] public string Info;

    [FoldoutGroup("Asset")] public bool IsGroup;
    [FoldoutGroup("Asset")] [ShowIf ("IsGroup")] public AssetInGroup<T>[] GroupAsset;

    [FoldoutGroup("Asset")]
    [Button]
    public void CopyKey()
    {
#if UNITY_EDITOR
        EditorGUIUtility.systemCopyBuffer = Key;
#endif
    }

    public bool IsMatch(string searchString)
    {
        return Key.ToLower().Contains(searchString.ToLower());
    }
}

[System.Serializable]
public class AssetInGroup<T>
{
    [Range(0, 100)]
    public float Chance = 50;
    public T Asset;
}