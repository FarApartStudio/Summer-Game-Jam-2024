using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.Juicer;

public class MaterialFlashEffect : MonoBehaviour
{
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private Renderer _renderer;
    private Material _defaultMaterial;

    private void Awake()
    {
        _defaultMaterial =  _renderer.material;
    }

    IEnumerator FlashRoutine(float duration)
    {
        _renderer.material = _flashMaterial;
        yield return new WaitForSeconds(duration);
        _renderer.material = _defaultMaterial;
    }

    public void Flash(float duration)
    {
        StartCoroutine(FlashRoutine(duration));
    }
}
