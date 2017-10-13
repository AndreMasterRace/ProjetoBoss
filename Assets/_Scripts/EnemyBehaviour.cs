using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    private int damage;
    public int Health;
    private int _maxHealth;
    private Animator _animator;
    public Text DamageText;
    public List<GameObject> Obstacles;
    public GameObject Aura;
    private int _damageAggregate;
    private int _nAttack1; //NUMERO DE VEZES QUE USOU O ATAQUE 1
    public int LungeAttackDamage;
    public int Attack1Treshold;
    public GameObject ObstacleRotater;

    private void Start()
    {
        _nAttack1 = 0;
        _damageAggregate = 0;
        _maxHealth = Health;
        _animator = GetComponent<Animator>();
        StartCoroutine(Idle());
    }

    public IEnumerator Idle()
    {
        yield return new WaitForSeconds(1f);
        ChoseAttack();
        yield return null;
    }

    public void ChoseAttack()
    {
        float rand = Random.Range(1, 101);
        //print(Vector3.Distance(transform.position, PlayerController.Transform.position));
        if (Vector3.Distance(transform.position, PlayerController.Transform.position) < 7.5f)
        {
            if (Health > (_maxHealth / 2))
            {
                if (rand >= 50)
                {
                    StartCoroutine(AuraAttack());
                }
                else
                {
                    StartCoroutine(LungeAttack());
                }

            }
            else
            {
                //if (rand >= 75)
                //{
                //    StartCoroutine(AuraAttack());
                //}
                //else
                //{
                //    StartCoroutine(LungeAttack());
                //}

                StartCoroutine(CombinedAttack());
            }

        }
        else
        {

            if (Health > (_maxHealth / 2))
            {
                if (rand >= 50)
                {
                    StartCoroutine(PillarsAttack());
                }
                else
                {
                    StartCoroutine(LungeAttack());
                }
            }
            else
            {
                //if (rand >= 75)
                //{
                //    StartCoroutine(CombinedAttack());
                //}
                //else
                //{
                //    StartCoroutine(LungeAttack());
                //}
                StartCoroutine(CombinedAttack());
            }
        }
    }

    //Pillars Attack
    public IEnumerator Attack1()
    {
        _animator.SetTrigger("isAttacking1");

        //GREAT SOLUTION//
        //yield return new WaitForFixedUpdate();
        //yield return new WaitForEndOfFrame();<
        //--------------//
        while (!_animator.IsInTransition(0))
        {

            yield return null;
        }
        //while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        //{

        //    yield return null;
        //}

        //ISTO É BASICAMENTO O SENO E COSSENO, SE A CADA POSICAO EU PUSER offsetZ = AO SEN(angle) e o offsetX = AO COS(angle) TENHO A POSICAO

        Vector3 position2D = new Vector3(PlayerController.Transform.position.x, 0.0f, PlayerController.Transform.position.z);
        float angle = Mathf.Deg2Rad * Vector3.Angle(position2D.normalized, Vector3.right);
        // print(Mathf.Rad2Deg * angle);
        float offsetX;
        float offsetY;
        float offsetZ;
        float distance;
        int i = 0;

        //POR OBSTACULOS NA POSICAO
        foreach (GameObject obstacle in Obstacles)
        {
            if (i % 2 == 0 && i != 0)
            {
                angle += Mathf.Deg2Rad * 45;
            }
            distance = (i % 2 == 0) ? 6f : 8.5f;
            offsetX = Mathf.Cos(angle) * distance;
            offsetY = 0;
            offsetZ = Mathf.Sin(angle) * distance;
            //ISTO TEM QUE VER COM A FORMA COMO O UNITY CALCULA OS ANGULOS. O VALOR DESTES É SEMPRE POSITIVO
            //iSSO FAZ COM QUE TENHA DE ARTIFICIALMENTE PASSAR O MEU offsetZ PARA NEGATIVO CASO O Z DA POSICAO SEJA NEGATIVO
            offsetZ = (position2D.z >= 0) ? offsetZ : -offsetZ;

            obstacle.transform.position = transform.position + new Vector3(offsetX, offsetY, offsetZ);
            obstacle.GetComponent<Animator>().SetTrigger("isActive");

            i++;
        }

        yield return null;

    }

    //OBSTACLE SHOWER
    public IEnumerator PillarsAttack()
    {
        StartCoroutine(Attack1());
        if (_nAttack1 >= Attack1Treshold)
        {
            yield return new WaitForSeconds(0.5f);
            float timer = 0;
            while (timer < 1)
            {
                foreach (GameObject obstacle in Obstacles)
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
            yield return new WaitForSeconds(3.5f);
        }
        foreach (GameObject obstacle in Obstacles)
        {
            obstacle.transform.position = transform.position + new Vector3(30, 30, 30);
        }
        _nAttack1++;
        StartCoroutine(Idle());
        yield return null;
    }

    //AURA ATTACK
    public IEnumerator Attack2()
    {
        _animator.SetTrigger("isAttacking2");

        while (!_animator.IsInTransition(0))
        {

            yield return null;
        }

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

    public IEnumerator CombinedAttack()
    {
        StartCoroutine(Attack2());
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(Attack1());

        yield return new WaitForSeconds(0.5f);
        float timer = 0;
        while (timer < 1)
        {
            foreach (GameObject obstacle in Obstacles)
            {
                Quaternion diseredRot = Quaternion.Euler(0, ObstacleRotater.transform.eulerAngles.y + 5, 0);
                ObstacleRotater.transform.rotation = Quaternion.Lerp(ObstacleRotater.transform.rotation, diseredRot, 1 * Time.fixedDeltaTime);

            }

            yield return new WaitForFixedUpdate();
            timer += 0.02f;
        }
        yield return new WaitForSeconds(2.0f);

        foreach (GameObject obstacle in Obstacles)
        {
            obstacle.transform.position = transform.position + new Vector3(30, 30, 30);
        }
        yield return new WaitForSeconds(1.0f);
        Aura.transform.position = transform.position + new Vector3(30, 30, 30);

        StartCoroutine(Idle());
        yield return null;
    }

    public IEnumerator LungeAttack()
    {
        transform.LookAt(PlayerController.Transform.position);


        _animator.SetTrigger("isAttacking3");

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
                _animator.SetBool("isDead", true);
        }

        if (other.tag == "Player")
        {
            damage = LungeAttackDamage;
            other.GetComponent<PlayerController>().TakeDamage(damage);

        }
    }


    private void Update()
    {

    }

}
