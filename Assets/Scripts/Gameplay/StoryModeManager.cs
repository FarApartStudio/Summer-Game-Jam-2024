using Pelumi.AudioSystem;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using Pelumi.UISystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StoryModeManager : MonoBehaviour
{
    public static StoryModeManager Instance { get; private set; }

    [SerializeField] private int maxRetry = 3;
    [SerializeField] private int _currentArea = 1;
    [SerializeField] private int rainStormDuration = 15;


    [Header("Settings")]
    [SerializeField] private Pilot _playerPrefab;
    [SerializeField] private Transform storySpawnpoint;
    [SerializeField] private Transform swampSpawnpoint;
    [SerializeField] private Transform bossSpawnpoint;

    [Header("First Cinematic")]
    [SerializeField] private GameObject _tutorialArea;
    [SerializeField] private GameObject cinematicActors;
    [SerializeField] private TimelineController _introTimeline;

    [Header("Second Cinematic")]
    [SerializeField] private GameObject swampArea;
    [SerializeField] private InteractionHandler _cinematicTwoTrigger;
    [SerializeField] private GameObject cinematicActorsTwo;
    [SerializeField] private TimelineController _introTimelineTwo;

    [Header("Third Cinematic")]
    [SerializeField] private GameObject bossArea;
    [SerializeField] private InteractionHandler _cinematicThreeTrigger;
    [SerializeField] private GameObject cinematicActorsThree;
    [SerializeField] private TimelineController _introTimelineThree;

    [Header("End")]
    [SerializeField] private VideoClip endVideo;
    [SerializeField] private Boss _boss;

    [Header("Effect")]
    [SerializeField] FollowTransfrom _rainStormPrefab;
    [SerializeField] FollowTransfrom _windlinesPrefab;
    [SerializeField] MultiHitObserver _cloudHitObserver;

    [Header("Enemy")]
    [SerializeField] private EnemyActivator[] _enemyActivators;
    [SerializeField] private EnemySpawnTrigger[] _enemySpawnTriggers;

    [Header("Debug")]
    [SerializeField] private bool test;
    [SerializeField] private int testArea;

    private Pilot _player;
    private FollowTransfrom _windlines;
    private GameMenu _gameMenu;
    private HealthBarMenu _healthBarMenu;
    private ScreenFadeMenu _screenFadeMenu;
    private CinematicMenu _cinematicMenu;
    private ArrowSelectionWheelMenu _arrowSelectionWheelMenu;
    private bool isPaused;
    private int _currentDeathCount;

    public Pilot GetPlayer => _player;

    private void Awake()
    {
        Instance = this;
        _currentArea = PlayerPrefs.GetInt("CurrentArea", 1);
        EnableNeedAreas();
    }

    private void Start()
    {
        InitUI();

        InputManager.Instance.ToggleCursor(false);

        AudioSystem.PlayAudio(AudioTypeID.MainTrack, AudioCategory.Music, volume: .15f);

        if (test)
        {
            SpawnCharacter(storySpawnpoint);
            ActivateEnemies();
            _gameMenu.Open();
            _healthBarMenu.Open();
            return;
        }



        LoadArea(_currentArea);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _arrowSelectionWheelMenu.Open();
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _arrowSelectionWheelMenu.Close();
        }
    }

    private void LoadArea(int area)
    {
        switch (area)
        {
            case 1:
                _cinematicTwoTrigger.OnInteractStart += OnPlayerInteractCinematicTwo;
                _cinematicThreeTrigger.OnInteractStart += OnPlayerInteractCinematicThree;
                IntroCutScene();
                break;
            case 2:
                _cinematicThreeTrigger.OnInteractStart += OnPlayerInteractCinematicThree;
                _screenFadeMenu.Open().ShowWithFade(1, 1.5f, () =>
                {
                    SpawnCharacter(swampSpawnpoint);

                    ActivateEnemies();

                    EnvironmeentEvents();

                    _gameMenu.Open();

                    _healthBarMenu.Open();
                });

                break;
            case 3:
                _screenFadeMenu.Open().ShowWithFade(1, 1.5f, () =>
                {
                    SpawnCharacter(bossSpawnpoint);

                    ActivateEnemies();

                    EnvironmeentEvents();

                    _gameMenu.Open();

                    _healthBarMenu.Open();

                    StartBossFight();
                });
                break;
        }
    }

    public void IntroCutScene()
    {
        _introTimeline.AddTimeEvent(0.8f, () =>
        {
            _screenFadeMenu.Open().Show(1, .5f, OnFadeMid: () =>
            {
                _cinematicMenu.Close();

                cinematicActors.SetActive(false);

                SpawnCharacter(storySpawnpoint);

                ActivateEnemies();

                EnvironmeentEvents();

                _gameMenu.Open();
                _healthBarMenu.Open();
            });

        });

        _introTimeline.OnFinished += () =>
        {
           
        };

        cinematicActors.gameObject.SetActive(true);

        _introTimeline.Play();

        _cinematicMenu.Open();
    }

    public void SecondCutScene()
    {
        _currentArea++;
        PlayerPrefs.SetInt("CurrentArea", _currentArea);

        _introTimelineTwo.AddTimeEvent(0.8f, () =>
        {
            _screenFadeMenu.Open().Show(1, .5f, OnFadeMid: () =>
            {
                cinematicActorsTwo.gameObject.SetActive(false);
                _cinematicMenu.Close();
                _gameMenu.Open();
                TeleportPlayer(swampSpawnpoint);
                _player.ToggleMovement(true);
                DisableArea(1);
            });

        });

        _introTimelineTwo.OnFinished += () =>
        {

        };

        _gameMenu.Close();

        EnableArea(2);

        cinematicActorsTwo.gameObject.SetActive(true);

        _introTimelineTwo.Play();

        _cinematicMenu.Open();
    }

    public void ThirdCutScene()
    {
        _currentArea++;
        PlayerPrefs.SetInt("CurrentArea", _currentArea);

        _introTimelineThree.AddTimeEvent(0.8f, () =>
        {
            _screenFadeMenu.Open().Show(1, .5f, OnFadeMid: () =>
            {
                cinematicActorsThree.gameObject.SetActive(false);
                _cinematicMenu.Close();
                _gameMenu.Open();
                TeleportPlayer(bossSpawnpoint);
                _player.ToggleMovement(true);
                DisableArea(2);
                StartBossFight();
            });

        });

        _introTimelineThree.OnFinished += () =>
        {
            Debug.Log("Finished");

        };

        EnableArea(3);

        _gameMenu.Close();

        cinematicActorsThree.gameObject.SetActive(true);

        _introTimelineThree.Play();

        _cinematicMenu.Open();
    }   

    public void InitUI()
    {
        _gameMenu = UIManager.GetMenu<GameMenu>();
        _healthBarMenu = UIManager.GetMenu<HealthBarMenu>();
        _screenFadeMenu = UIManager.GetMenu<ScreenFadeMenu>();
        _cinematicMenu = UIManager.GetMenu<CinematicMenu>();
        _arrowSelectionWheelMenu = UIManager.GetMenu<ArrowSelectionWheelMenu>();

        _gameMenu.OnSettingsPressed += () =>
        {

        };

        _gameMenu.OnResumePressed += () =>
        {
            TogglePause (false);
        };

        _gameMenu.OnQuitPressed += () =>
        {
            LoadingScreen.Instance.LoadScene(0);
        };

        _arrowSelectionWheelMenu.GetArrowSOs += () =>
        {
            return _player.GetCharacterCombatController.GetArrowSOs;
        };

        _arrowSelectionWheelMenu.OnArrowSelected += (arrowSO) =>
        {
            _player.GetCharacterCombatController.SetArrow(arrowSO);
        };

        InputManager.Instance.OnPauseBtnPressed = () =>
        {
            TogglePause (!isPaused);
        };

        TaskPrompt.OnMissionPrompt  = (text) => _gameMenu.SetMissionPrompt(text);
        TutorialKeyPrompt.OnTutorialPrompt = (data) => _gameMenu.SetTutorialPrompt(data);
    }

    public void TogglePause (bool state)
    {
        isPaused = state;
        _gameMenu.TogglePause(isPaused);
        InputManager.Instance.ToggleCursor(state);
        _player.ToggleMovement(!state);
    }

    private void OnPlayerInteractCinematicTwo(Collider obj)
    {
        _player.ToggleMovement(false);
        _cinematicTwoTrigger.gameObject.SetActive(false);
        SecondCutScene();
    }

    private void OnPlayerInteractCinematicThree(Collider collider)
    {
        _player.ToggleMovement(false);
        _cinematicThreeTrigger.gameObject.SetActive(false);
        ThirdCutScene();
    }

    public void SpawnCharacter (Transform spawnPoint)
    {
        _player = Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);

        _windlines = Instantiate(_windlinesPrefab, _player.transform.position, Quaternion.identity);
        _windlines.SetTarget(_player.transform);

        _player.GetCharacterCombatController.OnAimModeChanged += OnAimModeChanged;
        _player.GetCharacterCombatController.OnAimAccuracyChanged += OnAimAccuracyChanged;
        _player.GetCharacterCombatController.OnSuccessfulHit += OnSuccessfulHit;

        _player.GetHealthController.OnHealthChanged += PlayerChanged;
        _player.GetHealthController.OnHit += PlayerHit;
        _player.GetHealthController.OnDie += PlayerDead;
    }

    private void PlayerDead(DamageInfo info)
    {
        _currentDeathCount++;

        _gameMenu.ShowGameOver(maxRetry - _currentDeathCount);

        IEnumerator PlayerDeadRoutine()
        {
            yield return new WaitForSeconds(2);
            RespawnPlayer();
        }

        StartCoroutine(PlayerDeadRoutine());
    }

    private void RespawnPlayer ()
    {
        if (_currentDeathCount >= maxRetry)
        {
            RestartGame();
            return;
        }

        _gameMenu.Revive();

        _screenFadeMenu.Open().Show(1, 1f,null, () =>
        {
            CheckpointTrigger lastCheckPoint = CheckpointManager.Instance.GetActiveCheckpoint();
            if (lastCheckPoint == null)
            {
                TeleportPlayer(_currentArea == 1 ? storySpawnpoint : swampSpawnpoint);
            }
            else
            {
                TeleportPlayer(lastCheckPoint.GetSpawnPos);
            }

        }, () =>
        {
            Debug.Log("Fade Out");
            _player.Revive();
        });
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
        Juicer.WaitForSeconds(rainStormDuration, new JuicerCallBack(() => ObjectPoolManager.ReleaseObject(_rainStorm)));
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

    private void RestartGame()
    {
        LoadingScreen.Instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void StartBossFight ()
    {
        _gameMenu.ShowBossHealthBar(true);
        AudioSystem.PlayAudio(AudioTypeID.CombatMusic, AudioCategory.Music, volume: .15f);
        _boss.Activate(_player.transform);
        _boss.GetHealthController.OnHealthChanged += (health) => _gameMenu.SetBossHealthBar(health.GetNormalisedHealth);
        _boss.GetHealthController.OnDie += (info) => EndGame();
    }

    private void EndGame()
    {
        _player.GetHealthController.SetInvisibility(true);

        AudioSystem.StopAudioWithFade(AudioCategory.Music);

        IEnumerator EndGameRoutine()
        {
            yield return new WaitForSeconds(5);

            _gameMenu.Close();
            _healthBarMenu.Close();
            _screenFadeMenu.Close();

            UIManager.GetMenu<MovieMenu>().Play(endVideo, null, () =>
            {
                LoadingScreen.Instance.LoadScene(0);
            });
        }

        StartCoroutine(EndGameRoutine());
    }

    private void EnableNeedAreas ()
    {
        if (_currentArea == 1)
        {
            _tutorialArea.SetActive(true);
        }
        else if (_currentArea == 2)
        {
            swampArea.SetActive(true);
        }
        else if (_currentArea == 3)
        {
            bossArea.SetActive(true);
        }
    }

    private void EnableArea(int area)
    {
        if (area == 1)
        {
            _tutorialArea.SetActive(true);
        }
        else if (area == 2)
        {
            swampArea.SetActive(true);
        }
        else if (area == 3)
        {
            bossArea.SetActive(true);
        }
    }

    private void DisableArea (int area)
    {
        if (area == 1)
        {
            _tutorialArea.SetActive(false);
        }
        else if (area == 2)
        {
            swampArea.SetActive(false);
        }
        else if (area == 3)
        {
            bossArea.SetActive(false);
        }
    }

    [Button]
    private void GenerateAllActivatorsAndTriggers()
    {
        _enemyActivators = FindObjectsOfType<EnemyActivator>();
        _enemySpawnTriggers  = FindObjectsOfType<EnemySpawnTrigger>();
    }

    [Button]
    public void ClearData ()
    {
        PlayerPrefs.DeleteAll();
    }

    [Button]
    public void UseTestArea()
    {
        PlayerPrefs.SetInt("CurrentArea", testArea);
    }
}
