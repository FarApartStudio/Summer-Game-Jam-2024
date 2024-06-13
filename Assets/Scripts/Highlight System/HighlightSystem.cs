using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSystem : MonoBehaviour
{
    public static HighlightSystem Instance { get; private set; }

    private Transform _playerCameraTranform;

    [SerializeField] private LayerMask _detectLayer;

    [Header("Scanning")]
    [SerializeField] private float _scanningDistance;
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private float _highlightValue = 0.005f;

    private GameObject _lastHighlightedGameObject;

    private void Awake()
    {
        Instance = this;
        _playerCameraTranform = Camera.main.transform;
        _highlightMaterial.SetFloat("_Multiply_Value", _highlightValue);
    }

    void Update()
    {
        HandleHighligting();
    }

    public void HandleHighligting()
    {
        if (Physics.Raycast(_playerCameraTranform.position, _playerCameraTranform.forward, out RaycastHit raycastHit, _scanningDistance, _detectLayer))
        {
            Debug.DrawLine(_playerCameraTranform.position, raycastHit.point, Color.green);
            TryHighlightObject(raycastHit);
        }
        else
        {
            TryUnHighlightLastObject();
        }
    }

    public void TryHighlightObject(RaycastHit raycastHit)
    {
        if (_lastHighlightedGameObject != raycastHit.collider.gameObject)
        {
            TryUnHighlightLastObject();

            _lastHighlightedGameObject = raycastHit.collider.gameObject;

            ToggleHighlight(_lastHighlightedGameObject, true);
        }
    }

    public void TryUnHighlightLastObject()
    {
        if (_lastHighlightedGameObject != null)
        {
            ToggleHighlight(_lastHighlightedGameObject, false);
            _lastHighlightedGameObject = null;
        }
    }

    public void ToggleHighlight(GameObject gameObject, bool isOn = false)
    {
        if (gameObject.TryGetComponent(out HighlightSurface highlightSurface))
        {
            highlightSurface.ToggleVisibility(isOn);
        }
        foreach (Transform child in gameObject.transform) ToggleHighlight(child.gameObject, isOn);
    }
}
