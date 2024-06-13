using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetBank<T> : ScriptableObject
{
    [SerializeField] protected List<AssetWithID<T>> _valueList = new List<AssetWithID<T>>();

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
}

[System.Serializable]
public class AssetWithID<T>
{
    public string Key;
    public T Asset;

    [TextArea(1, 3)]
    public string Info;

    public bool IsGroup;
    public AssetInGroup<T>[] GroupAsset;

    [HideInInspector] public bool ShowFoldOut;
}

[System.Serializable]
public class AssetInGroup<T>
{
    [Range(0, 100)]
    public float Chance;
    public T Asset;
}