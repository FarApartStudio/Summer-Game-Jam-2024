using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pelumi.Juicer;
using System.Collections.Generic;
using Pelumi.AudioSystem;

public enum GameButtonVisiblity
{
    Visible,
    Hidden
}

public class AdvanceButton : Button, IPointerDownHandler, IPointerUpHandler
{
    public static event EventHandler OnAnyButtonHovered;

    public static event Action OnAnyButtonClicked;
    public static Action<GameButtonType> OnAnyButtonPointerDown;
    public event Action<bool> OnButtonStateChanged;

    [SerializeField] private GameButtonType buttonType;
    [SerializeField] private GameObject toggleIndicator;

    [SerializeField] private UnityEvent onHold;
    [SerializeField] private UnityEvent onRelease;

    [SerializeField] private UnityEvent<bool> onClickToggle;
    [SerializeField] private AdvanceButtonToggleGroup toggleGroup;

    private bool isOn = false;
    private JuicerRuntimeCore<Vector3> onClickSizeEffect;
    private List<JuicerRuntimeCore<Color>> OnClickColourEffect = new List<JuicerRuntimeCore<Color>>();

    private Image[] childImages = new Image[0];
    private Dictionary<Image, Color> childDefaultImageColors = new Dictionary<Image, Color>();

    [Space]
    [SerializeField] private Vector3 buttonUpScale = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private Vector3 buttonDownScale = new Vector3(0.9f, 0.9f, 0.9f);

    [Space]
    [SerializeField] public Color buttonDownColor = Color.gray;

    [Space]
    [SerializeField] private float buttonScaleDuration = 0.1f;
    [SerializeField] private float buttonDownDuration = 0.1f;
    [SerializeField] private float buttonUpDuration = 0.25f;

    private GameButtonVisiblity gameButtonVisiblity;

    public GameButtonType ButtonType { get { return buttonType; } }
    public GameButtonVisiblity GetGameButtonVisiblity { get { return gameButtonVisiblity; } }

    public UnityEvent OnHold { get { return onHold; } }

    public UnityEvent OnRelease { get { return onRelease; } }

    protected override void Awake()
    {
        base.Awake();
        childImages = GetComponentsInChildren<Image>();
        foreach (Image childImage in childImages)
        {
            childDefaultImageColors.Add(childImage, childImage.color);
        }

        transition = Selectable.Transition.None;

        if (!Application.isPlaying) return;

        for (int i = 0; i < childImages.Length; i++)
        {
            JuicerRuntimeCore<Color> juicerRuntime = childImages[i].JuicyColour(buttonDownColor, buttonDownDuration / 2);
            juicerRuntime.SetLoop(1).SetTimeMode(TimeMode.Unscaled);
            OnClickColourEffect.Add(juicerRuntime);
        }

        toggleGroup?.AddButton(this);
    }

    protected override void Start()
    {
        transition = Selectable.Transition.None;

        if (!Application.isPlaying) return;

        onClickSizeEffect = transform.JuicyScale(buttonUpScale, buttonScaleDuration);
        onClickSizeEffect.SetLoop(1).SetTimeMode(TimeMode.Unscaled);
    }

    public UnityEvent<bool> OnToggle
    {
        get { return onClickToggle; }
        set { onClickToggle = value; }
    }

    private void OnToggled()
    {
        isOn = !isOn;

        if (toggleGroup != null && !toggleGroup.AllowUnToggle && !isOn)
        {
            isOn = true;

            if (toggleGroup.SelectedButton == this)
            {
                return;
            }
        }

        if (toggleGroup != null && toggleGroup.AllowUnToggle && !isOn && toggleGroup.SelectedButton != this)
        {
            isOn = true;
        }

        Toggle (isOn);
    }

    public void Toggle(bool value)
    {
        isOn = value;
        onClickToggle?.Invoke(isOn);
        if (toggleIndicator)
            toggleIndicator.SetActive(isOn);
    }

    public void SetIsOn(bool value)
    {
        isOn = value;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        OnAnyButtonHovered?.Invoke(this, EventArgs.Empty);
        base.OnPointerEnter(eventData);
        onClickSizeEffect.StartWithNewDestination(buttonDownScale);

        AudioSystem.PlayOneShotAudio(AudioTypeID.UI_ButtonHover, AudioCategory.UI, true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onClickSizeEffect.StartWithNewDestination (Vector3.one);
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        if (gameButtonVisiblity == GameButtonVisiblity.Hidden)
        {
            return;
        }

        if (interactable)
        {
            AudioSystem.PlayOneShotAudio(AudioTypeID.UI_ButtonPress, AudioCategory.UI, true);

            OnAnyButtonClicked?.Invoke();
            OnToggled();
        }

        base.OnPointerClick(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnAnyButtonPointerDown?.Invoke(buttonType);
        OnButtonStateChanged?.Invoke(false);

        onClickSizeEffect.StartWithNewDestination(buttonDownScale);

        if (gameButtonVisiblity == GameButtonVisiblity.Hidden)
        {
            return;
        }

        base.OnPointerDown(eventData);
        if (interactable)
        {
            OnButtonStateChanged?.Invoke(true);
            onHold?.Invoke();
            //TriggerClickColorEffect(true);
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        onClickSizeEffect.StartWithNewDestination(Vector3.one);

        if (gameButtonVisiblity == GameButtonVisiblity.Hidden)
        {
            return;
        }

        base.OnPointerDown(eventData);
        if (interactable)
        {
            //TriggerClickColorEffect(false);
            OnButtonStateChanged?.Invoke(false);
            onRelease?.Invoke();
        }
    }

    public void SetVisibility(GameButtonVisiblity state)
    {
        StopClickColorEffect();

        gameButtonVisiblity = state;
        interactable = state == GameButtonVisiblity.Visible;

        foreach (Image childImage in childImages)
        {
            childImage.color = state == GameButtonVisiblity.Visible ? childDefaultImageColors[childImage] : Color.grey;
        }
    }

    public void TriggerClickColorEffect(bool on)
    {
        for (int i = 0; i < OnClickColourEffect.Count; i++)
        {
            OnClickColourEffect[i].StartWithNewDestination(on ? buttonDownColor : childDefaultImageColors[childImages[i]]);
        }
    }

    public void StopClickColorEffect()
    {
        for (int i = 0; i < OnClickColourEffect.Count; i++)
        {
            OnClickColourEffect[i].Stop();
        }
    }
}
