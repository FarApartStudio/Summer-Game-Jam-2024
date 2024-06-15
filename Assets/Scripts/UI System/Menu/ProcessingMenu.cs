using Pelumi.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessingMenu : GenericMenu<ProcessingMenu>
{
    protected override void OnCreated()
    {

    }

    protected override void OnOpened()
    {

    }

    protected override void OnClosed()
    {

    }

    protected override void OnDestoryInvoked()
    {

    }

    public void CloseWithDelay(float duration = 1f)
    {
        StartCoroutine(CloseWithDelayRoutine(duration));
    }

    private IEnumerator CloseWithDelayRoutine(float v)
    {
        yield return new WaitForSeconds(v);
        Close();
    }

    public override void ResetMenu()
    {
        throw new NotImplementedException();
    }
}
