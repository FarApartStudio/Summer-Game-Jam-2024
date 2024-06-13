using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera followVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private Transform _target;

    Coroutine fovCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void SetDistanceToTarget(float newValue)
    {
        CinemachineComponentBase componentBase = followVirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is Cinemachine3rdPersonFollow cinemachine3RdPersonFollow)
        {
            cinemachine3RdPersonFollow.CameraDistance = newValue;
        }
    }

    public void FollowAnLootAt(Transform target)
    {
        _target = target;
        followVirtualCamera.Follow = _target;
        aimVirtualCamera.LookAt = _target;
    }

    public void Follow(Transform target)
    {
        _target = target;
        followVirtualCamera.Follow = _target;
        aimVirtualCamera.Follow = _target;
    }

    public void LookAt(Transform target)
    {
        _target = target;
        followVirtualCamera.LookAt = _target;
        aimVirtualCamera.LookAt = _target;
    }

    public void ChangeCameraDirection(CameraDirection direction)
    {
        Change3rdPersonCameraSide(followVirtualCamera, direction == CameraDirection.Left ? 1 : 0);
        Change3rdPersonCameraSide(aimVirtualCamera, direction == CameraDirection.Left ? 1 : 0);
    }

    private void Change3rdPersonCameraSide(CinemachineVirtualCamera virtualCamera, float newSide)
    {
        Cinemachine3rdPersonFollow cinemachine3RdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        cinemachine3RdPersonFollow.CameraSide = newSide;
    }

    public void ChangeFollowFOV(float newFOV, float duration = 0.5f)
    {
        if (fovCoroutine != null) StopCoroutine(fovCoroutine);
        fovCoroutine = StartCoroutine(ChangeFOVSmoothlyCoroutine(followVirtualCamera, newFOV, duration));
    }

    private IEnumerator ChangeFOVSmoothlyCoroutine(CinemachineVirtualCamera virtualCamera, float newFOV, float duration = 0.5f)
    {
        float initialFOV = virtualCamera.m_Lens.FieldOfView;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsedTime / duration);
            float currentFOV = Mathf.Lerp(initialFOV, newFOV, t);
            virtualCamera.m_Lens.FieldOfView = currentFOV;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final FOV is set exactly
        virtualCamera.m_Lens.FieldOfView = newFOV;
    }
}