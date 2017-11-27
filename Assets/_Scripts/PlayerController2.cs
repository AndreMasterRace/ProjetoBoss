using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController2 : MonoBehaviour
{
    ///CAMERA VARIABLES
    public Vector3 CameraOffset;
    public float CameraRotationDamping;
    public float CameraFollowDamping;
    public float SensitivityYaw;
    public float SensitivityPitch;
    private float _angleY;
    private float _angleYsum;
    private float _angleX;
    private float _angleXsum;
    ///PARA ONDE A CAMERA VAI OLHAR, EM LOCK ON
    private Vector3 _cameraFocus;
    ///
    ///INPUT
    private float _moveHorizontal;
    private float _moveVertical;
    ///
    ///STATIC VARIABLES
    public static Transform Transform { get; set; }
    //public static Transform HandTransform { get; set; }
    public static int MaxHealth { get; set; }
    ///
    ///COMPONENTS & OBJECTS
    private Animator _animator;
    private Rigidbody _rb;
    public GameObject PlayerRotation;
    private Collider Focus;
    /// 
    ///VELOCIDADE QUANDO FREE CAMERA
    public float MovementSpeed;
    ///GRAUS QUE VIRO A CADA INPUT DE ESQUERDA/DIREITA EM FREE CAMERA
    public float TurnDegrees;
    ///VELOCIDADE PARA FRENTE/TRAS QUANDO LOCK ON
    public float SpeedWhenLockedOn;
    ///VELOCIDADE PARA ESQUERDA/DIREITA QUANDO LOCK ON
    public float RotationSpeed;
    ///O DANO TOTAL QUE LEVOU
    private int _totalDamageTaken;
    ///
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

    public GameObject Body;
    public WeaponBehaviour Weapon;
    //public GameObject Hand;

    private bool _canAttack;
    private float _timer;

    void Start()
    {
        _timer = 0;
        //HandTransform = Hand.transform;
        Transform = transform;
        MaxHealth = Health;
        _moveHorizontal = 0;
        _moveVertical = 0;
        _moveAllowed = true;
        _canAttack = true;
        IsInteracting = false;
        _lockedOn = false;
        _totalDamageTaken = 0;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        CombatGUIController.InCombat = false;
        PlayerEnabler.PlayerController2 = this;
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
       // print(_moveVertical);
        ///QUANDO FOR PERMITIDO MEXER O PLAYER
        if (_moveAllowed)
        {
            _moveHorizontal = Input.GetAxis("Horizontal");
            _moveVertical = Input.GetAxis("Vertical");
            if (_moveVertical != 0 || _moveHorizontal != 0)
            {
                _animator.SetBool("isMoving", true);
            }
            else
            {
                _animator.SetBool("isMoving", false);
            }
            ///ATAQUE BASICO
            if (Input.GetMouseButtonDown(KeyBindings.BasicAttackKey))
            {
                StartCoroutine(Attack(0));
            }
            ///
            ///ATAQUE PODEROSO
            if (Input.GetMouseButtonDown(KeyBindings.PowerAttackKey))
            {
                StartCoroutine(Attack(1));
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
                        LockOut();
                    }
                    else
                    {
                        LockOn();
                    }
                }
            }
            ///
        }

        ///
        ///DAR A INFORMACAO DO MEU MOVIMENTO AO ANIMATOR PARA ELE MOSTRAR A ANIMACAO CORRESPONDENTE
        _animator.SetFloat("MoveVertical", _moveVertical);
        ///
        ///VER SE O INIMIGO MORREU E SE MORREU JA NAO ESTOU LOCK ON
  
        if (!_lockedOn)
        {
            #region FREE MOVE
            ///MOVER PLAYER PARA FRENTE
            transform.position += transform.forward * _moveVertical * MovementSpeed;
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
            _cameraFocus = _centerOfFocus;
            _centerOfFocus.y -= Focus.GetComponent<NPCStats>().Height;
            //print(_distanceToCenter);
            if (_moveAllowed)
            {
                if (Input.GetKeyDown(KeyBindings.Dash))
                {
                    _animator.SetTrigger("isDashing");
                    if (_moveHorizontal < 0)
                    {
                        _degreesMove = 60;
                        _degreesMove = (RemotenessTreshold * _degreesMove) / _distanceToCenter;
                        _moveAllowed = false;
                    }
                    else if (_moveHorizontal > 0)
                    {
                        _degreesMove = -60;
                        _degreesMove = (RemotenessTreshold * _degreesMove) / _distanceToCenter;
                        _moveAllowed = false;
                    }
                    else if (_moveVertical > 0)
                    {
                        _moveAllowed = false;
                    }
                }
                else
                {
                    ///MOVER ESQUERDA/DIREITA EM CIRCULO DE VOLTA DO INIMIGO, QUANDO LEVANTO O BUTAO COLOCO A _desiretRot EM DEFAULT
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
                    ///
                    ///MOVER FRENTE E TRAS, NO RIGIDODY O INTERPOLATE TEM DE ESTAR LIGADO
                    if (Input.GetKey(KeyBindings.MoveForward))
                    {
                        if (_distanceToCenter > ProximityTreshold)
                        {
                            if (Vector3.Magnitude(transform.position + transform.forward * Time.deltaTime * SpeedWhenLockedOn) > ProximityTreshold)
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

                _animator.SetFloat("MoveSideways", _moveHorizontal);
            }
            PlayerRotation.transform.rotation = Quaternion.Lerp(PlayerRotation.transform.rotation, _desiredRot, RotationSpeed * Time.fixedDeltaTime);


            transform.LookAt(_centerOfFocus);

            Camera.main.transform.rotation = Quaternion.Lerp(transform.rotation, _desiredRot, RotationSpeed * Time.fixedDeltaTime);
            var cameraTarget = transform.position + transform.rotation * CameraOffset;
            Camera.main.transform.position = cameraTarget;
            Camera.main.transform.LookAt(_cameraFocus);
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
        _distanceToCenter = Vector3.Distance(transform.position, _centerOfFocus);
    }
    ///
    ///CORROTINA QUE CONTROLA A DASH DO PLAYER (LADOS)
    public IEnumerator DashSideways()
    {
        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y + _degreesMove, 0);


        yield return new WaitForSeconds(0.6f);
        _moveAllowed = true;
        _desiredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);
        yield return null;
    }
    ///
    /// ///CORROTINA QUE CONTROLA A DASH DO PLAYER (FRENTE/TRAS)
    public IEnumerator DashFwdBack()
    {
        if (_distanceToCenter > ProximityTreshold)
        {
            while (_timer < 0.6)
            {
                if(_distanceToCenter < ProximityTreshold+0.1)
                {
                    transform.position += transform.forward * Time.deltaTime * SpeedWhenLockedOn * 2;
                }
                _timer += 0.02f;
               yield return new WaitForSeconds(0.02f);
            }
        }
        //yield return new WaitForSeconds(0.6f);
        _moveAllowed = true;
        
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
    ///CORROTINA QUE CONTROLA O PRIMEIRO ATAQUE IMPLEMENTADO, IMPORTANTE TIRAR O PISCO DE CAN TRANSITION TO SELF NO ANIMATOR.
    ///ACEITA TIPO DE ATAQUE COMO PARAMETRO
    public IEnumerator Attack(int attackType)
    {
        ///DURACAO DO ATAQUE
        float animDuration = 0;
        ///
        switch (attackType)
        {
            case 0: _animator.SetTrigger("BasicAttackTrigger");
                animDuration = 0.864f; //1.13f;
                break;
            case 1:
                _animator.SetTrigger("PowerAttackTrigger");
                animDuration = 1.20f;
                break;
            default:
                break;
        }
        _moveAllowed = false;
        

        print(_moveAllowed);
       // yield return new WaitForEndOfFrame();
        StopMovement();

        //while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        //{
        //    yield return null;
        //}
        yield return new WaitForSeconds(animDuration);
        _moveAllowed = true;
        print(_moveAllowed);
        yield return null;
    }
    ///
    ///PARAR O MOVIMENTO DO PLAYER
    public void StopMovement()
    {
        _moveHorizontal = 0;
        _moveVertical = 0;
    }
    ///
    public void EnableTrigger()
    {
        Weapon.EnableWeaponCollider();
    }
    public void DisableTrigger()
    {
        Weapon.DisableWeaponCollider();
        try
        {
            if (Focus.GetComponent<NPCStats>().IsDead)
            {
                LockOut();
            }
        }
        catch
        {
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        GameManager.LifeBarDecrease(damage, _totalDamageTaken);
        //ISTO SEMPRE DEPOIS DE FAZER LIFEBARDECREASE()
        _totalDamageTaken += damage;
        print(Health);
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _animator.SetBool("isDead", true);
        StopMovement();
        GameManager.GameOverScreen();
        _moveAllowed = false;
    }
    public void LockOn()
    {
        Focus = LockOnController.CheckCloser();
        CombatGUIController.InCombat = true;
        _lockedOn = true;
        _animator.SetBool("LockedOn", _lockedOn);
        _centerOfFocus = Focus.transform.position;
        ///COLOCAR O ITEN QUE FAZ A ROTACAO DO PLAYER NO LOCAL DO INIMIGO
        PlayerRotation.transform.position = _centerOfFocus;
        ///COLOCAR O PLAYER DE VOLTA NO SEU LOCAL (POIS MEXER NO PlayeRotation VAI ALTERAR ESTA POSICAO)
        transform.position = _oldPosition;
        ///COLOCAR O PLAYER A OLHAR PARA O INIMIGO
        transform.LookAt(_centerOfFocus);
        ///FACO ISTO PARA "LIMPAR" O VALOR DA _desiredRot, IMPEDINDO O PLAYER DE FAZER UMA ROTACAO INICIAL DESNECESSARIA
        _desiredRot = PlayerRotation.transform.rotation;
        ///
    }
    public void LockOut()
    {
        _lockedOn = false;
        _animator.SetBool("LockedOn", _lockedOn);
        CombatGUIController.InCombat = false;
        ///FACO ISTO PARA "LIMPAR" O VALOR DA _desiredRot, IMPEDINDO O PLAYER DE FAZER UMA ROTACAO INICIAL DESNECESSARIA
        _desiredRot = transform.rotation;
        ///
        ///TORNAR O VALOR DE MoveSideways = 0 PARA NAO FAZER STRAFE QUANDO TIRO LOCK ON
        _animator.SetFloat("MoveSideways", 0.0f);
        ///
    }

}