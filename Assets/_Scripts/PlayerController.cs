using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static Transform Transform { get; set; }
    public static int MaxHealth { get; set; }
    public GameObject Focus;
    public GameObject PlayerRotation;
    public int Health;
    private Animator _animator;
    public Animator SwordAnimator;
    public static bool Dead { get; set; }

    public float RotationSpeed;
    public float Speed;
    //private Transform _transform;
    private Quaternion _rot;
    private Rigidbody _rb;
    private Vector3 _centerOfFocus;
    public int Timer;
    public float Degrees;
    private float _degreesMove;
    private bool _moveAllowed;
    private int _totalDamageTaken; //ESTE VALOR TORNA-SE O OFFSET NA FUNCAO DECRESELIFE DO GAMEMANAGER
    private float _distanceToCenter;
    private float xx;
    private float zz;

    Quaternion _diseredRot;


    private void Start()
    {
        Dead = false;
        MaxHealth = Health;
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody>();
        _rb.sleepThreshold = 0.1f;
        _totalDamageTaken = 0;
        _centerOfFocus = Focus.transform.position;
        _degreesMove = 0;
        _moveAllowed = true;
        _diseredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y + _degreesMove, 0);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        GameManager.LifeBarDecrease(damage, _totalDamageTaken);
        //ISTO SEMPRE DEPOIS DE FAZER LIFEBARDECREASE()
        _totalDamageTaken += damage;
        print(Health);
        if(Health<=0)
        {
            Die();
        }
    }

    public void Die()
    {
        _animator.SetBool("isDead", true);
        GameManager.GameOverScreen();
        _moveAllowed = false;
    }

    public IEnumerator RotateAround()
    {
        float timer = 0.0f;

        _diseredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y + _degreesMove, 0);
        _moveAllowed = false;

        yield return new WaitForSeconds(0.6f);
        _moveAllowed = true;
        _diseredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);
        yield return null;
    }

    public void GetDistance()
    {
        xx = transform.position.x > 0 ? transform.position.x : -transform.position.x;
        zz = transform.position.z > 0 ? transform.position.z : -transform.position.z;
        _distanceToCenter = Mathf.Sqrt(xx * xx + zz * zz);
    }

    private void FixedUpdate()
    {
        _centerOfFocus = Focus.transform.position;
        transform.LookAt(_centerOfFocus); //OLHAR SEMPRE PARA O CENTRO
        //APLICAR A ROTACAO DESEJADA AO CORPO PLAYEROTATION QUE POR SUA VEZ APLICA ROTACAO NO PLAYER EM TORNO DO INIMIGO
        PlayerRotation.transform.rotation = Quaternion.Lerp(PlayerRotation.transform.rotation, _diseredRot, RotationSpeed * Time.fixedDeltaTime);
        
    }

    private void Update()
    {
        GetDistance();
        //SwordAnimator.SetBool("isAttackingb", false);
        //float yy = 0;
        if (_moveAllowed)
        {
            //float xx;
            //float zz;
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            if (Input.GetMouseButtonDown(0))
            {
                //SwordAnimator.SetBool("isAttackingb", true);
                SwordAnimator.SetTrigger("isAttacking");
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

                if (moveHorizontal < 0)
                {
                    _degreesMove = 60;

                    _degreesMove = (9 * _degreesMove) / _distanceToCenter;
                   // print(_degreesMove);
                }

                else if (moveHorizontal > 0)
                {
                    _degreesMove = -60;
                    _degreesMove = (9 * _degreesMove) / _distanceToCenter;
                    //print(_degreesMove);
                }

                StartCoroutine(RotateAround());
            }

            else
            {
                if (Input.GetKey(KeyCode.A))
                {
                    _diseredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y + Degrees, 0);
                    //print("A");
                }
                if (Input.GetKeyUp(KeyCode.A))
                {
                    _diseredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);

                }

                if (Input.GetKey(KeyCode.D))
                {
                    _diseredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y - Degrees, 0);
                    //print("D");
                }
                if (Input.GetKeyUp(KeyCode.D))
                {
                    _diseredRot = Quaternion.Euler(0, PlayerRotation.transform.eulerAngles.y, 0);

                }

                if (Input.GetKey(KeyCode.W))
                {
                    if (_distanceToCenter >= 6)
                        _rb.MovePosition(transform.position + transform.forward * Time.deltaTime * Speed);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    if (_distanceToCenter <= 9)
                        _rb.MovePosition(transform.position - transform.forward * Time.deltaTime * Speed);

                }
            }
        }
        // transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        //print(Time.fixedDeltaTime);
        Transform = transform; 
    }
}
