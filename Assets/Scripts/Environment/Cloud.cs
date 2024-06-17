using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

[RequireComponent(typeof(HitObserver))]
public class Cloud : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material hitMaterial;

    private HitObserver hitObserver;
    private Renderer _renderer;

    private void Awake()
    {
        hitObserver.GetOnHit.AddListener(OnHit);
        _renderer = GetComponent<Renderer>();
    }

    private void OnHit()
    {
        _renderer.material = hitMaterial;
    }
}
