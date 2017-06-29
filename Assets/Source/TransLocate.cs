using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransLocate : MonoBehaviour {

    public Button clickable;
    public InputField x;
    public InputField y;
    public Vector2 GoToVector;

	// Use this for initialization
	void Start () {
        clickable.onClick.AddListener(() => { OnButtonClick(); });
	}
	
	// Update is called once per frame
	void Update () {
        GoToVector.x = int.Parse(x.text);
        GoToVector.y = int.Parse(y.text);
	}

    void OnButtonClick ()
    {
        PlayerPrefs.SetInt("TileX", (int)GoToVector.x);
        PlayerPrefs.SetInt("TileY", (int)GoToVector.y);
        SceneManager.LoadScene("Browser");
    }
}
