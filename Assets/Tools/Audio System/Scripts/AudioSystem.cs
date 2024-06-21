using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.Pool;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Pelumi.AudioSystem
{
    public struct AudioSfxEntry
    {
        AudioTypeID audioType;
        AudioCategory category;
        bool randomPitch;
        float pitch;
    }


    public class AudioSystem : MonoBehaviour
    {
        public static AudioSystem Instance { get; private set; }

        [Header("Resources")]
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private Audio3DPlayer audio3DPlayerPrefab;
        [SerializeField] private AudioSystemSO audioManagerSO;
        [SerializeField] private AudioBank audioBank;

        private Dictionary<AudioCategory, AudioSource> audioSources = new Dictionary<AudioCategory, AudioSource>();
        private ObjectPool<Audio3DPlayer> audio3DPool;
        private AudioMixerGroup[] audioMixerGroups;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                audioManagerSO.LoadAudioVolumeData();
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
            audioManagerSO.ChangeVolume(audioCategory, volume);
        }

        private IEnumerator PlayMusicFade(AudioSource musicPlayer, AudioClip audioClip, bool loop = true, float fadeDuration = 1.0f, float volume  = 1)
        {
            // Fade out the music player
            float elapsedTime = 0f;
            float startVolume = musicPlayer.volume;
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
            targetVolume = volume;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                musicPlayer.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
                yield return null;
            }

            musicPlayer.volume = volume; // Ensure the volume is set to the target value
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

        public static AudioClip PlayOneShotAudio(string id, AudioCategory category, bool randomPitch = false, float pitch = 1, float volume = 1)
        {
            return PlayOneShotAudio(Instance.audioBank.GetAsset (id), category, randomPitch, pitch, volume);
        }

        public static AudioClip PlayOneShotAudio(AudioTypeID audioType, AudioCategory category, bool randomPitch = false, float pitch = 1, float volume = 1)
        {
            return PlayOneShotAudio(Instance.audioBank.GetAsset(audioType.ToString()), category, randomPitch, pitch, volume);
        }

        public static AudioClip PlayOneShotAudio(AudioClip audioClip, AudioCategory category, bool randomPitch = false, float pitch = 1, float volume = 1)
        {
            if (Instance == null) return null;

            AudioSource sfxSource = Instance.audioSources[category];
            sfxSource.pitch = randomPitch ? Random.Range(0.8f, 1.2f) : pitch;
            sfxSource.volume = volume;
            sfxSource.PlayOneShot(audioClip);
            return audioClip;
        }

        public static void PlayAudio(AudioTypeID audioType, AudioCategory audioCategory, bool loop = true, float fadeduration = 1, float volume = 1)
        {
            PlayAudio(audioType.ToString(), audioCategory, loop, fadeduration, volume);
        }

        public static void PlayAudio(string id, AudioCategory audioCategory, bool loop = true, float fadeduration = 1, float volume = 1)
        {
            PlayAudio (Instance.audioBank.GetAsset (id), audioCategory, loop, fadeduration, volume);
        }

        public static void PlayAudio(AudioClip audioClip, AudioCategory audioCategory, bool loop = true, float fadeduration = 1, float volume = 1)
        {
            if (!InstanceExists()) return;
            Instance.StopAllCoroutines();
            Instance.StartCoroutine(Instance.PlayMusicFade(Instance.audioSources[audioCategory], audioClip, loop, fadeduration, volume));
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

#if UNITY_EDITOR

        [Button]
        public void GenerateEnumFromBank()
        {
            List<string> audioList = audioBank.GetAllID();
            string audioEnum = typeof(AudioTypeID).ToString();
            audioEnum = audioEnum.Substring(audioEnum.LastIndexOf(".") + 1);
            CreateEnum(audioEnum, audioList);
        }

        public void CreateEnum(string enumName, List<string> stringList)
        {
            if (stringList.Count == 0)
            {
                Debug.LogWarning("String list is empty. Add string values to create an enum.");
                return;
            }

            string enumScript = "using UnityEngine;\n\n";

            enumScript += $"namespace {"Pelumi.AudioSystem"}\n";
            enumScript += "{\n";

            enumScript += $"public enum {enumName}\n";
            enumScript += "{\n";

            // Generate enum values from the string list
            for (int i = 0; i < stringList.Count; i++)
            {
                enumScript += $"\t{stringList[i]} = {i},\n";
            }

            enumScript += "}\n";

            enumScript += "}\n";

            string assetPath = AssetDatabase.GetAssetPath(audioManagerSO);

            // Generate the enum script file path in the same directory as the ScriptableObject
            string scriptPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assetPath), $"{enumName}.cs");

            // Check if the script file already exists and delete it
            if (System.IO.File.Exists(scriptPath))
            {
                System.IO.File.Delete(scriptPath);
            }

            // Save the generated enum script as a C# file in the same directory as the ScriptableObject
            System.IO.File.WriteAllText(scriptPath, enumScript);

            // Refresh asset database to recognize the newly created script
            AssetDatabase.Refresh();
        }
#endif
    }
}
