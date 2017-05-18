using UnityEngine;
using System.Collections;

public class DEMO_Esc_BackToMenu : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
	
				if (Input.GetButtonDown ("Esc")) {
			Application.LoadLevel(0);
		}
	}
}
