using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionHandler : MonoBehaviour
{
    public event Action<Collider> OnInteractStart;
    public event Action<Collider> OnInteractEnd;

    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private UnityEvent OnStartInteract;
    [SerializeField] private UnityEvent OnEndInteract;


    private bool _isInteracting;

    public bool IsInteracting => _isInteracting;

    private void OnTriggerEnter(Collider other)
    {
        if (_interactableLayer != (_interactableLayer | (1 << other.gameObject.layer))) 
            return;

         _isInteracting = true;
        OnInteractStart?.Invoke(other);
        OnStartInteract?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactableLayer != (_interactableLayer | (1 << other.gameObject.layer)))
            return;

        _isInteracting = false;
        OnInteractEnd?.Invoke(other);
        OnEndInteract?.Invoke();
    }
}
