using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerClickHandler
{
    public delegate void ActivateMainMenu();
    public event ActivateMainMenu OnActivatingMainMenu;

    public void OnPointerClick(PointerEventData pointerData)
    {
        OnActivatingMainMenu?.Invoke();
    }
}
