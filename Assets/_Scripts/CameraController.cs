using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Camera.main.transform.LookAt(transform);
	}
	
	// Update is called once per frame
	void Update () {
        Camera.main.transform.LookAt(transform);
    }
}
