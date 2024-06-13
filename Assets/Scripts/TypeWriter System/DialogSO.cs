using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New Dialog", menuName = "Dialog System / NewDialog", order = 1)]
public class DialogSO : ScriptableObject
{
    public TextSequence[] textSequences;

    [Serializable]
    public class TextSequence
    {
        [TextArea(2, 25)]
        public string messages;
        public AudioClip audioClip;
        public float textDuration;
    }
}
