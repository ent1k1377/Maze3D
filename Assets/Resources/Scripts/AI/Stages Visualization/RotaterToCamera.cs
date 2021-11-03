using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotaterToCamera : MonoBehaviour
{
    [SerializeField] Transform _camera;



    void Update()
    {
        Vector3 cameraPosition = new Vector3(_camera.position.x, 0, _camera.position.z);
        Vector3 selfPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Quaternion rotation = Quaternion.LookRotation((cameraPosition - selfPosition).normalized);
        transform.rotation = rotation;
        transform.LookAt(_camera);
    }
}
