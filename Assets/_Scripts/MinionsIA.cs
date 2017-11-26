//using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

namespace Assets
{
    public class MinionsIA : MonoBehaviour
    {

        //-----------------------------------
        public enum AISTATE { IDLE = 0, CHASE = 1, ATTACK = 2 };
        public AISTATE CurrentState = AISTATE.IDLE;
        private NavMeshAgent ThisAgent = null;
        //private Transform transform = null;
        private Transform PlayerObject = null;

        //AI Visiility Settings
        public bool CanSeePlayer = false;
        public float ViewAngle = 90f;
        public float AttackDistance = 1f;
        //-----------------------------------
        // Use this for initialization
        void Awake()
        {
            ThisAgent = GetComponent<NavMeshAgent>();
            //transform = GetComponent<Transform>();
            //transform
            PlayerObject = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        //-----------------------------------
        void Start()
        {
            //Set Starting State
            ChangeState(CurrentState);
        }

        //-----------------------------------
        public IEnumerator Idle()
        {
            //Get Random Point
            Vector3 Point = RandomPointOnNavMesh();
            float WaitTime = 2f;
            float ElapsedTime = 0f;

            //Loop while idling
            while (CurrentState == AISTATE.IDLE)
            {
                print("idle");
                ThisAgent.SetDestination(Point);

                ElapsedTime += Time.deltaTime;

                if (ElapsedTime >= WaitTime)
                {
                    ElapsedTime = 0f;
                    Point = RandomPointOnNavMesh();
                }

                if (CanSeePlayer)
                {
                    ChangeState(AISTATE.CHASE);
                    yield break;
                }

                yield return null;
            }
        }
        //-----------------------------------
        public IEnumerator Chase()
        {
            while (CurrentState == AISTATE.CHASE)
            {
                ThisAgent.SetDestination(PlayerObject.position);

                print("can see player");
                if (!CanSeePlayer)
                {
                    print("cant see player");
                    yield return new WaitForSeconds(2f);

                    if (!CanSeePlayer)
                    {
                        ChangeState(AISTATE.IDLE);
                        yield break;
                    }
                }
                print(Vector3.Distance(transform.position, PlayerObject.position));
                if (Vector3.Distance(transform.position, PlayerObject.position) <= AttackDistance)
                {
                    ChangeState(AISTATE.ATTACK);
                    yield break;
                }

                yield return null;
            }
        }
        //-----------------------------------
        public IEnumerator Attack()
        {
            while (CurrentState == AISTATE.ATTACK)
            {
                ThisAgent.SetDestination(transform.position);

                //Deal damage here
                if (!CanSeePlayer || Vector3.Distance(transform.position, PlayerObject.position) > AttackDistance)
                {
                    ChangeState(AISTATE.CHASE);
                }

                yield return null;
            }
        }
        //-----------------------------------
        public void ChangeState(AISTATE NewState)
        {
            StopAllCoroutines();
            CurrentState = NewState;

            switch (NewState)
            {
                case AISTATE.IDLE:
                    StartCoroutine(Idle());
                    break;

                case AISTATE.CHASE:
                    StartCoroutine(Chase());
                    break;

                case AISTATE.ATTACK:
                    StartCoroutine(Attack());
                    break;
            }
        }



        //-----------------------------------

        private void OnTriggerStay(Collider other)
        {

            if (other.tag != "Player")
                return;

            print("trigger stay");

            CanSeePlayer = false;

            //Player transform
            Transform PlayerTransform = other.GetComponent<Transform>();

            //Is player in sight
            Vector3 DirToPlayer = PlayerTransform.position - transform.position;

            print("line of sight");
            CanSeePlayer = true;

            ////Get viewing angle
            //float ViewingAngle = Mathf.Abs(Vector3.Angle(transform.forward, DirToPlayer));

            //if (ViewingAngle > ViewAngle)
            //    return;

            //Is there a direct line of sight?
            //if (!Physics.Linecast(transform.position, PlayerTransform.position))
            //{
            //    print("line of sight");
            //    CanSeePlayer = true;
            //}

        }

        //-----------------------------------
        public Vector3 RandomPointOnNavMesh()
        {
            float Radius = 5f;
            Vector3 Point = transform.position + Random.insideUnitSphere * Radius;
            NavMeshHit NH;
            NavMesh.SamplePosition(Point, out NH, Radius, NavMesh.AllAreas);
            return NH.position;
        }
        //-----------------------------------

        private void OnTriggerExit(Collider other)
        {
            print("trigger exit");
            if (other.tag != "Player")
                return;

            CanSeePlayer = false;
        }
        //-----------------------------------
    }


}
