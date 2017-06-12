using UnityEngine;
using System.Collections;

public class PositionUpdater : MonoBehaviour
{
    public Vector3 newPosition;

    void Update()
    {
        this.transform.position.Set(newPosition.x, newPosition.y, newPosition.z);
    }
}