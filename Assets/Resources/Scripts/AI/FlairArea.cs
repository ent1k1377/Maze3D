using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlairArea : MonoBehaviour
{
    public delegate void Sense(Transform target);
    public event Sense OnSense;

    [SerializeField] string _playerTag = "Player";

    [SerializeField] BookDeactivating _bookDeactivator;

    SphereCollider _detectingArea;


    void Start()
    {
        _detectingArea = GetComponent<SphereCollider>();
        _detectingArea.radius = AIOptions.AroundAreaOfView;

        _bookDeactivator.OnBookDissapearing += ChangeRadius;
    }

    void ChangeRadius(BookDeactivating bookDeactivator)
    {
        _detectingArea.radius = AIOptions.AroundAreaOfViewEscape;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == _playerTag)
        {
            OnSense?.Invoke(collider.transform);
        }
    }
}
