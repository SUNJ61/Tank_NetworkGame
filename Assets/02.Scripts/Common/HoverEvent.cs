using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class HoverEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{
    public static HoverEvent event_instance;
    public bool isDown = false;
    public bool isClick = false;
    public bool isEnter = false;

    private void Start() //이벤트들 챗 gpt한테 물어보기
    {
        event_instance = this;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isClick = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
    }
}
