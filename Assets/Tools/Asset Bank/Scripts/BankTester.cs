using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BankTester : MonoBehaviour
{
    [SerializeField] private string _gameObjectKey;
    [SerializeField] private GameObjectBank _gameObjectBank;

    [SerializeField] private string _particleKey;
    [SerializeField] private ParticleBank _particleBank;

    [SerializeField] private string _audioKey;
    [SerializeField] private AudioBank _audioBank;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private float _testIterationIndex = 1000;

    [ContextMenu("Test Chance")]
    public void TestChance()
    {
        for (int i = 0; i < _testIterationIndex; i++)
        {
            GameObject randomObject = _gameObjectBank.GetAsset(_gameObjectKey);
            Debug.Log(randomObject.name);
        }
    }

    [ContextMenu("Spawn Random Object")]
    public void SpawnObjectBank()
    {
        GameObject randomObject = _gameObjectBank.GetAsset(_gameObjectKey);
        Instantiate(randomObject, transform.position, Quaternion.identity);
    }

    [ContextMenu("Test Particle Bank")]

    public void TestParticleBank()
    {
        ParticleSystem particle = _particleBank.GetAsset(_particleKey);
        Instantiate(particle, transform.position, Quaternion.identity);
    }

    [ContextMenu("Test Audio Bank")]
    public void TestAudioBank()
    {
        AudioClip randomObject = _audioBank.GetAsset(_audioKey);
        _audioSource.PlayOneShot(randomObject);
    }
}
