using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.AudioSystem;

public class TestAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AudioSystem.Play3DSoundEffect(audioClip,transform.position);
        }
    }
}
