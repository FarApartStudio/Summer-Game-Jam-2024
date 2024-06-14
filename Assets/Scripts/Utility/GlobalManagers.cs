using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManagers : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> persitentManagers;
    [SerializeField] protected List<MonoBehaviour> unPersitentManagers;

    public static GlobalManagers Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < persitentManagers.Count; i++)
        {
            Instantiate(persitentManagers[i], transform);
        }

        SceneManager.activeSceneChanged += SceneManager_ActiveSceneChanged;
    }

    private void SceneManager_ActiveSceneChanged(Scene arg0, Scene arg1)
    {
        for (int i = 0; i < unPersitentManagers.Count; i++)
        {
            Instantiate(unPersitentManagers[i], Vector3.zero, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneManager_ActiveSceneChanged;
    }
}
