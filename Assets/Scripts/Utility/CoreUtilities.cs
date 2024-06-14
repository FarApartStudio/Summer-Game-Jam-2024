using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameProject.Utilities
{
    public static class VectorUtilities
    {
        public static Vector3 Only(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }
        
        public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null) 
        {
            return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
        }
    }
    
    public static class CoreUtilities
    {
        public static bool ContainsLayer(this LayerMask layerMask, int layer)
        {
            return (layerMask.value & (1 << layer)) != 0;
        }
        
        public static T GetRandom<T>(this IEnumerable<T> collection)
        {
            if (collection == null)
                throw new Exception();
    
            T[] enumerable = collection as T[] ?? collection.ToArray();
                
            if (!enumerable.Any())
                throw new Exception();
    
            int randomIndex = Random.Range(0, enumerable.Count());
            return enumerable.ElementAt(randomIndex);
        }
        
        public static bool IsEmpty<T>(this List<T> collection)
        {
            return collection.Count == 0;
        }
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            bool hasComponent = gameObject.TryGetComponent(out T objectComponent);
            
            if (!hasComponent)
                objectComponent = gameObject.AddComponent<T>();

            return objectComponent;
        }
        
        public static T GetOrAddComponent<T>(this Transform transform) where T : Component
        {
            bool hasComponent = transform.TryGetComponent(out T objectComponent);

            if (!hasComponent)
                objectComponent = transform.gameObject.AddComponent<T>();

            return objectComponent;
        }
    }

    public static class LogUtilities
    {
        public static void LogMissingComponent<T>(GameObject gameObject = null)
        {
            if (gameObject == null)
                Debug.Log($"Missing Component of {typeof(T)}");
            else
                Debug.Log($"Missing Component of {typeof(T)}", gameObject);
        }
    }
}

