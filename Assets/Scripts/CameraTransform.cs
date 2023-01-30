using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTransform : GameBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _vcam;
    [HideInInspector]
    public Cinemachine3rdPersonFollow CamFollow;

    [Range(1, 10)]
    public float Sensitivity;

    private float _sensitivityDamp = 6;

    private float _cameraDistance;
    private const float _threshold = 0.01f;
    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;
    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    private void Awake()
    {
        CamFollow = _vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    private void Start()
    {
        Sensitivity = 4;
        _cinemachineTargetYaw = gameObject.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        CameraRotation();
    }
    public void SetCameraTarget(Transform target)
    {
        _vcam.Follow = target;
        _vcam.LookAt = target;
    }

    public void LookAtPlayer()
    {
        _vcam.Follow = gameObject.transform;
        _vcam.LookAt = gameObject.transform;
    }



    private void CameraRotation()
    {

        if (gameObject == null) return;
        // if there is an input and camera position is not fixed
        if (IM.cameraInput.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = 0.1f;

            _cinemachineTargetYaw += IM.cameraInput.x * deltaTimeMultiplier / _sensitivityDamp * Sensitivity;
            _cinemachineTargetPitch += -IM.cameraInput.y * deltaTimeMultiplier / _sensitivityDamp * Sensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        gameObject.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    public void ChangeSensitivity(float sensitivity)
    {
        Sensitivity = sensitivity;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
