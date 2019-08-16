/****************************************************
	文件：TimerSvc.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/20 8:49   	
	功能：计时服务
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class TimerSvc:SystemRoot
{
    #region 单例模式
    private static TimerSvc _instance = null;
    public static TimerSvc Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<TimerSvc>();
            }
            return _instance;
        }
    }
    #endregion

    private PETimer peTimer;

    public void InitSvc()
    {
        peTimer = new PETimer();
        //设置日志输出方式 
        peTimer.SetLog((string info) =>
        {
            PECommon.Log(info);
        });
        PECommon.Log("Init TimerSvc...");
    }

    private void Update()
    {
        peTimer.Update();
    }

    public int AddTimeTask(Action<int> callback,double delay,PETimeUnit timeUnit = PETimeUnit.Millisecond,int loopTime = 1)
    {
        return peTimer.AddTimeTask(callback, delay, timeUnit, loopTime);
    }

    public double GetNowTime()
    {
        return peTimer.GetMillisecondsTime();
    }

    public void DeleteTimeTask(int tid)
    {
        peTimer.DeleteTimeTask(tid);
    }


}

