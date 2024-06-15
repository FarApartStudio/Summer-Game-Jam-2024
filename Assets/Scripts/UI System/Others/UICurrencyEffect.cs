using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using Pelumi.AudioSystem;

public class UICurrencyEffect : MonoBehaviour
{
    public static bool _isPlayingSfx;

    [SerializeField] private Image _currencyIcon;
    private Transform _target;
    private JuicerRuntimeCore<Vector3> _scaleEffect;
    private JuicerRuntimeCore<Vector3> _positionEffect;
    private bool _shouldPlaySfx;

    private void Awake()
    {
        _isPlayingSfx = false;
        _scaleEffect = transform.JuicyScale(Vector3.one, 0.25f);
        _scaleEffect.SetLoop(2).SetStepDelay(0.25f);
        _positionEffect = transform.JuicyMove(Vector3.zero, .25f);
        _positionEffect.SetOnCompleted(() =>
        {
            if (_shouldPlaySfx && !_isPlayingSfx)
            {
                _isPlayingSfx = true;
                AudioSystem.PlayOneShotAudio(AudioTypeID.UI_Currency, AudioCategory.UI, true);

                Juicer.WaitForSecondsRealtime(.15f, new JuicerCallBack(() =>
                {
                    _isPlayingSfx = false;
                }));
            }

            ObjectPoolManager.ReleaseObject(this);
        });
    }

    public void SetUp(Sprite icon, Transform target, bool sound = true)
    {
        _currencyIcon.sprite = icon;
        _target = target;
        _shouldPlaySfx = sound;
        transform.localScale = Vector3.zero / 2;
        _scaleEffect.Start();
        _positionEffect.SetDelay(Random.Range(0.1f, 0.3f));
        _positionEffect.StartWithNewDestination(_target.position);
    }
}
