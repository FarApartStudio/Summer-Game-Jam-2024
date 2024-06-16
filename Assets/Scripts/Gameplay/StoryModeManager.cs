using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryModeManager : MonoBehaviour
{
    [SerializeField] private Pilot _playerPrefab;
    [SerializeField] private Transform spawnPos;

    [Header("Effect")]
    [SerializeField] FollowTransfrom _rainStormPrefab;
    [SerializeField] FollowTransfrom _windlinesPrefab;

    [Header("Enemy")]
    [SerializeField] private List<EnemyActivator> _enemyActivators;

    private Pilot _player;
    private FollowTransfrom  _rainStorm;
    private FollowTransfrom _windlines;


    private GameMenu _gameMenu;

    private void Awake()
    {

    }

    private void Start()
    {
        SpawnCharacter();
        _gameMenu = UIManager.GetMenu<GameMenu>();
        _gameMenu.Open();
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
            activator.OnActivated += OnEnemyActivated;
            activator.Init();
        }
    }

    private void OnEnemyActivated(EnemyController controller)
    {
        controller.SetTarget(_player.transform);
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
}
