using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractResponder : MonoBehaviour
{
    [SerializeField] private Collider interactObject;

    private Collider _col;

    private void Awake()
    {
        _col = GetComponent<Collider>();
    }

    public Collider GetInteractObject()
    {
        return interactObject;
    }


    public void ToggleActive(bool status)
    {
        _col.enabled = status;
    }
}
