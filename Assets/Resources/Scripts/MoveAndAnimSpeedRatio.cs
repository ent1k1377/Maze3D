using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAndAnimSpeedRatio : MonoBehaviour
{
    [SerializeField] float _movementSpeedRatio;

    float _defaultMoveAnimSpeed = 1f;

    
    public float AnimationSpeed(float currentMovementSpeed)
    {
        float speed = _defaultMoveAnimSpeed * (currentMovementSpeed / _movementSpeedRatio);
        return speed;
    }
}
