using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] BookCollisionHandler _bookCollisionHandler;

    //[SerializeField] Material _transparentMaterial;
    //[SerializeField] Material _opaqueMaterial;
    //[SerializeField] SkinnedMeshRenderer _skinRenderer;

    AIMover _mover;


    void Start()
    {
        _bookCollisionHandler.OnTakingBook += ChangeMaterial;
        _mover = GetComponent<AIMover>();
    }

    void ChangeMaterial(BookCollisionHandler collision)
    {
        //_skinRenderer.material = _opaqueMaterial;
        _mover.StopMoving();
    }

    void OnDestroy()
    {
        _bookCollisionHandler.OnTakingBook -= ChangeMaterial;
    }
}
