using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 1;
    public Vector3 rotationValue = new Vector3(15, 30, 45);
    void Update()
    {
        //Rotate this object according to the provided vector 3 over time
        transform.Rotate(rotationValue * Time.deltaTime * speed);
    }
}
