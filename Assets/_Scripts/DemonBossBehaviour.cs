using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemonBossBehaviour : MonoBehaviour
{
    private int damage;
    public int Health;
    private int _maxHealth;
    private Animator _animator;
    public Text DamageText;
    public GameObject ObstacleRotater;
    public List<Transform> Obstacles;
    public GameObject Aura;
    private int _damageAggregate;
    ///NUMERO DE VEZES QUE USOU O ATAQUE 1
    private int _nAttack1;
    ///
    public int LungeAttackDamage;
    public int Attack1Treshold;
    public bool isMelee;
    private void Awake()
    {
        BossEnabler.DemonBossBehaviour = this;
        _nAttack1 = 0;
        _damageAggregate = 0;
        _maxHealth = Health;
        _animator = GetComponent<Animator>();

    }
    private void Start()
    {
        isMelee = false;
        for (int i = 1; i < 33; i++)
        {
            Obstacles.Add(ObstacleRotater.GetComponentsInChildren<Transform>()[i]);
        }
        if (BossEnabler.IsEnabled)
        {
            ///COMECA A SUSSECAO DE ATAQUES
            StartCoroutine(Idle());
            ///
        }
    }

    public IEnumerator Idle()
    {
        yield return new WaitForSeconds(1f);
        ChoseAttack();
        yield return null;
    }

    public void ChoseAttack()
    {
        int rand = Random.Range(1, 101);
        //Vector3 eyeLevelPlayerPosition = new Vector3(PlayerController2.Transform.position.x, 8, PlayerController2.Transform.position.z);
        //transform.LookAt(eyeLevelPlayerPosition);
        //transform.forward = PlayerController2.Transform.position;
        transform.LookAt(PlayerController2.Transform);
        if (Vector3.Distance(transform.position, PlayerController2.Transform.position) < 7.5f)
        {
            _animator.SetTrigger("isAttacking2");
            //if (rand >= 50)
            //{

            //    //if (Health > (_maxHealth / 2))
            //       // StartCoroutine(AuraAttack());
            //    //else StartCoroutine(CombinedAttack());
            //}
            //else
            //{
            //    StartCoroutine(LungeAttack());
            //}

        }
        else
        {
            _animator.SetTrigger("isAttacking1");
            //if (rand >= 50)
            //{
            //    if (Health > (_maxHealth / 2))
            //        StartCoroutine(PillarsAttack());
            //    else StartCoroutine(CombinedAttack());
            //}
            //else
            //{
            //    StartCoroutine(LungeAttack());
            //}

        }
    }

    ///Pillars Attack
    public IEnumerator Attack1()
    {
        //_animator.SetTrigger("isAttacking1");

        ///ISTO É BASICAMENTO O SENO E COSSENO, SE A CADA POSICAO EU PUSER offsetZ = AO SEN(angle) e o offsetX = AO COS(angle) TENHO A POSICAO



        Vector3 position2D = new Vector3(PlayerController2.Transform.position.x, 0.0f, PlayerController2.Transform.position.z);
        float angle = Mathf.Deg2Rad * Vector3.Angle(position2D.normalized, Vector3.right);
        // print(Mathf.Rad2Deg * angle);
        float offsetX;
        float offsetY;
        float offsetZ;
        float distance;
        int i = 0;

        ///POR OBSTACULOS NA POSICAO
        foreach (Transform obstacle in Obstacles)
        {
            obstacle.GetComponent<ParticleSystem>().Stop();
            //if (i % 2 == 0 && i != 0)
            //{
            //    
            //}

            distance = 0;
           // distance = (i % 2 == 0) ? 3f : 5.5f;
            switch (i)
            {
                case 0:
                    distance = 3.0f;
                    break;
                case 1:
                    distance = 5.5f;
                    break;
                case 2:
                    distance = 8.0f;
                    break;
                case 3:
                    distance = 10.5f;
                    break;
                default:
                    break;
            }
            offsetX = Mathf.Cos(angle) * distance;
            offsetY = 0;
            offsetZ = Mathf.Sin(angle) * distance;
            ///ISTO TEM QUE VER COM A FORMA COMO O UNITY CALCULA OS ANGULOS. O VALOR DESTES É SEMPRE POSITIVO
            ///iSSO FAZ COM QUE TENHA DE ARTIFICIALMENTE PASSAR O MEU offsetZ PARA NEGATIVO CASO O Z DA POSICAO SEJA NEGATIVO
            offsetZ = (position2D.z >= 0) ? offsetZ : -offsetZ;

            obstacle.position = transform.position + new Vector3(offsetX, offsetY, offsetZ);
            //obstacle.GetComponent<Animator>().SetTrigger("isActive");
            obstacle.GetComponent<ParticleSystem>().Play();

            i++;
            if (i > 3)
            {
                angle += Mathf.Deg2Rad * 45;
                i = 0;
            }

        }
        ///


        yield return null;

    }
    /// 

    //OBSTACLE SHOWER
    public IEnumerator PillarsAttack()
    {
        ObstacleRotater.transform.position = transform.position;
        StartCoroutine(Attack1());
        yield return new WaitForSeconds(0.5f);
        foreach (Transform obstacle in Obstacles)
        {
            obstacle.GetComponent<Collider>().enabled = true;
        }

        if (_nAttack1 >= Attack1Treshold)
        {
            float timer = 0;
            while (timer < 1)
            {
                foreach (Transform obstacle in Obstacles)
                {
                    Quaternion diseredRot = Quaternion.Euler(0, ObstacleRotater.transform.eulerAngles.y + 5, 0);
                    ObstacleRotater.transform.rotation = Quaternion.Lerp(ObstacleRotater.transform.rotation, diseredRot, 1 * Time.fixedDeltaTime);

                }

                yield return new WaitForFixedUpdate();
                timer += 0.02f;
            }
            yield return new WaitForSeconds(2.0f);
        }

        else
        {
            yield return new WaitForSeconds(3.0f);
        }
        ObstacleRotater.transform.position = transform.position + new Vector3(30, 30, 30);
        foreach (Transform obstacle in Obstacles)
        {
            obstacle.GetComponent<Collider>().enabled = false;
            obstacle.GetComponent<ParticleSystem>().Stop();
        }
        //foreach (GameObject obstacle in Obstacles)
        //{
        //    //obstacle.transform.position = transform.position + new Vector3(30, 30, 30);
        //}
        _nAttack1++;
        StartCoroutine(Idle());
        yield return null;
    }

    //AURA ATTACK
    public IEnumerator Attack2()
    {
        //_animator.SetTrigger("isAttacking2");

        //while (!_animator.IsInTransition(0))
        //{

        //    yield return null;
        //}

        Aura.transform.position = transform.position;
        Aura.GetComponent<Animator>().SetTrigger("isActive");

        yield return null;
    }

    //AURA ATTACK
    public IEnumerator AuraAttack()
    {
        StartCoroutine(Attack2());

        yield return new WaitForSeconds(3f);

        Aura.transform.position = transform.position + new Vector3(30, 30, 30);

        StartCoroutine(Idle());
        yield return null;
    }


    ///ATAQUE COMBINADO TANTO COM AURA COMO OS OBSTACULOS
    public IEnumerator CombinedAttack()
    {
        StartCoroutine(Attack2());
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Attack1());

        yield return new WaitForSeconds(0.5f);
        float timer = 0;
        while (timer < 1)
        {
            foreach (Transform obstacle in Obstacles)
            {
                Quaternion diseredRot = Quaternion.Euler(0, ObstacleRotater.transform.eulerAngles.y + 5, 0);
                ObstacleRotater.transform.rotation = Quaternion.Lerp(ObstacleRotater.transform.rotation, diseredRot, 1 * Time.fixedDeltaTime);

            }

            yield return new WaitForFixedUpdate();
            timer += 0.02f;
        }
        yield return new WaitForSeconds(2.0f);

        foreach (Transform obstacle in Obstacles)
        {
            obstacle.position = transform.position + new Vector3(30, 30, 30);
        }
        yield return new WaitForSeconds(1.0f);
        Aura.transform.position = transform.position + new Vector3(30, 30, 30);

        StartCoroutine(Idle());
        yield return null;
    }
    ///
    public IEnumerator LungeAttack()
    {
        transform.LookAt(PlayerController2.Transform.position);
       // _animator.SetTrigger("isAttacking3");

        yield return new WaitForSeconds(1.5f);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        StartCoroutine(Idle());
        yield return null;
    }

    public IEnumerator ShowDamage()
    {
        DamageText.text = damage.ToString();
        //yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(0.05f);
        _damageAggregate += damage;
        //print(_damageAggregate);
        DamageText.text = "";

        yield return null;
    }

    //LEVAR DANO
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            damage = other.GetComponent<WeaponBehaviour>().DamageCalc();
            Health -= damage;
            StartCoroutine(ShowDamage());
            if (Health <= 0)
                Die();
        }

        if (isMelee && other.tag == "Player")
        {
            damage = LungeAttackDamage;
            other.GetComponent<PlayerController2>().TakeDamage(damage);

        }
    }

    public void Die()
    {
        _animator.SetBool("isDead", true);
    }

    public void ActivateMelee()
    {
        isMelee = true;
    }
    public void DeactivateMelee()
    {
        isMelee = false;
    }
    //private void Update()
    //{

    //}

}

