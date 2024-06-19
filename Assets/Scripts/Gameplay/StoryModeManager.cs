using Pelumi.Juicer;
using Pelumi.ObjectPool;
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
    [SerializeField] private Transform storySpawnpoint;
    [SerializeField] private Transform swampSpawnpoint;

    [Header("First Cinematic")]
    [SerializeField] private GameObject cinematicActors;
    [SerializeField] private TimelineController _introTimeline;

    [Header("Second Cinematic")]
    [SerializeField] private InteractionHandler _cinematicTwoTrigger;
    [SerializeField] private GameObject cinematicActorsTwo;
    [SerializeField] private TimelineController _introTimelineTwo;

    [Header("Effect")]
    [SerializeField] FollowTransfrom _rainStormPrefab;
    [SerializeField] FollowTransfrom _windlinesPrefab;
    [SerializeField] MultiHitObserver _cloudHitObserver;

    [Header("Enemy")]
    [SerializeField] private EnemyActivator[] _enemyActivators;
    [SerializeField] private EnemySpawnTrigger[] _enemySpawnTriggers;

    [Header("Debug")]
    [SerializeField] private bool test;

    private Pilot _player;
    private FollowTransfrom _windlines;
    private GameMenu _gameMenu;
    private HealthBarMenu _healthBarMenu;
    private ScreenFadeMenu _screenFadeMenu;
    private CinematicMenu _cinematicMenu;

    public Pilot GetPlayer => _player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitUI();

        if (test)
        {
            SpawnCharacter();
            ActivateEnemies();
            _gameMenu.Open();
            _healthBarMenu.Open();
            return;
        }

        IntroCutScene();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
     
        //}
    }

    public void IntroCutScene()
    {
        _introTimeline.AddTimeEvent(0.8f, () =>
        {
            _screenFadeMenu.Open().Show(1, .5f, OnFadeMid: () =>
            {
                _cinematicMenu.Close();

                cinematicActors.SetActive(false);

                SpawnCharacter();

                ActivateEnemies();

                EnvironmeentEvents();

                _gameMenu.Open();
                _healthBarMenu.Open();
            });

        });

        _introTimeline.OnFinished += (timeline) =>
        {
           
        };

        _introTimeline.Play();

        _cinematicMenu.Open();
    }

    public void SecondCutScene()
    {
        _introTimelineTwo.AddTimeEvent(0.8f, () =>
        {
            _screenFadeMenu.Open().Show(1, .5f, OnFadeMid: () =>
            {
                cinematicActorsTwo.gameObject.SetActive(false);
                _cinematicMenu.Close();
                _gameMenu.Open();
                TeleportPlayer(swampSpawnpoint);
                _player.ToggleMovement(true);
            });

        });

        _introTimelineTwo.OnFinished += (timeline) =>
        {

        };

        _gameMenu.Close();

        cinematicActorsTwo.gameObject.SetActive(true);

        _introTimelineTwo.Play();

        _cinematicMenu.Open();
    }

    public void InitUI()
    {
        _gameMenu = UIManager.GetMenu<GameMenu>();
        _healthBarMenu = UIManager.GetMenu<HealthBarMenu>();
        _screenFadeMenu = UIManager.GetMenu<ScreenFadeMenu>();
        _cinematicMenu = UIManager.GetMenu<CinematicMenu>();

        TaskPrompt.OnMissionPrompt  = (text) => _gameMenu.SetMissionPrompt(text);
        TutorialKeyPrompt.OnTutorialPrompt = (data) => _gameMenu.SetTutorialPrompt(data);

        _cinematicTwoTrigger.OnInteractStart += OnInteractStart;
    }

    private void OnInteractStart(Collider obj)
    {
        _player.ToggleMovement(false);
        _cinematicTwoTrigger.gameObject.SetActive(false);
        SecondCutScene();
    }

    public void SpawnCharacter ()
    {
        _player = Instantiate(_playerPrefab, storySpawnpoint.position, storySpawnpoint.rotation);

        _windlines = Instantiate(_windlinesPrefab, _player.transform.position, Quaternion.identity);
        _windlines.SetTarget(_player.transform);

        _player.GetCharacterCombatController.OnAimModeChanged += OnAimModeChanged;
        _player.GetCharacterCombatController.OnAimAccuracyChanged += OnAimAccuracyChanged;
        _player.GetCharacterCombatController.OnSuccessfulHit += OnSuccessfulHit;

        _player.GetHealthController.OnHealthChanged += PlayerChanged;
        _player.GetHealthController.OnHit += PlayerHit;
    }

    private void PlayerChanged(IHealth obj)
    {
        _gameMenu.SetPlayerHealthBar(obj.GetNormalisedHealth);
    }

    public void EnvironmeentEvents()
    {
        _cloudHitObserver.GetOnComplete.AddListener(OnCloudHit);
    }

    private void PlayerHit(DamageInfo damageInfo)
    {
        _gameMenu.SpawnDamageIndicator(_player.transform, damageInfo.damagePosition);
    }

    private void OnCloudHit()
    {
        _cloudHitObserver.ResetHit();
        FollowTransfrom  _rainStorm = ObjectPoolManager.SpawnObject(_rainStormPrefab, _player.transform.position, Quaternion.identity);
        _rainStorm.SetTarget(_player.transform);
        Juicer.WaitForSeconds(20, new JuicerCallBack(() => ObjectPoolManager.ReleaseObject(_rainStorm)));
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

    private void TeleportPlayer (Transform spawnpoint)
    {
        _player.gameObject.SetActive(false);
        _player.transform.position = spawnpoint.position;
        _player.transform.rotation = spawnpoint.rotation;
        _player.gameObject.SetActive(true);
    }

    [Button]
    private void GenerateAllActivatorsAndTriggers()
    {
        _enemyActivators = FindObjectsOfType<EnemyActivator>();
        _enemySpawnTriggers  = FindObjectsOfType<EnemySpawnTrigger>();
    }
}
