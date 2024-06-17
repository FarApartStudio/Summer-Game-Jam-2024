using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatfromHandler : MonoBehaviour
{
    [SerializeField] private float revealDelay = .25f;

    [SerializeField] private Platform[] _platforms;

    public void ShowPlatfroms ()
    {
        StartCoroutine(ShowPlatfromsRoutine());
    }

    private IEnumerator ShowPlatfromsRoutine()
    {
        foreach (var platfrom in _platforms)
        {
            platfrom.ResetPlatfrom();
            yield return new WaitForSeconds(revealDelay);    
        }
    }

    public void ResetPlatfroms()
    {
        foreach (var platfrom in _platforms)
        {
            platfrom.ResetPlatfrom();
        }
    }

    [Button]
    private void GeneratePlatforms()
    {
        _platforms = GetComponentsInChildren<Platform>();
    }
}
