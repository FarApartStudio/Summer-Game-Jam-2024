using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject CinemachineCameraTarget;

    [SerializeField] private bool invertVertical;
    [SerializeField] private bool invertHorizontal;

    [Range(0.01f, 100.0f)]
    [SerializeField] private float sensitivity = 0.2f;
    [SerializeField] private float TopClamp = 70.0f;
    [SerializeField] private float BottomClamp = -30.0f;
    [SerializeField] private float CameraAngleOverride = 0.0f;

    [SerializeField] private Vector2 moveInput;

    [SerializeField] private bool LockCameraPosition = false;
    [SerializeField] private bool IsCurrentDeviceMouse = true;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;

    private void Start()
    {
        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        TPSCameraRotation();
    }

    public void UpdateInput(Vector2 _moveInput)
    {
        moveInput = _moveInput;
    }

    private void TPSCameraRotation()
    {
        if (moveInput.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            //Horizontal
            cinemachineTargetYaw += moveInput.x * deltaTimeMultiplier * sensitivity * (invertHorizontal ? -1 : 1);
            //Vertical
            cinemachineTargetPitch += moveInput.y * deltaTimeMultiplier * sensitivity * (invertVertical ? 1 : -1);
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void AddRecoil(Vector2 recoil)
    {
        cinemachineTargetYaw += recoil.x;
        cinemachineTargetPitch += recoil.y;
    }
}
