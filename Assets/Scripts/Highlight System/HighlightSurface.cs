using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightSurface : MonoBehaviour
{
    private int _layerMask;
    private int _highlightLayer;

    private void Awake()
    {
        _layerMask = gameObject.layer;
        _highlightLayer = LayerMask.NameToLayer("Scanning");
    }

    public void ToggleVisibility(bool isOn)
    {
        gameObject.layer = isOn ? _highlightLayer : _layerMask;
    }
}
