using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraPoint : MonoBehaviour
{
    public Transform targetA; //right side. Update at run time
    public Transform targetB;
    public float smooth;

    private void LateUpdate()
    {
        Quaternion targetRot = Quaternion.FromToRotation(Vector3.forward, (targetB.position - targetA.position)) * Quaternion.Euler(0f, 90f, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smooth * Time.deltaTime);
    }
}
