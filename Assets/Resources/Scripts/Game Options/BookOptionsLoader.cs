using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookOptionsLoader : MonoBehaviour
{
    [Space(3)]
    [Header("Book idle settings")]
    [SerializeField] [Range(0f, 5f)] float _bookIdleRotation;

    [Space(3)]
    [Header("Book deactivating settings")]
    [SerializeField] [Range(0.2f, 10f)] float _deactivatingDuration;
    [SerializeField] [Range(0.01f, 5f)] float _rotateAcceleration;
    [SerializeField] [Range(0.1f, 5f)] float _animationSpeed;
    [SerializeField] [Range(0.1f, 5f)] float _animLengthMult;
    

    void Awake()
    {
        BookOptions.BookIdleRotation = _bookIdleRotation;

        BookOptions.DeactivatingDuration = _deactivatingDuration;
        BookOptions.RotateAcceleration = _rotateAcceleration;   
        BookOptions.AnimationLengthMult = _animLengthMult;   
        BookOptions.AnimationSpeed = _animationSpeed;
    }
}
