using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

public class CinemachineCameraShaker
{
    public CinemachineCameraShaker(CinemachineImpulseSource bumbSource, CinemachineImpulseSource explosionSource,
        CinemachineImpulseSource rumbleSource, CinemachineImpulseSource recoilSource, CinemachineImpulseSource genericSource, CinemachineImpulseListener listener)
    {
        impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Bump, bumbSource);
        impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Explosion, explosionSource);
        impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Rumble, rumbleSource);
        impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Recoil, recoilSource);

        shakesState.Add(CinemachineImpulseDefinition.ImpulseShapes.Bump, false);
        shakesState.Add(CinemachineImpulseDefinition.ImpulseShapes.Explosion, false);
        shakesState.Add(CinemachineImpulseDefinition.ImpulseShapes.Rumble, false);
        shakesState.Add(CinemachineImpulseDefinition.ImpulseShapes.Recoil, false);
        source = genericSource;
        impulseListener = listener;
    }


    Dictionary<CinemachineImpulseDefinition.ImpulseShapes, CinemachineImpulseSource> impulseSources = new();

    CinemachineImpulseSource source;
    CinemachineImpulseListener impulseListener;
    Dictionary<CinemachineImpulseDefinition.ImpulseShapes, bool> shakesState = new();

    public void ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes shape, float duration = 0.1f,
        float force = 1f,
        Vector2 velocity = default)
    {
        if (shakesState.TryGetValue(shape, out var isShaking))
        {
            if (isShaking)
                return;

            ResetShakeState(shape, duration);
            if (impulseSources.TryGetValue(shape, out var source))
            {
                source.m_ImpulseDefinition.m_ImpulseDuration = duration;
                if (velocity.Equals(Vector2.zero))
                {
                    source.GenerateImpulseWithForce(force);
                }
                else
                {
                    source.GenerateImpulseWithVelocity(velocity);
                }
            }
        }
    }

    public void ShakeCamera(ScreenShakeProfile profile)
    {
        SetupSettingsForGenericSource(profile);
        source.GenerateImpulseWithForce(profile.shakeForce);
    }

    void SetupSettingsForGenericSource(ScreenShakeProfile profile)
    {
        var definition = source.m_ImpulseDefinition;
        definition.m_ImpulseDuration = profile.shakeTime;
        source.m_DefaultVelocity = profile.defaultvelocity;
        definition.m_CustomImpulseShape = profile.impulseCurve;

        impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;

    }


    void ResetShakeState(CinemachineImpulseDefinition.ImpulseShapes shake, float duration)
    {
        shakesState[shake] = true;

        Juicer.WaitForSecondsRealtime(duration, new JuicerCallBack(() =>
        {
            shakesState[shake] = false;
        }));
    }
}
