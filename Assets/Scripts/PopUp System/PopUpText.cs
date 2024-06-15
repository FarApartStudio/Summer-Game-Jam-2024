using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pelumi.ObjectPool;
using Pelumi.Juicer;
using TMPro;

public class PopUpText : MonoBehaviour
{
    [Header("TextPopUp")]
    [SerializeField] private TMP_Text textMeshPro;
    [SerializeField] private float scaleDuration;
    [SerializeField] private float moveDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float moveLenght;
    [SerializeField] private JuicerRuntime scaleEffect;
    [SerializeField] private JuicerRuntimeCore<Vector3> moveEffect;
    [SerializeField] private JuicerRuntime fadeEffect;

    private void Awake()
    {
        scaleEffect = transform.JuicyScale(Vector3.one, scaleDuration);
        scaleEffect.SetOnCompleted(() =>
        {
            moveEffect.Start();
            fadeEffect.Start();
        });

        moveEffect = transform.JuicyMove(transform.position, moveDuration);

        fadeEffect = textMeshPro.JuicyAlpha(0, fadeDuration).SetDelay(0.1f);
        fadeEffect.SetOnCompleted(() =>
        {
            textMeshPro.color = Color.white;
            textMeshPro.spriteAsset = null;
            ObjectPoolManager.ReleaseObject(this);
        });
    }

    public void Init(string text, Color color, Vector3 pos, float size = 60)
    {
        textMeshPro.fontSize = size;
        textMeshPro.text = text;
        textMeshPro.color = color;
        transform.position = pos;
        Effect();
    }

    public void Effect()
    {
        textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1);
        moveEffect.SetDestination(transform.position + Vector3.up * moveLenght);
        scaleEffect.Start(() => transform.localScale = Vector3.zero);
    }

    public void Init(string damageText, TMP_SpriteAsset spriteAsset, Vector2 finalPos, float size)
    {
        textMeshPro.text = damageText;
        textMeshPro.fontSize = size;
        textMeshPro.color = Color.white;
        textMeshPro.spriteAsset = spriteAsset;
        transform.position = finalPos;
        Effect();
    }
}
