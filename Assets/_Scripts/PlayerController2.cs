using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController2 : MonoBehaviour
{
    public Vector3 CameraOffset;
    public Vector3 TargetOffset;
    public float SensitivityYaw;
    public float SensitivityPitch;
    public float MovementSpeed;

    public GameObject CameraController;
    public Animator SwordAnimator;
    private Animator _animator;
    private Rigidbody _rb;
    public GameObject PlayerRotation;
    public float Speed;

    public GameObject Focus;

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
    private Vector3 offset;

    public float RotationSpeed;

    private float _distanceToCenter;
    private float xx;
    private float zz;

    public float Degrees;

    private Vector3 _centerOfFocus; //FOCUS QUANDO ESTÁ LOCKED ON

    private Quaternion _desiredRot;
    //O FOWARD E RIGHT DO PLAYER
    private Vector3 _forward;
    private Vector3 _right;
    private float _degrees;
    private bool _lockedOn;
    private bool _offseted;
    private Vector3 _target;
    // Use this for initialization
    void Start()
    {
        _offseted = false;
        _lockedOn = false;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _rb.sleepThreshold = 0.1f;
        _rb.freezeRotation = true;
        _degrees = 5;
        _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        _angleYsum = 0;
        _direction = new Vector3(0.0f, 0.0f, 0.0f);
        _forward = Vector3.forward;
        _right = Vector3.right;
        cameraRotationTarget = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        GetDistance();

        if (Input.GetMouseButtonDown((int)KeyBindings.BasicAttackKey))
        {
            SwordAnimator.SetTrigger("isAttacking");
        }

        if (_distanceToCenter > 6 && _distanceToCenter < 9)
        {
            if (Input.GetKeyDown(KeyBindings.LockON))
            {
                //COLOCAR O ITEN QUE FAZ A ROTACAO DO PLAYER NO LOCAL DO INIMIGO
                
                
                //PlayerRotation.transform.forward = transform.forward;
                //
                if (_lockedOn)
                {
                    _offseted = false;
                    _lockedOn = false;
                }
                else
                {
                    _lockedOn = true;
                    PlayerRotation.transform.position = Focus.transform.position;
                    transform.position = _oldPosition;
                    _desiredRot = PlayerRotation.transform.rotation;
                }
            }
        }

        if (!_lockedOn)
        {
            #region FREE MOVE

            transform.position += transform.forward * Input.GetAxis("Vertical") * MovementSpeed;


            if (Input.GetAxis("Horizontal") > 0)
            {
                _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y + _degrees, 0);

            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y - _degrees, 0);
            }

            if (Input.GetKeyDown(KeyBindings.MoveBackwards))
            {
                _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);

                transform.rotation = _desiredRot;

            }

            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, 10 * Time.deltaTime);
            Camera.main.transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, 10 * Time.deltaTime);

            CameraController.transform.position = transform.position;

            var cameraTarget = CameraController.transform.position + transform.rotation * CameraOffset;

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, CameraFollowDamping * Time.deltaTime);
            #endregion
        }

        if (_lockedOn)
        {
            _centerOfFocus = Focus.transform.position;

            if (Input.GetKey(KeyBindings.MoveLeft))
            {
                _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y + Degrees, 0);
                //print("A");
            }
            if (Input.GetKeyUp(KeyBindings.MoveLeft))
            {
                _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);

            }

            if (Input.GetKey(KeyBindings.MoveRight))
            {
                _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y - Degrees, 0);
                //print("D");
            }
            if (Input.GetKeyUp(KeyBindings.MoveRight))
            {
                _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);

            }
            //NO RIGIDODY O INTERPOLATE TEM DE ESTAR LIGADO
            if (Input.GetKey(KeyBindings.MoveForward))
            {
                if (_distanceToCenter >= 6)
                {
                    print("HERE");
                    // print(_distanceToCenter);
                    // print(transform.forward);
                    _rb.MovePosition(transform.position + transform.forward * Time.deltaTime * Speed);
                    // _target -= transform.forward.normalized;
                }

            }
            if (Input.GetKey(KeyBindings.MoveBackwards))
            {
                if (_distanceToCenter <= 9)
                {
                    print("THERE");
                    _rb.MovePosition(transform.position - transform.forward * Time.deltaTime * Speed);
                }

            }
            // Vector3 offset = new Vector3(0, 0, -8);
            transform.LookAt(_centerOfFocus);
            //if (!_offseted)
            //{
            //    offset = -(Focus.transform.position - transform.position);
            //    _offseted = true;
            //  //  _target = PlayerRotation.transform.position + PlayerRotation.transform.rotation * offset;

            //    //transform.position = Vector3.Lerp(transform.position, _target, CameraFollowDamping * Time.deltaTime);

            //    //_target = Focus.transform.position + Focus.transform.rotation * offset ;
            //}



            PlayerRotation.transform.rotation = Quaternion.Lerp(PlayerRotation.transform.rotation, _desiredRot, RotationSpeed * Time.fixedDeltaTime);


            transform.LookAt(Focus.transform.position);
            Camera.main.transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, RotationSpeed * Time.fixedDeltaTime);

            CameraController.transform.position = transform.position;

            var cameraTarget = CameraController.transform.position + transform.rotation * CameraOffset;

            Camera.main.transform.position = cameraTarget;

        }
        
      
        _oldPosition = transform.position;
    }

    public void GetDistance()
    {
        _distanceToCenter = Vector3.Distance(transform.position, Focus.transform.position);

    }

    private void FixedUpdate()
    {

        if (_lockedOn)
        {

          
            //var cameraTarget = CameraController.transform.position + transform.rotation * CameraOffset;
            //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, CameraFollowDamping * Time.fixedDeltaTime);
        }
    }
}