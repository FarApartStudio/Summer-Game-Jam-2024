using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class StoryModeManager : MonoBehaviour
{
    [SerializeField] private Pilot _player;
    [SerializeField] private GameMenu _gameMenu;

    private void Start()
    {
        _gameMenu = UIManager.GetMenu<GameMenu>();
        _player.GetCharacterCombatController.OnAimModeChanged += OnAimModeChanged;
        _player.GetCharacterCombatController.OnAimAccuracyChanged += OnAimAccuracyChanged;
        _gameMenu.Open();
    }

    private void OnAimAccuracyChanged(float value)
    {
        Debug.Log("Accuracy: " + value);
        _gameMenu.SetAimIndicatorSize(1 - value);
    }

    private void OnAimModeChanged(ViewMode mode)
    {
        _gameMenu.ToggleCrosshair(mode == ViewMode.Aim);
    }
}
