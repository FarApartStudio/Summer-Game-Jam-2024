using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Pelumi.AudioSystem
{
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
        [SerializeField] private List<AudioVolumeData> audioVolumeData = new List<AudioVolumeData>();

        public AudioMixer AudioMixer => audioMixer;

        public void SetMute(string key, bool mute)
        {
            audioMixer.SetFloat(key, mute ? -80 : 0);
        }

        public void ChangeVolume(AudioCategory audioCategory, float volume)
        {
            SetVolume (audioCategory, volume);
            SaveAudioVolumeData();
        }

        private void SetVolume(AudioCategory audioCategory, float volume)
        {
            audioMixer.SetFloat(audioCategory.ToString(), ConvertVolumeToDecibel(volume));
        }

        public float GetVolume (AudioCategory audioCategory)
        {
            audioMixer.GetFloat(audioCategory.ToString(), out float volume);
            return ConvertDecibelToVolume(volume);
        }

        public float ConvertVolumeToDecibel(float volume)
        {
            volume /= 100;
            if (volume <= 0) volume = 0.0001f;
            return (Mathf.Log10(volume) * 20);
        }

        public float ConvertDecibelToVolume(float decibel)
        {
            return (Mathf.Pow(10, decibel / 20)) * 100;
        }

        public void LoadAudioVolumeData()
        {
           // LoadFromPlayerPrefs();

            foreach (AudioVolumeData data in audioVolumeData)
            {
                SetVolume(data.AudioCategory, data.Volume);
            }
        }

        public void SaveAudioVolumeData()
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("");
            List<AudioVolumeData> nonExistingAudioVolume = new List<AudioVolumeData>();

            for (int i = 0; i < audioVolumeData.Count; i++)
            {
                for (int j = 0; j < groups.Length; j++)
                {
                    if (audioVolumeData[i].AudioCategory.ToString() == Enum.GetName(typeof(AudioCategory), j))
                    {
                        groups[j].audioMixer.GetFloat(audioVolumeData[i].AudioCategory.ToString(), out float volume);
                        audioVolumeData[i].Volume = ConvertDecibelToVolume(volume);
                        break;
                    }

                    nonExistingAudioVolume.Add(audioVolumeData[i]);
                }
            }

            foreach (AudioVolumeData index in nonExistingAudioVolume)
            {
                audioVolumeData.Remove(index);
            }

            foreach (AudioMixerGroup group in groups)
            {
                if (audioVolumeData.Exists(x => x.AudioCategory.ToString() == group.name)) continue;
                group.audioMixer.GetFloat(group.name, out float volume);
                AudioCategory audioCategory = (AudioCategory)Enum.Parse(typeof(AudioCategory), group.name);
                audioVolumeData.Add(new AudioVolumeData() { AudioCategory = audioCategory, Volume = ConvertDecibelToVolume(volume) });
            }

          //  SaveAsPlayerPef();
        }

        public void SaveAsPlayerPef ()
        {
            // convert audioVolumeData to json
            string json = JsonUtility.ToJson(audioVolumeData);
            // save json to player prefs
            PlayerPrefs.SetString("AudioVolumeData", json);
        }

        public void LoadFromPlayerPrefs ()
        {
            // get json from player prefs
            string json = PlayerPrefs.GetString("AudioVolumeData", null);
            if (json != null)
            audioVolumeData = JsonUtility.FromJson<List<AudioVolumeData>>(json);
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

            SaveAudioVolumeData();
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

            if (GUILayout.Button("Save"))
            {
                audioSystemSO.SaveAudioVolumeData();
            }

            if (GUILayout.Button("Load"))
            {
                audioSystemSO.LoadAudioVolumeData();
            }


            if (GUILayout.Button("Clear PlayerPref"))
            {
                PlayerPrefs.DeleteKey("AudioVolumeData");
            }
        }
    }
#endif
}
