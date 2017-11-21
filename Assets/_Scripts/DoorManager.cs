using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour {

    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        print("enter");
        if (other.tag == "Player" && Inventory.hasBossKey)
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
        _anim.Play("DoorOpen");
        TakeKey();
    }

    public void TakeKey()
    {
        Inventory.hasBossKey =false;
        print(Inventory.hasBossKey);
    }
}
