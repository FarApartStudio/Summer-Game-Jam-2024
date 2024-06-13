using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class InventoryData<T>
{
    public abstract int Count { get; }
    public abstract void Add(T itemBaseData);
    public abstract void Remove(T itemBaseData);
    public abstract bool Contains(T itemBaseData);
    public abstract void Clear();
}

public abstract class InventorySO<IInventoryData, DataType> : ScriptableObject where IInventoryData : InventoryData<DataType>
{
    public int max;
    public string inventoryID;
    public IInventoryData inventoryData;
    public event Action<DataType> OnValueAdded;
    public event Action<DataType> OnValueRemoved;
    public event Action OnMaxReachError;

    public virtual bool AddToInventory(DataType data, bool notify = true)
    {
        if (inventoryData.Count >= max)
        {
            OnMaxReachError?.Invoke();
            return false;
        }
        inventoryData.Add(data);
        if (notify)  OnValueAdded?.Invoke(data);
        Save();
        return true;
    }

    public virtual void RemoveFromInventory(DataType data, bool notify = true)
    {
        inventoryData.Remove(data);
        if (notify) OnValueRemoved?.Invoke(data);
        Save();
    }

    public bool ExitsInInventory(DataType data)
    {
        return inventoryData.Contains(data);
    }

    [Button("Save")]
    public virtual void Save()
    {
        if (string.IsNullOrEmpty(inventoryID))
        {
            Debug.LogError("Inventory ID is null or empty", this);
            return;
        }
        FileManager.Save(inventoryID, inventoryData);
    }

    [Button("Load")]
    public virtual void Load()
    {
        if (string.IsNullOrEmpty(inventoryID))
        {
            Debug.LogError("Inventory ID is null or empty", this);
            return;
        }
        inventoryData.Clear();
        IInventoryData savedInventoryData = FileManager.Load<IInventoryData>(inventoryID);
        if (savedInventoryData != null)
        {
            inventoryData = savedInventoryData;
        }
    }

    [Button("Clear")]
    public virtual void Clear()
    {
        if (inventoryData != null) inventoryData.Clear();
        Save();
    }

    [Button("Delete")]
    public void Delete()
    {
        FileManager.Delete(inventoryID);
    }
}