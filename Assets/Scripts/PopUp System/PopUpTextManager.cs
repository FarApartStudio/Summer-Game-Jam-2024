using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpTextManager : MonoBehaviour
{
    public enum PopUpType
    {
        World,
        UI
    }

    public static PopUpTextManager Instance { get; private set; }

    [SerializeField] private RectTransform uiPopUpOrigin;
    [SerializeField] private PopUpText worldPopUpPrefab;
    [SerializeField] private PopUpText uiPopUpPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PopUpTextAtTransfrom(PopUpType popUpType, Transform spawnPosition, Vector3 randomIntensity, string text, Color color, bool parent = false)
    {
        PopUpText textPopUp = GetPopUpText (popUpType);
        if (parent) textPopUp.transform.SetParent(spawnPosition);
        SetPopUpInfo(popUpType, textPopUp, spawnPosition.position, randomIntensity, text, color);
    }

    public void PopUpAtTextPosition(PopUpType popUpType, Vector3 spawnPosition, Vector3 randomIntensity, string text, Color color, float txtSize = 60)
    {
        PopUpText textPopUp = GetPopUpText(popUpType);
        SetPopUpInfo(popUpType,textPopUp, spawnPosition, randomIntensity, text, color, size: txtSize);
    }

    public void PopUpTextAtTransfrom(PopUpType popUpType, Transform damageModelTarget, Vector3 randomIntensity, string damageText, TMP_SpriteAsset spriteAsset, float size, bool parent = false)
    {
        PopUpText textPopUp = GetPopUpText(popUpType);
        if (parent) textPopUp.transform.SetParent(damageModelTarget);
        SetPopUpInfo(popUpType, textPopUp, damageModelTarget.position, randomIntensity, damageText, spriteAsset, size);
    }

    private void SetPopUpInfo(PopUpType popUpType ,PopUpText textPopUp, Vector3 spawnPosition, Vector3 randomIntensity, string text, Color color, float size = 60)
    {
        Vector2 finalPos = spawnPosition += new Vector3
           (
           Random.Range(-randomIntensity.x, randomIntensity.x),
               Random.Range(-randomIntensity.y, randomIntensity.y),
               Random.Range(-randomIntensity.z, randomIntensity.z)
           );

        switch (popUpType)
        {
            case PopUpType.UI:
                finalPos = Camera.main.WorldToScreenPoint(spawnPosition);
                break;
        }

        textPopUp.Init(text, color, finalPos, size);
    }

    private void SetPopUpInfo(PopUpType popUpType ,PopUpText textPopUp, Vector3 spawnPosition, Vector3 randomIntensity, string damageText, TMP_SpriteAsset spriteAsset, float size)
    {
        Vector2 finalPos = spawnPosition += new Vector3
        (
            Random.Range(-randomIntensity.x, randomIntensity.x),
            Random.Range(-randomIntensity.y, randomIntensity.y),
            Random.Range(-randomIntensity.z, randomIntensity.z)
        );

        switch (popUpType)
        {
            case PopUpType.UI:
                finalPos = Camera.main.WorldToScreenPoint(spawnPosition);
                break;
        }

        textPopUp.Init(damageText, spriteAsset, finalPos, size);
    }

    PopUpText GetPopUpText(PopUpType popUpType )
    {
        switch (popUpType)
        {
            case PopUpType.World:
                return ObjectPoolManager.SpawnObject(worldPopUpPrefab);
            case PopUpType.UI:
                return ObjectPoolManager.SpawnObject(uiPopUpPrefab, uiPopUpOrigin);
            default:
                return null;
        }
    }
}
