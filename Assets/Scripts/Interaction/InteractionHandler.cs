using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public event Action<Collider> OnInteractStart;
    public event Action<Collider> OnInteractEnd;

    [SerializeField] private LayerMask _interactableLayer;

    private bool _isInteracting;

    public bool IsInteracting => _isInteracting;

    private void OnTriggerEnter(Collider other)
    {
        if (_interactableLayer == (_interactableLayer | (1 << other.gameObject.layer)))
        OnInteractStart?.Invoke(other);
        _isInteracting = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactableLayer == (_interactableLayer | (1 << other.gameObject.layer)))
            OnInteractEnd?.Invoke(other);

        _isInteracting = false;
    }
}
