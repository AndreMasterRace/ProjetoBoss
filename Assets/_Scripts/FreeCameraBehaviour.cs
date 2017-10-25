using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCameraBehaviour : MonoBehaviour
{
    public Vector3 CameraOffset;
    public Vector3 TargetOffset;
    public float SensitivityYaw;
    public float SensitivityPitch;
    public float MovementSpeed;

    public float CameraRotationDamping;
    public float CameraFollowDamping;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Input.GetAxis("Vertical") * MovementSpeed;
        transform.position += transform.right * Input.GetAxis("Horizontal") * MovementSpeed;
        transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * SensitivityYaw, Vector3.up);

        //var cameraTarget = transform.position /*transform.rotation*/ + CameraOffset;
        var cameraTarget = transform.position + transform.rotation * CameraOffset;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, CameraFollowDamping * Time.deltaTime);
        var cameraRotationTarget = Quaternion.LookRotation(transform.position + transform.rotation * TargetOffset - Camera.main.transform.position);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraRotationTarget, CameraRotationDamping * Time.deltaTime);
    }
}
