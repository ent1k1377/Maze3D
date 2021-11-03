using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenuButton : MonoBehaviour, IPointerClickHandler
{
    public delegate void ActivatePause();
    public event ActivatePause OnActivatingPause;

    public void OnPointerClick(PointerEventData pointerData)
    {
        OnActivatingPause?.Invoke();
    }
}
