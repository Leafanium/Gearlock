using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBandage : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f; // Speed of rotation
    private Vector3 initialPosition;

    void Start()
    {
        // Save initial position to prevent movement
        initialPosition = transform.position;
    }

    void Update()
    {
        // Keep the object at its original position
        transform.position = initialPosition;

        // Rotates the object in place on the Y-axis only
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.Self);
    }
}
