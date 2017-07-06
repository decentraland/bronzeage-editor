using UnityEngine;
using System.Collections;

public class PositionUpdater : MonoBehaviour
{
    public Vector3 newPosition;
    public float easenessFactor = 0.95f;

    private Vector3 distance = new Vector3();

    void Update()
    {
        distance = (transform.position - newPosition);
        distance.x *= easenessFactor;
        distance.y *= easenessFactor;
        distance.z *= easenessFactor;
        transform.position -= distance;
    }
}