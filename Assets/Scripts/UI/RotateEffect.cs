using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;

public class RotateEffect : MonoBehaviour
{
    [SerializeField] private float _duration = 1f;
    [SerializeField] private bool _autoPlay = true;
    private JuicerRuntimeCore<Vector3> rotateEffect;

    public void SetAutoPlay (bool value)
    {
        _autoPlay = value;
    }

    private void OnEnable()
    {
        if (_autoPlay)
        {       
            StartRotate();
        }
    }

    private void OnDisable()
    {
        if (_autoPlay)
        {
            StopRotate();
        }
    }

    public void StartRotate()
    {
        if (rotateEffect == null)
        {
            rotateEffect = transform.JuicyLocalRotate(Vector3.forward * 360, _duration);
            rotateEffect.SetLoop(0, LoopType.Incremental);
        }

        rotateEffect.Start();
    }

    public void StopRotate()
    {
        if (rotateEffect == null) return;
        rotateEffect.Pause();
    }
}
