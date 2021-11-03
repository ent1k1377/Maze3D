using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] Transform _camera;
    
    [SerializeField] [Range(0.5f, 10f)] float _followSpeed;

    [SerializeField] float _distancingRange;
    [SerializeField] float _distancingSpeed;
    [SerializeField] float _returnSpeed;
    [SerializeField] float _delayToStartDistancing;

    float _spacingZ;
    float _spacingX;
    float _heightRatio;

    Vector3 _defaultDistance;
    Vector3 _finalDistance;

    Coroutine _raiseCamera;
    Coroutine _lowerCamera;


    void Start()
    {
        _spacingZ = transform.localPosition.z;
        _spacingX = transform.localPosition.x;

        _heightRatio = Mathf.Abs(transform.localPosition.y / transform.localPosition.z);

        _defaultDistance = _camera.localPosition;
        float distancingY = _defaultDistance.y + _distancingRange * _heightRatio;
        float distancingZ = _defaultDistance.y - _distancingRange;
        if (transform.localPosition.z > 0)
            distancingZ = _defaultDistance.y + _distancingRange;
        _finalDistance = new Vector3(_defaultDistance.x, distancingY, distancingZ);
    }

    void Update()
    {
        if (_player)
            CameraFollowingTo(_player.localPosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (_lowerCamera != null)
            {
                StopCoroutine(_lowerCamera);
                _lowerCamera = null;
            }
            _raiseCamera = StartCoroutine(DistanceCameraPosition(_finalDistance, _distancingSpeed, _delayToStartDistancing, _raiseCamera));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_raiseCamera != null)
            {
                StopCoroutine(_raiseCamera);
                _raiseCamera = null;
            }
            _lowerCamera = StartCoroutine(DistanceCameraPosition(_defaultDistance, _returnSpeed, 0f, _lowerCamera));
        }

    }

    void CameraFollowingTo(Vector3 target)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, 
            new Vector3(target.x + _spacingX, transform.localPosition.y, target.z + _spacingZ), 
            _followSpeed * Time.deltaTime);
    }

    IEnumerator DistanceCameraPosition(Vector3 position, float speed, float delay, Coroutine thisCoroutine)
    {
        yield return new WaitForSeconds(delay);
        while (Vector3.Distance(_camera.localPosition, position) > 0.01f)
        {
            _camera.localPosition = Vector3.Lerp(_camera.localPosition, position, speed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        _camera.localPosition = position;
        thisCoroutine = null;
    }
}
