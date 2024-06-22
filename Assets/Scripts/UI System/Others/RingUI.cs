using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pelumi.Juicer;
using System;

namespace GameProject
{
    public class RingUI : MonoBehaviour
    {
        public Action OnSelected;

        [SerializeField] private Image icon;
        [SerializeField] private Image ring;

        public Image GetIcon => icon;
        public Image GetRing => ring;

        JuicerRuntime highlightRingColorEffect;
        JuicerRuntime highlightRingScaleEffect;
        JuicerRuntime highlihtIconScaleEffect;

        JuicerRuntime unHighlightRingColorEffect;
        JuicerRuntime unHighlightRingScaleEffect;
        JuicerRuntime unHighlightIconSacleEffect;

        JuicerRuntime selectedEffect;

        JuicerRuntime openEffect;

        private void Awake()
        {
            highlightRingColorEffect = ring.JuicyAlpha(0.75f, 0.15f).SetTimeMode(TimeMode.Unscaled);
            highlightRingScaleEffect = ring.transform.JuicyScale(1.1f, 0.25f).SetTimeMode(TimeMode.Unscaled);
            highlihtIconScaleEffect = icon.transform.JuicyScale(1.1f, 0.15f).SetTimeMode(TimeMode.Unscaled);

            unHighlightRingColorEffect = ring.JuicyAlpha(0.5f, 0.15f).SetTimeMode(TimeMode.Unscaled); ;
            unHighlightRingScaleEffect = ring.transform.JuicyScale(1f, 0.25f).SetTimeMode(TimeMode.Unscaled);
            unHighlightIconSacleEffect = icon.transform.JuicyScale(1f, 0.15f).SetTimeMode(TimeMode.Unscaled);

            selectedEffect = icon.JuicyColour(Color.yellow, 0.15f).SetLoop(2).SetTimeMode(TimeMode.Unscaled);
            selectedEffect.SetOnCompleted(() => { OnSelected?.Invoke(); });

            openEffect = transform.JuicyScale(1f, .2f).SetTimeMode(TimeMode.Unscaled);
        }

        public void Init(float delay)
        {
            transform.localScale = Vector3.zero;
            openEffect.SetDelay(delay).Start();
        }

        public void Highlight()
        {
            highlightRingColorEffect.Start();
            highlightRingScaleEffect.Start();
            highlihtIconScaleEffect.Start();
        }

        public void UnHighlight()
        {
            unHighlightRingColorEffect.Start();
            unHighlightRingScaleEffect.Start();
            unHighlightIconSacleEffect.Start();
        }

        public void Select()
        {
            selectedEffect.Start();
        }
    }
}
