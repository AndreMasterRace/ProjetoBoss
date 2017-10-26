using System;
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

    public GameObject CameraController;
    public Animator SwordAnimator;

    public float CameraRotationDamping;
    public float CameraFollowDamping;

    private Quaternion cameraRotationTarget;
    private Vector3 lookAt; //My Fwd vector
    private float _angleY;
    private float _angleYsum;
    private float _angleX;
    private float _angleXsum;
    private Vector3 _direction;


    // Use this for initialization
    void Start()
    {
        _angleYsum = 0;
        _direction = new Vector3(0.0f, 0.0f, 0.0f);
        //cameraRotationTarget = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Input.GetAxis("Vertical") * MovementSpeed;
        transform.position += transform.right * Input.GetAxis("Horizontal") * MovementSpeed;
        //transform.LookAt(GetComponent<Rigidbody>().velocity);
        //_direction.x = Input.GetAxis("Horizontal");
        //_direction.z = Input.GetAxis("Vertical");
     //   transform.rotation = Quaternion.LookRotation(_direction);
        // cameraRotationTarget *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * SensitivityPitch, Vector3.right);

        if (Input.GetMouseButtonDown(0))
        {
            //SwordAnimator.SetBool("isAttackingb", true);
            SwordAnimator.SetTrigger("isAttacking");
        }

        _angleY = Input.GetAxis("Mouse Y") * SensitivityPitch;
        _angleYsum += _angleY;
        //print(_angleYsum);
        if (_angleYsum > 30)
        {
          
            _angleY = 0;
            _angleYsum = 30;
        }
        if (_angleYsum < -30)
        {
            
            _angleY = 0;
            _angleYsum = -30;
        }
        _angleX = Input.GetAxis("Mouse X") * SensitivityYaw;
        _angleXsum += _angleX;
        print(_angleXsum);
        if (_angleXsum > 30)
        {
            print("over");
            _angleX = 0;
            _angleXsum = 30;
        }
        if (_angleXsum < -30)
        {
            print("under");
            _angleX = 0;
            _angleXsum = -30;
        }

        CameraController.transform.rotation *= Quaternion.AngleAxis(_angleX, Vector3.up);
        CameraController.transform.rotation *= Quaternion.AngleAxis(_angleY, Vector3.left);
        

        //var cameraTarget = transform.position /*transform.rotation*/ + CameraOffset;
        var cameraTarget = CameraController.transform.position + CameraController.transform.rotation * CameraOffset;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, CameraFollowDamping * Time.deltaTime);
        lookAt = (CameraController.transform.position + CameraController.transform.rotation * TargetOffset - Camera.main.transform.position).normalized;
        //print(lookAt);
        //if (lookAt.y >= 0.7f)
        //{
        //    print("dddd");
        //    lookAt.y = 0.69f;
        //}
        //if (lookAt.y <= -0.7f)
        //{
        //    print("eeee");
        //    lookAt.y = -0.69f;
        //}

        cameraRotationTarget = Quaternion.LookRotation(lookAt);

        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraRotationTarget, CameraRotationDamping * Time.deltaTime);
    }
}
