using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickHandler : MonoBehaviour
{
    [SerializeField] RectTransform _controller;
    [SerializeField] RectTransform _rotater;
    [SerializeField] Transform _player;
    [SerializeField] GameObject _joystick;
    [SerializeField] float _speed;
    
    RectTransform _selfRectTransform;

    Vector2 _startPosition;
    Vector2 _finalPosition;

    bool _movingToBorder = false;
    bool _movingBack = false;

    


    void Start()
    {
        _selfRectTransform = GetComponent<RectTransform>();
        _startPosition = _controller.localPosition;
        _finalPosition = new Vector2(_startPosition.x, 60f);
    }

    void Update()
    {
        if (_player)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _joystick.SetActive(true);
                _selfRectTransform.position = Input.mousePosition;
                _movingToBorder = true;
                _movingBack = false;
            }
            else if (Input.GetMouseButton(0))
            {
                _controller.localPosition = Vector2.Lerp(_controller.localPosition, _finalPosition, _speed * Time.deltaTime);
                _rotater.localRotation = Quaternion.Euler(0f, 0f, -_player.localEulerAngles.y);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _joystick.SetActive(false);
                _movingToBorder = false;
                _movingBack = true;
            }    
        }

        if ((_movingBack && !_movingToBorder) || (!_player && _movingToBorder))
        {
            _controller.localPosition = Vector2.Lerp(_controller.localPosition, _startPosition, _speed * Time.deltaTime);
            if (_controller.localPosition.y < 0.5f)
            {
                _controller.localPosition = _startPosition;
                _movingBack = false;
                _movingToBorder = false;
            }
        }
    }
}
