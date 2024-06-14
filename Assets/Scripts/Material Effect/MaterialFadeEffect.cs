using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFadeEffect : MonoBehaviour
{
    [SerializeField] private Material transparentMaterial;
    private Renderer[] renderers;
    private Dictionary<Renderer, Material> materialMap = new Dictionary<Renderer, Material>();

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            materialMap.Add(renderer, renderer.material);
        }
    }

    public void Show()
    {
        foreach (var renderer in renderers)
        {
            renderer.material = materialMap[renderer];
        }
    }

    public void Hide()
    {
        foreach (var renderer in renderers)
        {
            renderer.material = transparentMaterial;
        }
    }
}
