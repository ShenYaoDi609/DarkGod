/****************************************************
    文件：LoginSystem.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/2 17:26:2
	功能：登录系统
*****************************************************/

using UnityEngine;
using PEProtocol;

public class LoginSys : SystemRoot 
{

    #region 单例模式
    private static LoginSys _instance = null;

    public static LoginSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<LoginSys>();
            }
            return _instance;
        }
    }
    #endregion

    public LoginWindow loginWindow;
    public CreateWindow createWindow;

    public override void InitSys()
    {
        base.InitSys();
        PECommon.Log("Init LoginSys.....");
    }

    /// <summary>
    /// 进入登录界面
    /// </summary>
    public void EnterLogin()
    {
        //异步加载登录场景
        //显示加载进度
        //打开注册登录界面
        resSvc.AsyncLoadScene(Constant.SceneLogin,()=> {
            //显示登录界面
            loginWindow.SetWindowState(true);
            //播放背景音效
            audioSvc.PlayBGMusic(Constant.BGLogin, true);
        });

    }


    public void RspLogin(GameMsg msg)
    {
        //显示登录成功的tips
        GameRoot.AddTipsToQueue("登陆成功");

        //存储PlayerData
        GameRoot.Instance.SetPlayerData(msg.rspLogin.playerData);
        if(msg.rspLogin.playerData.name == "")
        {
            //打开角色创建界面
            createWindow.SetWindowState(true);
        }
        else
        {
            //进入主城
            MainCitySys.Instance.EnterMainCity();
        }

        //关闭登录界面
        loginWindow.SetWindowState(false);
    }

    public void RspRename(GameMsg msg)
    {
        if(msg.rspRename.name != null)
        {
            GameRoot.AddTipsToQueue("改名成功，正在进入游戏");

            GameRoot.Instance.SetPlayerDataName(msg.rspRename.name);

            //进入主城
            //关闭创建角色界面
            createWindow.SetWindowState(false);
            MainCitySys.Instance.EnterMainCity();
        }   
    }

}