using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraPoint : MonoBehaviour
{
    public Transform target; //update whoever's on right side at run time
    public float smooth;

    private void LateUpdate()
    {
        Quaternion targetAngle = Quaternion.Euler(new Vector3(0, target.localEulerAngles.y + 90, 0));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, smooth * Time.deltaTime);
    }
}
