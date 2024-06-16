using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresurePlate : MonoBehaviour
{
    private bool isPressed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pilot pilot))
        {
            isPressed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Pilot pilot))
        {
            isPressed = false;
        }
    }
}
