using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOptionsLoader : MonoBehaviour
{
    [Header("Player Movement Settings", order = 0)]

    [Space(1.5f)]
    [Header("Default movement", order = 1)]
    [SerializeField] [Range(0.1f, 2)] float _playerMovementSpeed;
    [SerializeField] [Range(0.1f, 25)] float _playerRotationSpeed;

    [Space(1.5f)]
    [Header("Movement with all books")]
    [SerializeField] [Range(0.1f, 2)] float _playerMovementSpeedWithAllBooks;
    [SerializeField] [Range(0.1f, 25)] float _playerRotationSpeedWithAllBooks;


    void Awake()
    {
        PlayerOptions.PlayerMovementSpeed = _playerMovementSpeed;
        PlayerOptions.PlayerMovementSpeedWithAllBooks = _playerMovementSpeedWithAllBooks;
        PlayerOptions.PlayerRotationSpeed = _playerRotationSpeed;
        PlayerOptions.PlayerRotationSpeedWithAllBooks = _playerRotationSpeedWithAllBooks;
    }
}
