using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBandage : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    private Vector3 initialPosition;

    void Start()
    {
        // Save initial position to prevent movement
        initialPosition = transform.position;
    }

    void Update()
    {
        
        transform.position = initialPosition;

        
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.Self);
    }
}
