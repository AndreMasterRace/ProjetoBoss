using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    private Animator _anim;
    private Vector3 CameraOffset;
    private float CameraRotationDamping;
    private float CameraFollowDamping;
    public Vector3 SpawnPosition;

    private bool appeared;

    private void Awake()
    {
        CameraOffset = new Vector3(0, 1.5f, -4);
        CameraFollowDamping = 2f;
        CameraRotationDamping = 5f;
        appeared = false;
        _anim = GetComponent<Animator>();
    }

    public void Spawn()
    {
        StartCoroutine(CameraPan());
    }

    private void OnTriggerStay(Collider other)
    {
        print("enter");
        if (other.tag == "Player")
        {
            print("tag good");
            if (other.GetComponent<PlayerController2>().IsInteracting)
            {
                print("open");
                Open();
            }

        }
    }

    public void Open()
    {
        //_anim.SetBool("isOpen", true);
        _anim.Play("ChestOpen");
    }

    private void Update()
    {
        if (appeared)
        {
            //Camera.main.transform.rotation = Quaternion.Lerp(transform.rotation, /*transform.rotation + */Quaternion.Euler(0.0f, 180, 0.0f), CameraRotationDamping * Time.deltaTime);


            var cameraTarget = transform.position + transform.rotation * CameraOffset;

            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraTarget, CameraFollowDamping * Time.deltaTime);

            Camera.main.transform.LookAt(transform.position);

        }
    }

    public IEnumerator CameraPan()
    {
        yield return new WaitForSeconds(1f);
        PlayerEnabler.IsEnabled = false;
        //transform.position = new Vector3(128, 23.9f, 59);
        transform.position = SpawnPosition;
        appeared = true;
        yield return new WaitForSeconds(3.5f);
        appeared = false;
        PlayerEnabler.IsEnabled = true;
        yield return null;
    }

}
