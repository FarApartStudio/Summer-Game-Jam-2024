using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pelumi.Juicer;

public class PresurePlate : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float yPressedPosition;
    [SerializeField] private float yReleasedPosition;
    [SerializeField] private Transform plate;
    [SerializeField] private float moveDuration;

    [Header("Events")]
    [SerializeField] private UnityEvent OnPressed;
    [SerializeField] private UnityEvent OnReleased;

    private bool isPressed;
    JuicerRuntimeCore<float> plateEffect;

    private void Awake()
    {
        plateEffect = plate.JuicyLocalMoveY(0, moveDuration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pilot pilot))
        {
            isPressed = true;
            OnPressed.Invoke();
            plateEffect.StartWithNewDestination(yPressedPosition);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Pilot pilot))
        {
            isPressed = false;
            OnReleased.Invoke();
            plateEffect.StartWithNewDestination(yReleasedPosition);
        }
    }
}
