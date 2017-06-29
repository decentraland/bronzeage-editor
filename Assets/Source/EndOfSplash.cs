using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfSplash : MonoBehaviour {

    void Start()
    {
        StartCoroutine(WaitAndMove());
    }

    IEnumerator WaitAndMove()
    {
        yield return new WaitForSeconds(6);
        SceneManager.LoadScene("Browser");
    }
}
