/****************************************************
    文件：PEListener.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/19 14:46:56
	功能：UI监听事件插件
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PEListener : MonoBehaviour, IPointerClickHandler , IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Action<object> onClick;
    public Action<PointerEventData> onClickDown;
    public Action<PointerEventData> onClickUp;
    public Action<PointerEventData> onDrag;

    public object args;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(onClick != null && args != null)
        {
            onClick(args);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(onDrag != null)
        {
            onDrag(eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(onClickDown != null)
        {
            onClickDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(onClickUp != null)
        {
            onClickUp(eventData);
        }
    }
}