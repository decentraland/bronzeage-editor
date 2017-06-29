using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Teleport : MonoBehaviour
{

    public string scenegoto;
    public string objecttodetect;

    void Update()
    {
        var up = transform.TransformDirection(Vector3.up);
        //note the use of var as the type. This is because in c# you 
        // can have lamda functions which open up the use of untyped variables
        //these variables can only live INSIDE a function. 
        RaycastHit hit;
        Debug.DrawRay(transform.position, -up * 4, Color.green);

        if (Physics.Raycast(transform.position, -up, out hit, 4))
        {

            Debug.Log("HIT");

            if (hit.collider.gameObject.name == objecttodetect)
            {
                Debug.Log("TEST");
                SceneManager.LoadScene(scenegoto);
            }
        }
    }
}