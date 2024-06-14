using Pelumi.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectGroup
{
    public string id;
    public ParticleSystem[] vfx;
    public AudioEffect[] sfx;
}

[System.Serializable]
public class AudioEffect
{
    public string id;
    public AudioCategory category;
    public bool randomPitch;
}

public class EffectTrigger : MonoBehaviour
{
    [SerializeField] private EffectGroup[] _vfxGroups;

    public void PlayVFX(string id)
    {
        foreach (var vfxGroup in _vfxGroups)
        {
            if (vfxGroup.id == id)
            {
                foreach (var vfx in vfxGroup.vfx)
                {
                    vfx.Play();
                }

                foreach (var audioEffect in vfxGroup.sfx)
                {
                    AudioSystem.PlayOneShotAudio(audioEffect.id, audioEffect.category, audioEffect.randomPitch);
                }
            }
        }
    }

    public void PlayVFXByIndex(int index)
    {
        foreach (var vfx in _vfxGroups[index].vfx)
        {
            vfx.Play();
        }

        foreach (var audioEffect in _vfxGroups[index].sfx)
        {
            AudioSystem.PlayOneShotAudio(audioEffect.id, audioEffect.category, audioEffect.randomPitch);
        }
    }

    public void StopVFX(string id)
    {
        foreach (var vfxGroup in _vfxGroups)
        {
            if (vfxGroup.id == id)
            {
                foreach (var vfx in vfxGroup.vfx)
                {
                    vfx.Stop();
                }
            }
        }
    }
}
