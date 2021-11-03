using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SubstrateToActivateGameplay : MonoBehaviour, IPointerDownHandler
{
    public delegate void ActivateGameplay();
    public event ActivateGameplay OnActivatingGameplay;

    public void OnPointerDown(PointerEventData pointerData)
    {
        OnActivatingGameplay?.Invoke();
    }
}
