using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }

    //[SerializeField] protected PlayerController _playerControllerPrefab;
    //protected PlayerController _playerController;

    public T GetInstance<T>() where T : GameMode
    {
        return Instance as T;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        BeginPlay();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    /// <summary>
    /// This method is automatically called when the game mode is initialized, called once on Awake
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// This method is automatically called when the game mode is started, called once on Start
    /// </summary>
    public abstract void BeginPlay();

    /// <summary>
    /// This method need is called to start the game mode
    /// </summary>
    public abstract void StartGame();

    /// <summary>
    /// This method is called to pause the game
    /// </summary>
    public abstract void PauseGame();

    /// <summary>
    /// This method is called to resume the game
    /// </summary>
    public abstract void ResumeGame();

    /// <summary>
    /// This method is called to restart the game
    /// </summary>
    public abstract void RestartGame();

    /// <summary>
    /// This method is called to end the game
    /// </summary>
    public abstract void EndGame();

    /// <summary>
    /// This method is called when the game mode is destroyed
    /// </summary>
   public abstract void OnDestroyed();
}
