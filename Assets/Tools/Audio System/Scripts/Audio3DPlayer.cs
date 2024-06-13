using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pelumi.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class Audio3DPlayer : MonoBehaviour
    {
        public Action<Audio3DPlayer> OnFinished;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySoundEffect(AudioClip audioID, float _dopplerLevel = 1, float _spread = 0, AudioRolloffMode _audioRolloffMode = AudioRolloffMode.Linear, float _minDistance = 1, float _maxDistance = 500)
        {
            audioSource.clip = audioID;
            audioSource.dopplerLevel = _dopplerLevel;
            audioSource.spread = _spread;
            audioSource.rolloffMode = _audioRolloffMode;
            audioSource.minDistance = _minDistance;
            audioSource.maxDistance = _maxDistance;
            audioSource.Play();
            StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            yield return new WaitForSecondsRealtime(audioSource.clip.length + 1f);
            OnFinished?.Invoke(this);
        }
    }
}
