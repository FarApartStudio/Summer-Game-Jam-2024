using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    public event EventHandler OnScanningObjectChanged;

    [Header("Interaction Settings")]
    [SerializeField] private Transform playerCameraTranform;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private float interactDistance;

    private GameObject lastActiveScannedGameObject;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        DetectObject();
    }

    public void ToggleInteraction(bool isEnabled)
    {
        Cursor.visible = !isEnabled;
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void DetectObject()
    {
        if (Physics.Raycast(playerCameraTranform.position, playerCameraTranform.forward, out RaycastHit raycastHit, interactDistance, detectLayer))
        {
            Debug.DrawLine(playerCameraTranform.position, raycastHit.point, Color.green);
            DetectScannable(raycastHit);
            DetectInteractable(raycastHit);
        }
        else
        {
            Debug.DrawRay(playerCameraTranform.position, playerCameraTranform.forward * interactDistance, Color.red);
            DeScanLastObject();
        }
    }

    public void DeScanLastObject()
    {
        if (lastActiveScannedGameObject != null)
        {
            lastActiveScannedGameObject = null;
            OnScanningObjectChanged?.Invoke(lastActiveScannedGameObject, EventArgs.Empty);
        }
    }

    public void DetectInteractable(RaycastHit raycastHit)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (raycastHit.transform.TryGetComponent(out IInteractable interactable))
            {
                if (Vector3.Distance(transform.position, raycastHit.transform.position) < interactDistance)
                {
                    interactable.Interact();
                }
            }
        }
    }

    public void DetectScannable(RaycastHit raycastHit)
    {
        if (lastActiveScannedGameObject == raycastHit.transform.gameObject) return;
        if (raycastHit.transform.TryGetComponent(out IScannable scannable))
        {
            if (Vector3.Distance(transform.position, raycastHit.transform.position) < interactDistance)
            {
                lastActiveScannedGameObject = raycastHit.transform.gameObject;
            }
        }
    }
}

public interface IInteractable
{
    public void Interact();
}

public interface IScannable
{
    public string Scan();
}