using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] GameObject frontFace;
    [SerializeField] GameObject backFace;
    bool isFlipped = false;
    float absoluteAngle = 0f;

    private void Start()
    {
        FlipEffect();
    }

    void Update()
    {
        UpdateView();
    }

    public void FlipEffect()
    {
        transform.JuicyRotate(Vector3.up * 180, .5f)
            .SetEase(Ease.EaseOutSine)
            .SetLoop(0, LoopType.Incremental)
            .Start();
    }

    public void UpdateView()
    {
        absoluteAngle = Mathf.Abs(transform.rotation.eulerAngles.y);
        isFlipped = absoluteAngle > 90f && absoluteAngle < 270f;
        frontFace.SetActive(!isFlipped);
        backFace.SetActive(isFlipped);
    }
}
