using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Pelumi.ObjectPool
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static Dictionary<string, PoolObjectInfo> objectPoolDictionary;
        public static GameObject _poolHolder;
        public static GameObject _particleSystemParent;
        public static GameObject _gameObjectSystemParent;
        public static GameObject _uiParent;

        private void Awake()
        {
            _poolHolder = new GameObject("Pool Holder");
            _poolHolder.transform.SetParent(transform);

            _particleSystemParent = new GameObject("Particle System Parent");
            _particleSystemParent.transform.SetParent(_poolHolder.transform);

            _gameObjectSystemParent = new GameObject("Game Object Parent");
            _gameObjectSystemParent.transform.SetParent(_poolHolder.transform);

            _uiParent = new GameObject("UI Parent");
            _uiParent.transform.SetParent(_poolHolder.transform);

            objectPoolDictionary = new Dictionary<string, PoolObjectInfo>();
        }

        public static T SpawnObject<T>(T original, PoolType poolType = PoolType.Any) where T : Component
        {
            if (SpawnObject(original.gameObject, Vector3.zero, Quaternion.identity, poolType).TryGetComponent(out T t))
            {
                return t;
            }
            else
            {
                Debug.LogWarning("Trying to spawn object that has not been pooled");
                return null;
            }
        }

        public static GameObject SpawnObject(GameObject original, PoolType poolType = PoolType.Any)
        {
            return SpawnObject(original, Vector3.zero, Quaternion.identity, poolType);
        }

        public static T SpawnObject<T>(T original, Vector3 position, PoolType poolType = PoolType.Any) where T : Component
        {
            if (SpawnObject(original.gameObject, position, Quaternion.identity, poolType).TryGetComponent(out T t))
            {
                return t;
            }
            else
            {
                Debug.LogWarning("Trying to spawn object that has not been pooled");
                return null;
            }
        }

        public static GameObject SpawnObject(GameObject original, Vector3 position, PoolType poolType = PoolType.Any)
        {
            return SpawnObject(original, position, Quaternion.identity, poolType);
        }

        public static T SpawnObject<T>(T original, Vector3 position, Quaternion rotation, Transform parent, PoolType poolType = PoolType.Any) where T : Component
        {
            if (SpawnObject(original.gameObject, position, rotation, parent, poolType).TryGetComponent(out T t))
            {
                return t;
            }
            else
            {
                Debug.LogWarning("Trying to spawn object that has not been pooled");
                return null;
            }
        }


        public static GameObject SpawnObject(GameObject original, Vector3 position, Quaternion rotation, Transform parent, PoolType poolType = PoolType.Any)
        {
            GameObject gameObject = SpawnObject(original, position, rotation, poolType);
            if (gameObject != null)
            {
                gameObject.transform.SetParent(parent, false);
            }
            return gameObject;
        }

        public static T SpawnObject<T>(T original, Transform parent, PoolType poolType = PoolType.Any) where T : Component
        {
            if (SpawnObject(original.gameObject, parent.transform.position, parent.transform.rotation, parent, poolType).TryGetComponent(out T t))
            {
                return t;
            }
            else
            {
                Debug.LogWarning("Trying to spawn object that has not been pooled");
                return null;
            }
        }

        public static GameObject SpawnObject(GameObject original, Transform parent, PoolType poolType = PoolType.Any)
        {
            GameObject gameObject = SpawnObject(original, parent.transform.position, parent.transform.rotation, poolType);
            if (gameObject != null)
            {
                gameObject.transform.SetParent(parent, false);
                gameObject.transform.localPosition = Vector3.zero;
            }
            return gameObject;
        }

        public static T SpawnObject<T>(T original, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.Any) where T : Component
        {
            if (SpawnObject(original.gameObject, position, rotation, poolType).TryGetComponent(out T t))
            {
                return t;
            }
            else
            {
                Debug.LogWarning("Trying to spawn object that has not been pooled");
                return null;
            }
        }

        public static GameObject SpawnObject(GameObject objectPrefab, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.Any)
        {
            PoolObjectInfo poolObjectInfo = null;

            if (objectPoolDictionary.ContainsKey(objectPrefab.name))
            {
                poolObjectInfo = objectPoolDictionary[objectPrefab.name];
            }
            else
            {
                poolObjectInfo = new PoolObjectInfo(objectPrefab, poolType);
                objectPoolDictionary.Add(objectPrefab.name, poolObjectInfo);
            }


            GameObject gameObject = poolObjectInfo.pool.Get();
            gameObject.transform.SetPositionAndRotation(position, rotation);
            return gameObject;
        }

        public static void ReleaseObject<T>(T t) where T : Component
        {
            ReleaseObject(t.gameObject);
        }

        public static void ReleaseObject(GameObject objectToReturn)
        {
            PoolObjectInfo poolObjectInfo = null;

            if (objectPoolDictionary.ContainsKey(objectToReturn.GetRawName()))
            {
                poolObjectInfo = objectPoolDictionary[objectToReturn.GetRawName()];
            }
            else
            {
                Debug.LogWarning("Failed to release object that has not been pooled but created a new pool for it" + objectToReturn.name);
                poolObjectInfo = new PoolObjectInfo(objectToReturn);
                objectPoolDictionary.Add(objectToReturn.GetRawName(), poolObjectInfo);
            }

            objectToReturn.SetActive(false);

            objectToReturn.transform.SetParent(GetRootTransfrom(poolObjectInfo.poolType), false);

            poolObjectInfo.pool.Release(objectToReturn);
        }

        public static Transform GetRootTransfrom(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.Any:
                    return _poolHolder.transform;
                case PoolType.Particle:
                    return _particleSystemParent.transform;
                case PoolType.GameObject:
                    return _gameObjectSystemParent.transform;
                case PoolType.UI:
                    return _uiParent.transform;
            }
            return _poolHolder.transform;
        }
    }

    public class PoolObjectInfo
    {
        public string key;
        public ObjectPool<GameObject> pool;
        public PoolType poolType;

        public PoolObjectInfo(GameObject _gameObject, PoolType _poolType = PoolType.Any)
        {
            key = _gameObject.name;
            poolType = _poolType;
            pool = new ObjectPool<GameObject>(
            () =>
            {
                GameObject gameObject = GameObject.Instantiate(_gameObject, ObjectPoolManager.GetRootTransfrom(_poolType), false);
                return gameObject;
            }
            ,
            (gameObject) =>
            {
                gameObject.SetActive(true);

                if (gameObject.TryGetComponent(out IHasPool hasPool))
                {
                    hasPool.OnGet();
                }
            },
            (gameObject) =>
            {
                if (gameObject.TryGetComponent(out IHasPool hasPool))
                {
                    hasPool.OnRelease();
                }
            },
            (gameObject) => GameObject.Destroy(gameObject), true);
        }
    }

    public static class GameObjectExtension
    {
        public static string GetRawName(this GameObject poolObject)
        {
            if (poolObject.name.Contains("(Clone)"))
            {
                return poolObject.name.Substring(0, poolObject.name.Length - 7);
            }
            return poolObject.name;
        }
    }

    public interface IHasPool
    {
        public void OnGet();
        public void OnRelease();
    }

    public enum PoolType
    {
        Any,
        Particle,
        GameObject,
        UI
    }
}