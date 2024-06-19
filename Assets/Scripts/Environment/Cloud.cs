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
        hitObserver = GetComponent<HitObserver>();
        _renderer = GetComponent<Renderer>();
        hitObserver.GetOnHit.AddListener(OnHit);
    }

    private void OnHit()
    {
        _renderer.material = hitMaterial;
        _renderer.transform.JuicyShakePosition(.15f, new Vector3(0.02f, 0.05f, 0.05f));
        _renderer.transform.JuicyShakeScale(.15f, new Vector3(0.02f, 0.05f, 0.05f));
        _renderer.transform.JuicyShakePosition(.15f, new Vector3(0.02f, 0.05f, 0.05f));
    }
}
