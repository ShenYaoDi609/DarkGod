/****************************************************
	文件：FuBenSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/22 21:51   	
	功能：副本系统
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class FuBenSys:SystemRoot
{
    #region 单例模式
    private static FuBenSys _instance = null;

    public static FuBenSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<FuBenSys>();
            }
            return _instance;
        }
    }
    #endregion

    public FuBenWindow fubenWindow;

    private Transform characterTrans;
    private Transform cameraTrans;

    public override void InitSys()
    {
        base.InitSys();

        PECommon.Log("Init FuBenSys.....");
    }

    #region FuBen Window
    public void OpenFuBenWindow()
    {
        fubenWindow.SetWindowState(true);
    }
    #endregion

    public void RspFBFight(GameMsg msg)
    {
        GameRoot.AddTipsToQueue("正在进入战斗场景");

        GameRoot.Instance.SetPlayerDataByRspFBFight(msg.rspFBFight);

        MainCitySys.Instance.mainCityWindow.SetWindowState(false);
        //进入战斗场景
        BattleSys.Instance.StartBattle(msg.rspFBFight.fbid);
    }

}

