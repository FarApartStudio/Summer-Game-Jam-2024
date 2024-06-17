using Pelumi.UISystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryModeManager : MonoBehaviour
{
    public static StoryModeManager Instance { get; private set; }


    [SerializeField] private Pilot _playerPrefab;
    [SerializeField] private Transform spawnPos;

    [Header("Effect")]
    [SerializeField] FollowTransfrom _rainStormPrefab;
    [SerializeField] FollowTransfrom _windlinesPrefab;

    [Header("Enemy")]
    [SerializeField] private EnemyActivator[] _enemyActivators;
    [SerializeField] private EnemySpawnTrigger[] _enemySpawnTriggers;

    private Pilot _player;
    private FollowTransfrom  _rainStorm;
    private FollowTransfrom _windlines;
    private GameMenu _gameMenu;
    private HealthBarMenu _healthBarMenu;

    public Pilot GetPlayer => _player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitUI();

        SpawnCharacter();

        _gameMenu.Open();
        _healthBarMenu.Open();
    }

    public void InitUI()
    {
        _gameMenu = UIManager.GetMenu<GameMenu>();
        _healthBarMenu = UIManager.GetMenu<HealthBarMenu>();
    }

    public void SpawnCharacter ()
    {
        _player = Instantiate(_playerPrefab, spawnPos.position, spawnPos.rotation);

        _rainStorm = Instantiate(_rainStormPrefab, _player.transform.position,Quaternion.identity);
        _rainStorm.SetTarget(_player.transform);
        _windlines = Instantiate(_windlinesPrefab, _player.transform.position, Quaternion.identity);
        _windlines.SetTarget(_player.transform);

        _player.GetCharacterCombatController.OnAimModeChanged += OnAimModeChanged;
        _player.GetCharacterCombatController.OnAimAccuracyChanged += OnAimAccuracyChanged;
        _player.GetCharacterCombatController.OnSuccessfulHit += OnSuccessfulHit;

        ActivateEnemies();
    }

    public void ActivateEnemies()
    {
        foreach (var activator in _enemyActivators)
        {
            activator.OnActivated = OnEnemyActivated;
            activator.OnKilled = OnEnemyKilled;
            activator.Init();
        }

        foreach (var trigger in _enemySpawnTriggers)
        {
            trigger.OnActivated = OnEnemyActivated;
            trigger.OnKilled = OnEnemyKilled;
            trigger.Init();
        }
    }

    private void OnEnemyActivated(EnemyController controller)
    {
        controller.SetTarget(_player.transform);
        _healthBarMenu.AddHealthBar(controller.healthController);

    }

    private void OnEnemyKilled(EnemyController controller)
    {
        _healthBarMenu.RemoveHealthBar(controller.healthController);
    }


    private void OnSuccessfulHit()
    {
        _gameMenu.PlayHitMarker();
    }

    private void OnAimAccuracyChanged(float value)
    {
        _gameMenu.SetAimIndicatorSize(1 - value);
    }

    private void OnAimModeChanged(ViewMode mode)
    {
        _gameMenu.ToggleCrosshair(mode == ViewMode.Aim);
    }

    [Button]
    private void GenerateAllActivatorsAndTriggers()
    {
        _enemyActivators = FindObjectsOfType<EnemyActivator>();
        _enemySpawnTriggers  = FindObjectsOfType<EnemySpawnTrigger>();
    }
}
