using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    public float MaxHeight
    {
        get
        {
            return _maxHeight;
        }
    }
    public float MinHeight
    {
        get
        {
            return _minHeight;
        }
    }

    [SerializeField] float _maxHeight;
    [SerializeField] float _minHeight;
}
