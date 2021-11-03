using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookMarker : MonoBehaviour
{
    [SerializeField] GameObject _marker;
    [SerializeField] Transform _book;
    [SerializeField] BookCollisionHandler _bookHandler;
    [SerializeField] BookDeactivating _bookDeactivator;

    UnitInfo _ghost;

    Transform _target;
    

    void Start()
    {
        _ghost = _bookHandler.Book.Owner;

        _ghost.OnDying += TurnOffMarker;
        _bookDeactivator.OnBookDissapearing += ChangeMarkerTarget;

        _target = _book;
    }

    void Update()
    {
        Vector3 direction = (_target.position - transform.position);
        direction = new Vector3(direction.x, 0, direction.z).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void ChangeMarkerTarget(BookDeactivating bookDeactivator)
    {
        _target = _ghost.transform;
    }

    void TurnOffMarker(UnitInfo info)
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        _ghost.OnDying -= TurnOffMarker;
        _bookDeactivator.OnBookDissapearing -= ChangeMarkerTarget;
    }
}
