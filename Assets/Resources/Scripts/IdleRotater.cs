using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRotater : MonoBehaviour
{
    [HideInInspector] public float RotationSpeed;


    void Start()
    {
        RotationSpeed = BookOptions.BookIdleRotation;
    }

    void FixedUpdate()
    {
        float angle = transform.rotation.eulerAngles.y + RotationSpeed;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, angle, transform.rotation.eulerAngles.z);
    }
}
