using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Gate : MonoBehaviour
{
    [SerializeField] ZoneBlocker zoneBlocker;
    [SerializeField] UnityEvent onOpened;
    [SerializeField] UnityEvent onClosed;

    public void Open()
    {
        zoneBlocker.Open();
        onOpened.Invoke();
    }

    public void Close()
    {
        zoneBlocker.Close();
        onClosed.Invoke();
    }
}
