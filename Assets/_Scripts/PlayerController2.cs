using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController2 : MonoBehaviour
{
    ///CAMERA VARIABLES
    public Vector3 CameraOffset;
    //public GameObject CameraController;
    public float CameraRotationDamping;
    public float CameraFollowDamping;
    public float SensitivityYaw;
    public float SensitivityPitch;
    private float _angleY;
    private float _angleYsum;
    private float _angleX;
    private float _angleXsum;
    ///
    ///INPUT
    private float _moveHorizontal;
    private float _moveVertical;
    ///
    ///STATIC VARIABLES
    public static Transform Transform { get; set; }
    public static int MaxHealth { get; set; }
    ///
    ///COMPONENTS & OBJECTS
    private Animator _animator;
    private Rigidbody _rb;
    public Animator SwordAnimator;
    public GameObject PlayerRotation;
    private EnemyMinionBehaviour Focus;
    /// 
    ///VELOCIDADE QUANDO FREE CAMERA
    public float MovementSpeed;
    ///GRAUS QUE VIRO A CADA INPUT DE ESQUERDA/DIREITA EM FREE CAMERA
    public float TurnDegrees;
    ///VELOCIDADE PARA FRENTE/TRAS QUANDO LOCK ON
    public float SpeedWhenLockedOn;
    ///VELOCIDADE PARA ESQUERDA/DIREITA QUANDO LOCK ON
    public float RotationSpeed;


    public int Health;
    
    private Vector3 _oldPosition;

    private float _distanceToCenter;
    private bool _moveAllowed;

    
    ///PARA ONDE O PLAYER VAI OLHAR QUANDO ESTÁ LOCKED ON
    private Vector3 _centerOfFocus;
    ///

    public LockOnController LockOnController;

    private Quaternion _desiredRot;

    private float _degreesMove;
    private bool _lockedOn;
    [HideInInspector]
    public bool IsInteracting;
    ///QUAO PERTO E QUAO LONGE POSSO ESTAR DO INIMIGO EM LOCKON
    public float ProximityTreshold;
    public float RemotenessTreshold;
    ///

    //public GameObject Body;

    void Start()
    {
        Transform = transform;
        MaxHealth = Health;
        _moveHorizontal = 0;
        _moveVertical = 0;
        _moveAllowed = true;
        IsInteracting = false;
        _lockedOn = false;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        CombatGUIController.InCombat = false;
        ///PARA IMPEDIR ROTACOES MINIMAS PERSISTENTES
        _rb.sleepThreshold = 0.1f;
        _rb.freezeRotation = true;
        ///
        _degreesMove = 0;
        _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        _angleYsum = 0;
    }

    void Update()
    {
        if (_moveHorizontal != 0 || _moveVertical != 0)
        {
            GetComponent<Animator>().SetBool("isMoving", true);
        }
        else GetComponent<Animator>().SetBool("isMoving", false);
        if (_moveAllowed)
        {
            _moveHorizontal = Input.GetAxis("Horizontal");
            _moveVertical = Input.GetAxis("Vertical");
        }
        ///ATAQUE
        if (Input.GetMouseButtonDown((int)KeyBindings.BasicAttackKey))
        {
            SwordAnimator.SetTrigger("isAttacking");
        }
        ///VER SE O INIMIGO MORREU E SE MORREU JA NAO ESTOU LOCK ON
        if (Input.GetMouseButtonUp((int)KeyBindings.BasicAttackKey))
        {
            if (_lockedOn)
            {
                if (Focus.GetComponent<EnemyMinionBehaviour>().IsDead)
                {
                    _lockedOn = false;
                }
            }
        }
        ///
        ///INTERAGIR COM OBJETOS
        if (Input.GetKeyDown(KeyBindings.Interact))
        {
            StartCoroutine(Interact());
        }
        ///
        ///FAZER LOCK ON
        if (LockOnController.TheresEnemiesOnSight)
        {
            if (Input.GetKeyDown(KeyBindings.LockON))
            {
                print("Lock on");
                if (_lockedOn)
                {
                    _lockedOn = false;
                    CombatGUIController.InCombat = false;
                    ///FACO ISTO PARA "LIMPAR" O VALOR DA _desiredRot, IMPEDINDO O PLAYER DE FAZER UMA ROTACAO INICIAL DESNECESSARIA
                    _desiredRot = transform.rotation;
                    ///
                }
                else
                {
                    Focus = LockOnController.CheckCloser();
                    CombatGUIController.InCombat = true;
                    _lockedOn = true;
                    ///COLOCAR O ITEN QUE FAZ A ROTACAO DO PLAYER NO LOCAL DO INIMIGO
                    PlayerRotation.transform.position = Focus.transform.position;
                    ///COLOCAR O PLAYER DE VOLTA NO SEU LOCAL (POIS MEXER NO PlayeRotation VAI ALTERAR ESTA POSICAO)
                    transform.position = _oldPosition;
                    ///COLOCAR O PLAYER A OLHAR PARA O INIMIGO
                    transform.LookAt(_centerOfFocus);
                    ///FACO ISTO PARA "LIMPAR" O VALOR DA _desiredRot, IMPEDINDO O PLAYER DE FAZER UMA ROTACAO INICIAL DESNECESSARIA
                    _desiredRot = PlayerRotation.transform.rotation;
                    ///
                }
            }
        }
        ///

        if (!_lockedOn)
        {
            #region FREE MOVE
            ///MOVER PLAYER PARA FRENTE
            transform.position += transform.forward * _moveVertical* MovementSpeed;
            ///
            ///RODAR PLAYER PARA ESQUERDA/DIREITA
            if (_moveHorizontal > 0)
            {
                _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y + TurnDegrees, 0);

            }
            if (_moveHorizontal < 0)
            {
                _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y - TurnDegrees, 0);
            }
            ///
            ///VIRAR PLAYER PARA TRAS
            if (Input.GetKeyDown(KeyBindings.MoveBackwards))
            {
                _desiredRot = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
                transform.rotation = _desiredRot;
            }
            ///
            ///APLICAR A ROTACAO ANTES ESPECIFICADA
            transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, CameraRotationDamping * Time.deltaTime);
            ///
            Camera.main.transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, CameraRotationDamping * Time.deltaTime);

          

            var cameraTarget = transform.position + transform.rotation * CameraOffset;

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, CameraFollowDamping * Time.deltaTime);
            #endregion
        }

        if (_lockedOn)
        {
            ///OBTER DISTANCIA ENTRE O PLAYER E O INIMIGO
            GetDistance();
            ///

            _centerOfFocus = Focus.transform.position;
            //print(_distanceToCenter);
            if (_moveAllowed)
            {
                if (Input.GetKeyDown(KeyBindings.Dash))
                {

                    if (_moveHorizontal < 0)
                    {
                        _degreesMove = 60;

                        _degreesMove = (RemotenessTreshold * _degreesMove) / _distanceToCenter;
                        //print(_degreesMove);
                    }

                    else if (_moveHorizontal > 0)
                    {
                        _degreesMove = -60;
                        _degreesMove = (RemotenessTreshold * _degreesMove) / _distanceToCenter;
                        //print(DegreesMove);
                    }

                    StartCoroutine(Dash());
                }
                else
                {
                    if (Input.GetKey(KeyBindings.MoveLeft))
                    {
                        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y + TurnDegrees, 0);
                    }
                    if (Input.GetKeyUp(KeyBindings.MoveLeft))
                    {
                        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);

                    }

                    if (Input.GetKey(KeyBindings.MoveRight))
                    {
                        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y - TurnDegrees, 0);

                    }
                    if (Input.GetKeyUp(KeyBindings.MoveRight))
                    {
                        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);

                    }
                    ///NO RIGIDODY O INTERPOLATE TEM DE ESTAR LIGADO
                    if (Input.GetKey(KeyBindings.MoveForward))
                    {
                        if (_distanceToCenter > ProximityTreshold)
                        {
                            if (Vector3.Magnitude(transform.position + transform.forward * Time.deltaTime * SpeedWhenLockedOn)>ProximityTreshold)
                            {

                                transform.position += transform.forward * Time.deltaTime * SpeedWhenLockedOn;
                            }
                        }
                    
                    }
                    if (Input.GetKey(KeyBindings.MoveBackwards))
                    {
                        if (_distanceToCenter < RemotenessTreshold)
                        {
                            transform.position -= transform.forward * Time.deltaTime * SpeedWhenLockedOn;
                        }

                    }
                    ///
                }


            }
            PlayerRotation.transform.rotation = Quaternion.Lerp(PlayerRotation.transform.rotation, _desiredRot, RotationSpeed * Time.fixedDeltaTime);


            transform.LookAt(Focus.transform.position);

            Camera.main.transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, RotationSpeed * Time.fixedDeltaTime);
            var cameraTarget = transform.position + transform.rotation * CameraOffset;
            Camera.main.transform.position = cameraTarget;
            Camera.main.transform.LookAt(Focus.transform.position);
        }

        ///ATUALIZAR A TRANSFORM ESTATICA
        Transform = transform;
        ///
        ///MANTER UM REGISTO DA ULTIMA POSICAO 
        _oldPosition = transform.position;
        ///
    }
    ///OBTER DISTANCIA ENTRE O PLAYER E O INIMIGO
    public void GetDistance()
    {
        _distanceToCenter = Vector3.Distance(transform.position, Focus.transform.position);
    }
    ///
    ///CORROTINA QUE CONTROLA A DASH DO PLAYER
    public IEnumerator Dash()
    {
        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y + _degreesMove, 0);
        _moveAllowed = false;

        yield return new WaitForSeconds(0.6f);
        _moveAllowed = true;
        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);
        yield return null;
    }
    ///
    ///CORROTINA QUE CONTROLA A JANELA DE TEMPO EM QUE O OUTPUT DE INTERACAO ESTÁ ATIVADO
    public IEnumerator Interact()
    {
        IsInteracting = true;

        yield return new WaitForSeconds(0.5f);

        IsInteracting = false;

        yield return null;
    }
    ///

}