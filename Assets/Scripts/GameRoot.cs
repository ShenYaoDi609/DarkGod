/****************************************************
    文件：GameRoot.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/2 17:22:25
	功能：游戏启动入口
*****************************************************/
using System;
using UnityEngine;
using PEProtocol;

public class GameRoot : MonoBehaviour 
{

    public static GameRoot Instance = null;

    public LoadingWindow loadingWindow;
    public DynamicWindow dynamicWindow;

    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }


    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        PECommon.Log("Game Start....");
        ClearUIRoot();
        Init();
    }

    //初始化各UIwindow的显示/隐藏
    private void ClearUIRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
        dynamicWindow.SetWindowState();
    }

    private void Init()
    {
        //服务模块初始化
        ResSvc resSvc = GetComponent<ResSvc>();
        resSvc.InitSvc();
        AudioSvc audioSvc = GetComponent<AudioSvc>();
        audioSvc.InitSvc();
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();
        TimerSvc timeSvc = GetComponent<TimerSvc>();
        timeSvc.InitSvc();

        //业务系统初始化
        LoginSys loginSys = GetComponent<LoginSys>();
        loginSys.InitSys();

        //主城系统初始化
        MainCitySys mainCitySys = GetComponent<MainCitySys>();
        mainCitySys.InitSys();

        //副本系统初始化
        FuBenSys fubenSys = GetComponent<FuBenSys>();
        fubenSys.InitSys();

        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();

        //进入登录界面并 加载UI
        loginSys.EnterLogin();

    }

    /// <summary>
    /// 显示Tips
    /// </summary>
    /// <param name="tips"></param>
    public static void AddTipsToQueue(string  tips,string color  = Constant.ColorYellow)
    {
        Instance.dynamicWindow.AddTipsToQueue(tips,color);
    }

    /// <summary>
    /// 设置PlayerData
    /// </summary>
    /// <param name="_playerData"></param>
    public void SetPlayerData(PlayerData _playerData)
    {
        playerData = _playerData;
    }

    public void SetPlayerDataName(string name)
    {
        playerData.name = name;
    }
 
    public void SetPlayerDataByRspTask(RspTask taskData)
    {
        PlayerData.taskid = taskData.taskID;
        PlayerData.coin = taskData.coin;
        PlayerData.exp = taskData.exp;
        PlayerData.lv = taskData.lv;
        PlayerData.hp = taskData.hp;
    }

    public void SetPlayerDataByRspStrong(RspStrong strongData)
    {
        PlayerData.ad += strongData.addhurt;
        PlayerData.ap += strongData.addhurt;
        PlayerData.addef += strongData.adddef;
        PlayerData.apdef += strongData.adddef;
        PlayerData.hp += strongData.addhp;
        PlayerData.coin -= strongData.coin;
        PlayerData.crystal -= strongData.crystal;

        PlayerData.strongArr = strongData.strongArr;
    }

    public void SetPlayerDataByRspPurchase(RspPurchase purchaseData)
    {
        PlayerData.diamond = purchaseData.diamond;
        PlayerData.coin = purchaseData.coin;
        PlayerData.stamina = purchaseData.stamina;
    }

    public void SetPlayerDataByPshStamina(PshStamina pshStamina)
    {
        PlayerData.stamina = pshStamina.stamina;
    }

    public void SetPlayerDataByRspTakeTaskReward(RspTakeTaskReward rspTakeTaskReward)
    {
        PlayerData.coin = rspTakeTaskReward.coin;
        PlayerData.exp = rspTakeTaskReward.exp;
        PlayerData.lv = rspTakeTaskReward.lv;
        PlayerData.hp = rspTakeTaskReward.hp;
        PlayerData.taskArr = rspTakeTaskReward.taskArr;
    }

    public void SetPlayerDataByPshTaskProgs(PshTaskProgs pshTaskProgs)
    {
        PlayerData.taskArr = pshTaskProgs.taskArr;
    }

    public void SetPlayerDataByRspFBFight(RspFBFight rspFBFight)
    {
        PlayerData.stamina = rspFBFight.stamina;
    }

    public void SetPlayerDataByRspFBFightEnd(RspFBFightEnd rspFBFightEnd)
    {
        PlayerData.coin = rspFBFightEnd.coin;
        PlayerData.exp = rspFBFightEnd.exp;
        PlayerData.lv = rspFBFightEnd.lv;
        PlayerData.crystal = rspFBFightEnd.crystal;
        PlayerData.fuben = rspFBFightEnd.fuben;
    }
}