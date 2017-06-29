using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRot : MonoBehaviour {

    public Vector3 rotationvect;
    public Vector3 startrot;

	// Use this for initialization
	void Start () {
        Cursor.visible = true;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = Quaternion.Euler((rotationvect * Time.frameCount) + startrot);
	}
}
