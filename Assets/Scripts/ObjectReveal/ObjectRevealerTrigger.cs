using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRevealerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ObjectRevealer objectRevealer = other.GetComponent<ObjectRevealer>();
        if (objectRevealer != null)
        {
            objectRevealer.ToggleReveal(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ObjectRevealer objectRevealer = other.GetComponent<ObjectRevealer>();
        if (objectRevealer != null)
        {
            objectRevealer.ToggleReveal(false);
        }
    }
}
