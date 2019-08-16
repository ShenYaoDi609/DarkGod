/****************************************************
    文件：WindowRoot.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/2 20:10:7
	功能：UI界面基类
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class WindowRoot : MonoBehaviour 
{
    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;
    protected NetSvc netSvc = null;
    protected TimerSvc timeSvc = null;

    public void SetWindowState(bool isActive = true)
    {
        if(gameObject.activeInHierarchy !=  isActive)
        {
            SetActive(gameObject, isActive);
        }
        if(isActive)
        {
            InitWindow();
        }
        else
        {
            ClearWindow();
        }
    }

    protected virtual void InitWindow()
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timeSvc = TimerSvc.Instance;
    }

    protected virtual void ClearWindow()
    {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
        timeSvc = null;
    }

    #region TextToolFunction
    protected void SetText(Text txt,string context = "")
    {
        txt.text = context;
    }
    protected void SetText(Text txt,int num = 0)
    {
        txt.text = num.ToString();
    }
    protected void SetText(Transform trans,string context =  "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }
    protected void SetText(Transform trans, int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num);
    }
    #endregion

    #region ImageToolFunction
    public void SetImage(Image image,string path)
    {
        Sprite sprite = resSvc.LoadSprite(path);
        image.sprite = sprite;
    }

    public void SetImage(Image image, Sprite sprite)
    { 
        image.sprite = sprite;
    }
    #endregion
    #region Show/Hide ToolFunction
    protected void SetActive(GameObject go,bool isActive = true)
    {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans,bool isActive = true)
    {
        trans.gameObject.SetActive(isActive);
    }
    protected void SetActive(RectTransform rectTrans,bool isActive = true)
    {
        rectTrans.gameObject.SetActive(isActive);
    }
    protected void SetActive(Text txt,bool isActive = true)
    {
        txt.gameObject.SetActive(isActive);
    }
    protected  void SetActive(Image image,bool isActive = true)
    {
        image.gameObject.SetActive(isActive);
    }

    #endregion

    #region Click Event
    protected T GetOrAddComponent<T>(GameObject go) where T:MonoBehaviour
    {
        T t = go.GetComponent<T>();
        if(t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    protected void OnClick(GameObject go,Action<object> evt,object args)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClick = evt;
        listener.args = args;
    }

    protected void OnClickDown( GameObject go, Action<PointerEventData> evt)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickDown = evt;
    }

    protected void OnClickUp(GameObject go, Action<PointerEventData> evt)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onClickUp = evt;
    }

    protected void OnDrag(GameObject go, Action<PointerEventData> evt)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.onDrag = evt;
    }
    #endregion
}