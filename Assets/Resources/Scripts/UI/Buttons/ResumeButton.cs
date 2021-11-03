using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeButton : MonoBehaviour, IPointerClickHandler
{
    public delegate void DeactivatePause();
    public event DeactivatePause OnDeactivatingPause;

    public void OnPointerClick(PointerEventData pointerData)
    {
        OnDeactivatingPause?.Invoke();
    }
}
