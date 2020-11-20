using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImmediatePushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler 
{
    public MainMenu mainMenu;
    public void OnPointerUp(PointerEventData data)
    {
    }
    public void OnPointerDown(PointerEventData data)
    {
        mainMenu.PlayGame();
    }
}
