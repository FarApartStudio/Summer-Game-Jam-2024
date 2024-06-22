using Pelumi.AudioSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public void PlaySFX(string id, AudioCategory category, bool randomPitch)
    {
        AudioSystem.PlayOneShotAudio(id, category, randomPitch);
    }

    public void PlaySFXWithRandomPitch(AudioClip clip)
    {
        AudioSystem.PlayOneShotAudio(clip, AudioCategory.Sfx, true);
    }

    public void PlaySFX(AudioEffect audioEffect)
    {
        AudioSystem.PlayOneShotAudio(audioEffect.id, audioEffect.category, audioEffect.randomPitch);
    }

    public void PlaySFX(AudioEffect[] audioEffects)
    {
        foreach (var audioEffect in audioEffects)
        {
            AudioSystem.PlayOneShotAudio(audioEffect.id, audioEffect.category, audioEffect.randomPitch);
        }
    }
}
