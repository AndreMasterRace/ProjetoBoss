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
    private Vector3 _direction; //DIRECAO QUE VOU ANDAR
    private Vector3 _oldPosition;

    private Quaternion _desiredRot;
    //O FOWARD E RIGHT DO PLAYER
    private Vector3 _forward;
    private Vector3 _right;
    private float _degrees;

    // Use this for initialization
    void Start()
    {
        _degrees = 5;
        _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        _angleYsum = 0;
        _direction = new Vector3(0.0f, 0.0f, 0.0f);
        _forward = Vector3.forward;
        _right = Vector3.right;
        //cameraRotationTarget = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //VECTOR3.FWD E VECTOR3.RIGHT EM VEZ DE TRANSFORM.FWD E ETC..
        //transform.position += Vector3.forward * Input.GetAxis("Vertical") * MovementSpeed;
        //transform.position += Vector3.right * Input.GetAxis("Horizontal") * MovementSpeed;
        //Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //transform.position += movement *MovementSpeed;
        //if (_forward.x < 0)
        //{
        //    transform.position += -_right * Input.GetAxis("Horizontal") * MovementSpeed;
        //}
        //else if (_forward.x > 0)
        //{
        //    transform.position += _right * Input.GetAxis("Horizontal") * MovementSpeed;
        //}

        transform.position += transform.forward * Input.GetAxis("Vertical") * MovementSpeed;

        if (Input.GetAxis("Horizontal")>0)
        {
            _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y+ _degrees, 0);

        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y - _degrees, 0);
        }

        if(Input.GetKeyDown(KeyBindings.MoveBackwards))
        {
            _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);

            transform.rotation = _desiredRot;

        }

        ////transform
        //transform.position += _forward * Input.GetAxis("Vertical") * MovementSpeed;
        ////transform.position += _right * Input.GetAxis("Horizontal") * MovementSpeed;
        ////
        //print(Input.GetAxis("Horizontal"));
        ////
        //if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        //{
        //    _direction.x = Input.GetAxis("Horizontal");
        //    _direction.z = Input.GetAxis("Vertical");
        //    _forward = _direction.normalized;
        //    if (_forward.x != 0 && _forward.z != 0)
        //    {
        //        if (_forward.x < 0 && _forward.z < 0)
        //            _right = new Vector3(_forward.x, 0, -_forward.z);
        //        if (_forward.x < 0 && _forward.z > 0)
        //            _right = new Vector3(-_forward.x, 0, _forward.z);
        //        if (_forward.x > 0 && _forward.z > 0)
        //            _right = new Vector3(_forward.x, 0, -_forward.z);
        //        if (_forward.x > 0 && _forward.z < 0)
        //            _right = new Vector3(-_forward.x, 0, _forward.z);
        //    }
        //    else
        //    {
        //        if (_forward.x != 0)
        //            _right = new Vector3(0.0f, 0.0f, -_forward.x);
        //        if (_forward.z != 0)
        //            _right = new Vector3(_forward.z, 0.0f, 0.0f);
        //    }

        //}

         

            //if (_oldPosition != transform.position)
            //{
            //    transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
            //}

            // cameraRotationTarget *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * SensitivityPitch, Vector3.right);
            _oldPosition = transform.position;

        if (Input.GetMouseButtonDown((int)KeyBindings.BasicAttackKey))
        {
            //SwordAnimator.SetBool("isAttackingb", true);
            SwordAnimator.SetTrigger("isAttacking");
        }

        if (Input.GetKey(KeyBindings.FreeCameraKey))
        {
            //LIMITES DA CAMERA NO EIXO Y
            _angleY = Input.GetAxis("Mouse Y") * SensitivityPitch;
            _angleYsum += _angleY;

            if (_angleYsum > 10)
            {
                _angleY = 0;
                _angleYsum = 10;
            }
            if (_angleYsum < -30)
            {
                _angleY = 0;
                _angleYsum = -30;
            }
            //
            //LIMITES DA CAMERA NO EIXO X
            _angleX = Input.GetAxis("Mouse X") * SensitivityYaw;
            _angleXsum += _angleX;
            //print(_angleXsum);
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

            //_angleY = Input.GetAxis("Mouse Y") * SensitivityPitch;
            //_angleYsum += _angleY;

            //_angleX = Input.GetAxis("Mouse X") * SensitivityPitch;
            //_angleXsum += _angleX;

            //CORPO QUE A CAMERA SEGUE FAZ AS SEGUINTES ROTACOES 
            CameraController.transform.rotation *= Quaternion.AngleAxis(_angleX, Vector3.up);
            CameraController.transform.rotation *= Quaternion.AngleAxis(_angleY, Vector3.left);
            //CameraController.transform.rotation *= Quaternion.AngleAxis(0, Vector3.back);
            //

            //var cameraTarget = transform.position /*transform.rotation*/ + CameraOffset;
            //PARA ONDE A CAMERA PRECISA DE TRANSLADAR

        }

        if (Input.GetKeyUp(KeyBindings.FreeCameraKey))
        {
            _angleXsum = 0;
            _angleYsum = 0;
            CameraController.transform.rotation = Quaternion.Euler(Vector3.zero);
        }




        CameraController.transform.position = transform.position;

        //var cameraTarget = CameraController.transform.position + CameraController.transform.rotation * CameraOffset;
        var cameraTarget = CameraController.transform.position + transform.rotation * CameraOffset;
        //
        //FAZ A CAMERA PASSAR DA POSICAO QUE ESTA PARA A POSICAO DESEJADA (cameraTarget)
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, CameraFollowDamping * Time.deltaTime);
        //Camera.main.transform.position = cameraTarget;
        //

        //        lookAt = (CameraController.transform.position + CameraController.transform.rotation * TargetOffset - Camera.main.transform.position).normalized;

        //lookAt = (/*transform.position + */transform.rotation * TargetOffset - Camera.main.transform.position).normalized;

        Vector3 eulerVector = new Vector3(CameraController.transform.rotation.x, CameraController.transform.rotation.y, 0);

        lookAt = _forward;

        //lookAt = transform.forward;
        //lookAt = transform.rotation * CameraFollowDamping;
        //float fuckingAngle;
        //fuckingAngle = Quaternion.Angle(CameraController.transform.rotation, CameraController.transform.rotation * Quaternion.AngleAxis(_angleX, Vector3.up));

        cameraRotationTarget = Quaternion.LookRotation(lookAt);
        // cameraRotationTarget = Quaternion.Euler(0, (transform.rotation.y / 2), 0);

        //cameraRotationTarget = Quaternion.EulerRotation(eulerVector);

        //Camera.main.transform.rotation =
        //    Quaternion.Lerp(Camera.main.transform.rotation, cameraRotationTarget, CameraRotationDamping * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, 10 * Time.fixedDeltaTime);
        Camera.main.transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, 10 * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
    }
}
