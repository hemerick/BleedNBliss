using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Action OnClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke(); //Si OnClick n'est pas null, Execute OnClick()
        transform.localScale = transform.localScale / 1.1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = transform.localScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = transform.localScale / 1.1f;
    }
    
}
