using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Pelumi.UISystem;
using System;
using GameProject;

public class ArrowSelectionWheelMenu : GenericMenu<ArrowSelectionWheelMenu>
{
    public event Func<List<ArrowSO>> GetArrowSOs;
    public event Action<ArrowSO> OnArrowSelected;

    [SerializeField] private RingUI _ringUIPrefab;
    [SerializeField] private Transform _ringUIParent;
    [FoldoutGroup("Debug")][ReadOnly] private List<RingUI> _ringUIs = new List<RingUI>();
    [FoldoutGroup("Debug")][ReadOnly][SerializeField] private int _selectedSlot = -1;

    private SelectionWheel selectionWheel;

    protected override void OnCreated()
    {
        selectionWheel = GetComponent<SelectionWheel>();
        selectionWheel.OnRingUISelected += (ringUI) =>
        {
            _selectedSlot = _ringUIs.IndexOf(ringUI);
            OnArrowSelected?.Invoke(GetArrowSOs.Invoke()[_selectedSlot]);
            Close();
        };
    }

    protected override void OnOpened()
    {
        List<ArrowSO> arrowSOs = GetArrowSOs?.Invoke();

        for (int i = 0; i < arrowSOs.Count; i++)
        {
            RingUI ringUI = Instantiate(_ringUIPrefab, _ringUIParent);
            ringUI.GetIcon.sprite = arrowSOs[i].GetIcon;
            _ringUIs.Add(ringUI);
        }

        selectionWheel.Init(_ringUIs);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    protected override void OnClosed()
    {
        foreach (RingUI ringUI in _ringUIs)
        {
            Destroy(ringUI.gameObject);
        }

        _ringUIs.Clear();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    protected override void OnDestoryInvoked()
    {

    }

    public override void ResetMenu()
    {

    }
}
