/****************************************************
    文件：SystemRoot.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/3 10:53:1
	功能：系统类基类
*****************************************************/

using UnityEngine;

public class SystemRoot : MonoBehaviour 
{
    protected ResSvc resSvc = null;
    protected AudioSvc audioSvc = null;
    protected NetSvc netSvc = null;
    protected TimerSvc timerSvc = null;

    public virtual void InitSys()
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        netSvc = NetSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }

    public virtual void ClearSvc()
    {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
        timerSvc = null;
    }
}