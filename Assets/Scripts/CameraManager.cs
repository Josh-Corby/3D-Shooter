using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera povCam;
    [SerializeField] private CinemachineVirtualCamera thirdPersonCam;
    [SerializeField] private Camera mainCamera;

    //private void Update()
    //{
    //    Debug.Log(Vector3.Distance(mainCamera.transform.position, thirdPersonCam.LookAt.gameObject.transform.position));
    //}
}
