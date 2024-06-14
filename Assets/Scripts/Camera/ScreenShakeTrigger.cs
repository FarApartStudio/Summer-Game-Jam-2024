using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeTrigger : MonoBehaviour
{
    public void Shake(ScreenShakeTriggerSO screenShakeTriggerSO)
    {
        CameraManager.Instance.ShakeCamera(screenShakeTriggerSO.shape, screenShakeTriggerSO.duration, screenShakeTriggerSO.force, screenShakeTriggerSO.velocity);
    }
}
