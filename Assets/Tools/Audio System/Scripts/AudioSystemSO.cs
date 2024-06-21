using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Pelumi.AudioSystem
{
    [Serializable]
    public class AudioSaveData
    {
        public List<AudioVolumeData> audioVolumeData = new List<AudioVolumeData>();
    }

    [Serializable]
    public class AudioVolumeData
    {
        public AudioCategory AudioCategory;

        [Range(0, 100)]
        public float Volume;
    }

    [CreateAssetMenu(fileName = "AudioSettings", menuName = "Audio System/AudioSystemSO")]
    public class AudioSystemSO : ScriptableObject
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSaveData audioSaveData;

        public AudioMixer AudioMixer => audioMixer;

        public void Mute(AudioCategory audioCategory, bool mute)
        {
            SetMute(audioCategory.ToString(), mute);
            SaveAudioVolumeData (audioCategory, GetVolumeFromMixer(audioCategory));
        }

        public void ChangeVolume(AudioCategory audioCategory, float volume)
        {
            SetVolume (audioCategory, volume);
            SaveAudioVolumeData(audioCategory, volume);
        }

        public float GetVolume(AudioCategory audioCategory)
        {
            foreach (AudioVolumeData data in audioSaveData.audioVolumeData)
            {
                if (data.AudioCategory == audioCategory)
                {
                    return data.Volume;
                }
            }
            return 0;
        }

        public void LoadAudioVolumeData()
        {
            LoadAll();

            foreach (AudioVolumeData data in audioSaveData.audioVolumeData)
            {
                SetVolume(data.AudioCategory, data.Volume);
            }
        }

        private void SetMute(string key, bool mute)
        {
            audioMixer.SetFloat(key, mute ? -80 : 0);
        }

        private void SetVolume(AudioCategory audioCategory, float volume)
        {
            audioMixer.SetFloat(audioCategory.ToString(), ConvertVolumeToDecibel(volume));
        }

        private float GetVolumeFromMixer (AudioCategory audioCategory)
        {
            audioMixer.GetFloat(audioCategory.ToString(), out float volume);
            return ConvertDecibelToVolume(volume);
        }

        private float ConvertVolumeToDecibel(float volume)
        {
            volume /= 100;
            if (volume <= 0) volume = 0.0001f;
            return (Mathf.Log10(volume) * 20);
        }

        private float ConvertDecibelToVolume(float decibel)
        {
            return (Mathf.Pow(10, decibel / 20)) * 100;
        }

        private void RefreshVolumeData()
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("");
            List<AudioVolumeData> nonExistingAudioVolume = new List<AudioVolumeData>();

            for (int i = 0; i < audioSaveData.audioVolumeData.Count; i++)
            {
                for (int j = 0; j < groups.Length; j++)
                {
                    if (audioSaveData.audioVolumeData[i].AudioCategory.ToString() == Enum.GetName(typeof(AudioCategory), j))
                    {
                        groups[j].audioMixer.GetFloat(audioSaveData.audioVolumeData[i].AudioCategory.ToString(), out float volume);
                        audioSaveData.audioVolumeData[i].Volume = ConvertDecibelToVolume(volume);
                        break;
                    }

                    nonExistingAudioVolume.Add(audioSaveData.audioVolumeData[i]);
                }
            }

            foreach (AudioVolumeData index in nonExistingAudioVolume)
            {
                audioSaveData.audioVolumeData.Remove(index);
            }

            foreach (AudioMixerGroup group in groups)
            {
                if (audioSaveData.audioVolumeData.Exists(x => x.AudioCategory.ToString() == group.name)) continue;
                group.audioMixer.GetFloat(group.name, out float volume);
                AudioCategory audioCategory = (AudioCategory)Enum.Parse(typeof(AudioCategory), group.name);
                audioSaveData.audioVolumeData.Add(new AudioVolumeData() { AudioCategory = audioCategory, Volume = ConvertDecibelToVolume(volume) });
            }
        }

        private void SaveAudioVolumeData(AudioCategory audioCategory, float volume)
        {
            foreach (AudioVolumeData data in audioSaveData.audioVolumeData)
            {
                if (data.AudioCategory == audioCategory)
                {
                    data.Volume = volume;
                    break;
                }
            }

            SaveAll();
        }

        private void SaveAll()
        {
            string json = JsonUtility.ToJson(audioSaveData);
            PlayerPrefs.SetString("AudioSaveData", json);
        }

        private void LoadAll()
        {
            string json = PlayerPrefs.GetString("AudioSaveData", "");
            if (String.IsNullOrEmpty(json)) return;
            audioSaveData = JsonUtility.FromJson<AudioSaveData>(json);
        }

#if UNITY_EDITOR

        public void Generate()
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("");
            List<string> audioList = new List<string>();

            foreach (AudioMixerGroup group in groups)
            {
                audioList.Add(group.name);
            }

            string audioEnum = typeof(AudioCategory).ToString();
            audioEnum = audioEnum.Substring(audioEnum.LastIndexOf(".") + 1);

            CreateEnum(audioEnum, audioList);

            RefreshVolumeData();

           PlayerPrefs.DeleteKey("AudioVolumeData");
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

            // Get the path of the ScriptableObject asset
            string assetPath = AssetDatabase.GetAssetPath(this);

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

#if UNITY_EDITOR
    [CustomEditor(typeof(AudioSystemSO))]
    public class AudioSystemSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AudioSystemSO audioSystemSO = (AudioSystemSO)target;

            if (GUILayout.Button("Generate"))
            {
                audioSystemSO.Generate();
            }

            //if (GUILayout.Button("Clear PlayerPref"))
            //{
            //    PlayerPrefs.DeleteKey("AudioVolumeData");
            //}
        }
    }
#endif
}
