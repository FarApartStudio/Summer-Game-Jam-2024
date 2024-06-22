using GameProject;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionWheel : MonoBehaviour
{
    public event Action<RingUI> OnRingUISelected;

    [SerializeField] private float _gap = 10f;

    [FoldoutGroup("Debug")][ReadOnly] private int _numberOfSlots = 8;
    [FoldoutGroup("Debug")][ReadOnly] private List<RingUI> _ringUIs = new List<RingUI>();
    [FoldoutGroup("Debug")][ReadOnly] [SerializeField] private int _selectedSlot = -1;

    public void Init(List<RingUI> rings)
    {
        _ringUIs = rings;
        _numberOfSlots = rings.Count;
        float step = 360f / _numberOfSlots;
        float _iconDistance = Vector3.Distance(rings[0].GetIcon.transform.position, rings[0].GetRing.transform.position);

        for (int i = 0; i < rings.Count; i++)
        {
            RingUI ringUI = rings[i];

            ringUI.transform.localPosition = Vector3.zero;
            ringUI.transform.localRotation = Quaternion.identity;

            ringUI.GetRing.fillAmount = 1f / _numberOfSlots - _gap / 360f;
            ringUI.GetRing.transform.localPosition = Vector3.zero;
            ringUI.GetRing.transform.localRotation = Quaternion.Euler(0f, 0f, -step / 2f + _gap / 2f + i * step);
            ringUI.GetRing.color = new Color(1f, 1f, 1f, 0.5f);

            ringUI.GetIcon.transform.localPosition = ringUI.GetRing.transform.localPosition + Quaternion.AngleAxis(i * step, Vector3.forward) * Vector3.up * _iconDistance;

            float openDelay = .25f / _numberOfSlots * i;

            ringUI.Init(openDelay);

            ringUI.OnSelected = () =>
            {
                OnRingUISelected?.Invoke(ringUI);
            };
        }
    }

    private void Update()
    {
        HighlightSlot(HandleSelection());
        HandleInput();
    }

    public int HandleSelection()
    {
        float step = 360f / _numberOfSlots;
        float _mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2), Vector3.forward) + step / 2f);
        int _slot = (int)(_mouseAngle / step);
        return _slot;
    }

    public void HighlightSlot(int currentSlot)
    {
        if (_selectedSlot != currentSlot)
        {
            if (_selectedSlot != -1)
            {
                _ringUIs[_selectedSlot].UnHighlight();
            }

            _selectedSlot = currentSlot;

            _ringUIs[_selectedSlot].Highlight();
        }
    }

    public void HandleInput()
    {
        if (_selectedSlot == -1) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            _ringUIs[_selectedSlot].Select();
        }
    }

    private float NormalizeAngle(float angle) => (angle + 360f) % 360f;
}
