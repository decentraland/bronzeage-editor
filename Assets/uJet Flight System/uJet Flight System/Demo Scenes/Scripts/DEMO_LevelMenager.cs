using UnityEngine;
using System.Collections;

public class DEMO_LevelMenager : MonoBehaviour {

	// Use this for initialization
	public void LevelJetpack3D () {
		Application.LoadLevel ("Jetpack_3D");
	}
	public void LevelJetpack2D () {
		Application.LoadLevel ("Jetpack_2D");
	}
	public void LevelSpaceAngel () {
		Application.LoadLevel ("Space_FPS3D");
	}
	public void LevelSpaceAngel2D () {
		Application.LoadLevel ("Space_2D");
	}
	public void LevelCastleAngel () {
		Application.LoadLevel ("Angel_FPS3D");
	}
}
