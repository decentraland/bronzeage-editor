using UnityEngine;
using System.Collections;

public class DEMO_LockMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
	

}
