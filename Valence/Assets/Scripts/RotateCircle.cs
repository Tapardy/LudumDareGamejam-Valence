using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCircle : MonoBehaviour
{
    [SerializeField] private Transform circle; 
    [SerializeField] private float rotationSpeed = 10f; 
    [SerializeField] private bool clockwise = false; 

    private void FixedUpdate()
    {
        float direction = clockwise ? -1f : 1f;

        float rotationAmount = direction * rotationSpeed * Time.fixedDeltaTime;

        circle.Rotate(0, 0, rotationAmount);
    }

}
