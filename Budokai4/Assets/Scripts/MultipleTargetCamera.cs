using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MultipleTargetCamera : MonoBehaviour
{
    public List<Transform> targets;
    public Transform lookTarget;
    CinemachineVirtualCamera virtualCamera;
    CinemachineComponentBase componentBase;
    float distance;
    float cameraDistance;
    public float minZoom;
    public float maxZoom;
    public float zoomLimiter;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
    }

    private void Update()
    {
        distance = Vector3.Distance(targets[0].position, targets[1].position);
    }

    private void LateUpdate()
    {
        if(targets.Count == 0) { return; }

        lookTarget.position = Vector3.Lerp(targets[0].position, targets[1].position, 0.5f);
        Zoom();
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);
        (componentBase as CinemachineFramingTransposer).m_CameraDistance = newZoom;
    }
}
