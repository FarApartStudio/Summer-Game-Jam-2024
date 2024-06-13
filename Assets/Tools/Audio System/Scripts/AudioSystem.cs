using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Pelumi.AudioSystem
{

    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem Instance { get; private set; }

        [Header("Resources")]
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private Audio3DPlayer audio3DPlayerPrefab;
        [SerializeField] private AudioSystemSO audioManagerSO;

        private Dictionary<AudioCategory, AudioSource> audioSources = new Dictionary<AudioCategory, AudioSource>();
        private ObjectPool<Audio3DPlayer> audio3DPool;
        private AudioMixerGroup[] audioMixerGroups;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
                Init();
            }
            else Destroy(gameObject);
        }

        public void Init()
        {
            audioMixerGroups = audioManagerSO.AudioMixer.FindMatchingGroups("");
            foreach (AudioMixerGroup group in audioMixerGroups)
            {
                AudioSource audioSource = Instantiate(audioSourcePrefab, transform);
                audioSource.outputAudioMixerGroup = group;
                AudioCategory audioCategory = (AudioCategory)Enum.Parse(typeof(AudioCategory), group.name);
                audioSources.Add(audioCategory, audioSource);
                audioSource.name = group.name;
            }

            audio3DPool = new ObjectPool<Audio3DPlayer>(On3DAudioCreated, On3DAudioPooledPool, On3DAudioReleasedToPool);

            audioManagerSO.LoadAudioVolumeData();
        }

        public Audio3DPlayer On3DAudioCreated()
        {
            Audio3DPlayer audio3DPlayer = GameObject.Instantiate(Instance.audio3DPlayerPrefab);
            audio3DPlayer.transform.SetParent(transform);
            audio3DPlayer.OnFinished = (Audio3DPlayer player) => audio3DPool.Release(player);
            return audio3DPlayer;
        }

        public void On3DAudioPooledPool(Audio3DPlayer audio3DPlayer)
        {
            audio3DPlayer.gameObject.SetActive(true);
        }

        public void On3DAudioReleasedToPool(Audio3DPlayer audio3DPlayer)
        {
            audio3DPlayer.gameObject.SetActive(false);
        }

        public void SetVolume(AudioCategory audioCategory, float volume)
        {
            audioManagerSO.SetVolume(audioCategory, volume);
            audioManagerSO.SaveAudioVolumeData();
        }

        private IEnumerator PlayMusicFade(AudioSource musicPlayer, AudioClip audioClip, bool loop = true, float fadeDuration = 1.0f)
        {
            // Fade out the music player
            float elapsedTime = 0f;
            float startVolume = 1;
            float targetVolume = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                musicPlayer.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
                yield return null;
            }

            musicPlayer.volume = 0f; // Ensure the volume is set to the target value
            musicPlayer.Stop();

            // Start the new audio clip
            musicPlayer.clip = audioClip;
            musicPlayer.loop = loop;
            musicPlayer.Play();

            // Fade in the music player
            elapsedTime = 0f;
            startVolume = 0f;
            targetVolume = 1;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                musicPlayer.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
                yield return null;
            }

            musicPlayer.volume = 1; // Ensure the volume is set to the target value
        }

        private IEnumerator StopAudioWithFadeRoutine(AudioCategory audioCategory)
        {
            float speed = 0.05f;
            AudioSource musicPlayer = audioSources[audioCategory];

            while (musicPlayer.volume >= speed)
            {
                musicPlayer.volume -= speed;
                yield return new WaitForSecondsRealtime(0.1f);
            }

            musicPlayer.Stop();
        }

        private IEnumerator BlendTwoAudioRoutine(AudioCategory audioCategory, AudioClip intro, AudioClip loopMusic, bool loop = true)
        {
            AudioSource musicPlayer = audioSources[audioCategory];
            yield return PlayMusicFade(musicPlayer, intro, false);
            yield return new WaitForSecondsRealtime(musicPlayer.clip.length - 0.5f);
            yield return PlayMusicFade(musicPlayer, loopMusic, loop);
        }

        public AudioSource GetAudioSource(AudioCategory audioCategory) => audioSources[audioCategory];

        public static void PlayOneShotAudio(AudioClip audioClip, AudioCategory category, bool randomPitch = false)
        {
            if (Instance == null) return;

            AudioSource sfxSource = Instance.audioSources[category];
            sfxSource.pitch = randomPitch ? Random.Range(0.8f, 1.2f) : 1;
            sfxSource.PlayOneShot(audioClip);
            return;
        }

        public static void PlayAudio(AudioClip audioClip, AudioCategory audioCategory, bool loop = true)
        {
            if (!InstanceExists()) return;
            Instance.StopAllCoroutines();
            Instance.StartCoroutine(Instance.PlayMusicFade(Instance.audioSources[audioCategory], audioClip, loop));
        }

        public static void PauseAudio(AudioCategory audioCategory)
        {
            if (!InstanceExists()) return;
            Instance.audioSources[audioCategory].Pause();
        }

        public static void ResumeAudio(AudioCategory audioCategory)
        {
            if (!InstanceExists()) return;
            Instance.audioSources[audioCategory].UnPause();
        }

        public static void StopAudioWithFade(AudioCategory audioCategory)
        {
            if (!InstanceExists()) return;
            Instance.StartCoroutine(Instance.StopAudioWithFadeRoutine(audioCategory));
        }

        public static void BlendTwoAudio(AudioCategory audioCategory, AudioClip startAudio, AudioClip nextAudio, bool loop = true)
        {
            if (!InstanceExists()) return;
            Instance.StopAllCoroutines();
            Instance.StartCoroutine(Instance.BlendTwoAudioRoutine(audioCategory, startAudio, nextAudio, loop));
        }

        public static Audio3DPlayer Play3DSoundEffect(AudioClip audio, Vector3 position, float _minDistance = 1, float _maxDistance = 500, 
            float _dopplerLevel = 1, float _spread = 0, AudioRolloffMode _audioRolloffMode = AudioRolloffMode.Linear)
        {
            if (Instance == null) return null;
            Audio3DPlayer audio3DPlayer = Instance.audio3DPool.Get();
            audio3DPlayer.transform.position = position;
            audio3DPlayer.PlaySoundEffect(audio, _dopplerLevel, _spread, _audioRolloffMode, _minDistance, _maxDistance);
            return audio3DPlayer;
        }

        private static bool InstanceExists()
        {
            if (Instance == null) Debug.LogError("No Audio Manager in the scene");
            return Instance != null;
        }
    }
}
