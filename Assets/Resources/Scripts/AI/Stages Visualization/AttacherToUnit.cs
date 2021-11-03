using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacherToUnit : MonoBehaviour
{
    [SerializeField] Transform _unit;


    void Update()
    {
        transform.position = new Vector3(_unit.position.x, transform.position.y, _unit.position.z);
    }
}
