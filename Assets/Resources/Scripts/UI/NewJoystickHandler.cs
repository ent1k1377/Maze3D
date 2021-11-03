using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewJoystickHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private NewJoystickHandler _joystickParent;
    private GameObject _joystick;
    private Image _joystickOutter;
    private Image _joystickInner;
    private Vector2 _inputVector;

    public void Start()
    {
        _joystickParent = transform.GetComponent<NewJoystickHandler>();
        _joystick = transform.GetChild(0).gameObject;
        _joystickOutter = _joystick.transform.GetComponent<Image>();
        _joystickInner = _joystick.transform.GetChild(0).transform.GetComponent<Image>();
        _joystick.SetActive(false);
    }

    public void StartPosition(Vector2 pos)
    {
        _joystickOutter.rectTransform.position = new Vector2(pos.x, pos.y);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickOutter.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / _joystickOutter.rectTransform.sizeDelta.x);
            pos.y = (pos.y / _joystickOutter.rectTransform.sizeDelta.x);

            _inputVector = new Vector2(pos.x * 2, pos.y * 2);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;
            _joystickInner.rectTransform.anchoredPosition = new Vector2(_inputVector.x * (_joystickOutter.rectTransform.sizeDelta.x / 2.75f), _inputVector.y * (_joystickOutter.rectTransform.sizeDelta.y / 2.75f));
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _inputVector = Vector2.zero;
        _joystickInner.rectTransform.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _joystick.SetActive(true);
        _joystickParent.StartPosition(new Vector2(eventData.position.x, eventData.position.y));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _joystick.SetActive(false);
    }
}
