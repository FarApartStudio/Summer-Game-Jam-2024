using System;
using System.Collections;
using System.Collections.Generic;
using Pelumi.Juicer;
using UnityEngine;

namespace Pelumi.UISystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class Menu : MonoBehaviour
    {
        public enum ExitBehaviour 
        {   
            Disable,
            Destory
        }

        public enum Status 
        {
            Opened, 
            Closed 
        }


        [SerializeField] protected int _viewPriority = 0;
        [SerializeField] protected ExitBehaviour _closeBehaviour = ExitBehaviour.Disable;

        protected Canvas[] _canvas;
        protected Status status = Status.Closed;
        protected Visibility _visibility = Visibility.Visible;
        protected CanvasGroup _canvasGroup;

        private JuicerRuntimeCore<float> _visibilityEffect;

        public ExitBehaviour GetCloseBehaviour() => _closeBehaviour;
        public Status GetStatus() => status;

        private void OnValidate()
        {
            InitializedAllCanvas();
        }

        public void InitialiseMenu()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
            _visibilityEffect = _canvasGroup.JuicyAlpha(0, 0.25f);
            _visibilityEffect.SetTimeMode(TimeMode.Unscaled);
            InitializedAllCanvas();
            OnCreated();
        }

        private void InitializedAllCanvas()
        {
            _canvas = GetComponentsInChildren<Canvas>();
            for (int i = 0; i < _canvas.Length; i++)
            {
                _canvas[i].sortingOrder = _viewPriority + i;
            }
        }

        protected void OpenMenu()
        {
            status = Status.Opened;
            gameObject.SetActive(true);
            OnOpened();
            _visibilityEffect.StartWithNewDestination(1);
        }

        protected void CloseMenu()
        {
            status = Status.Closed;
            OnClosed();
            _visibilityEffect.StartWithNewDestination(0);

            switch (_closeBehaviour)
            {
                case ExitBehaviour.Disable:
                    gameObject.SetActive(false);
                    break;
                case ExitBehaviour.Destory:
                    OnDestoryInvoked();
                    Destroy(gameObject);
                    break;
            }
        }

        protected abstract void OnCreated();

        protected abstract void OnOpened();

        protected abstract void OnClosed();

        protected abstract void OnDestoryInvoked();

        public void SetVisibility(Visibility state)
        {
            _visibility = state;
           _visibilityEffect.StartWithNewDestination(state == Visibility.Visible ? 1 : 0);
        }

        public void ToggleInteractivity (bool state)
        {
            _canvasGroup.interactable = state;
        }

        public abstract void ResetMenu();
    }
}

