using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Sprite _clickedSprite;
    Sprite _defaultSprite;
    Image _buttonImage;

    RectTransform _selfRTransform;

    float _heightDifference;



    void Start()
    {
        if (GetComponent<Image>())
        {
            _buttonImage = GetComponent<Image>();
            _defaultSprite = _buttonImage.sprite;
        }

        _selfRTransform = GetComponent<RectTransform>();
        _heightDifference = 4.5f;
    }

    public void OnPointerDown(PointerEventData pointerData)
    {
        _selfRTransform.anchoredPosition = new Vector2(_selfRTransform.anchoredPosition.x, _selfRTransform.anchoredPosition.y - _heightDifference);
        _buttonImage.sprite = _clickedSprite;
    }

    public void OnPointerUp(PointerEventData pointerData)
    {
        _selfRTransform.anchoredPosition = new Vector2(_selfRTransform.anchoredPosition.x, _selfRTransform.anchoredPosition.y + _heightDifference);
        _buttonImage.sprite = _defaultSprite;
    }
}
