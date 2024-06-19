using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractionHandler))]
public class HealthKit : MonoBehaviour
{
    [SerializeField] private int healthAmount;
    private InteractionHandler interactionHandler;

    private void Awake()
    {
        interactionHandler = GetComponent<InteractionHandler>();
        interactionHandler.OnInteractStart += OnInteract;
    }

    private void OnInteract(Collider collider)
    {
        if (collider.TryGetComponent(out HealthController healthController) && healthController.RestoreHeal(healthAmount))
        {
            ObjectPoolManager.ReleaseObject(gameObject);
        }
    }
}
