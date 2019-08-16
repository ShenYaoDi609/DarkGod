/****************************************************
    文件：NetSvc.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/5 19:44:52
	功能：网络服务
*****************************************************/

using UnityEngine;
using PENet;
using PEProtocol;
using System.Collections.Generic;

public class NetSvc : SystemRoot 
{
    #region 单例模式
    private static NetSvc _instance = null;
    public static NetSvc Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<NetSvc>();
            }
            return _instance;
        }
    }
    #endregion
    public PESocket<ClientSession, GameMsg> client = null;

    private static readonly string obj = "lock";
    private Queue<GameMsg> msgQueue = new Queue<GameMsg>();

    public void InitSvc()
    {
        client = new PESocket<ClientSession, GameMsg>();


        client.SetLog(true, (string msg, int lv) =>
        {
            switch (lv)
            {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg = "LogWarning:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg = "LogError:" + msg;
                    Debug.LogError(msg);
                    break;
                case 4:
                    msg = "Info:" + msg;
                    Debug.Log(msg);
                    break;
            }
        });
        client.StartAsClient(SrvCfg.srvIP, SrvCfg.srvPort);
        PECommon.Log("Init NetSvc...");
    }

    public void SendMsg(GameMsg msg)
    {
        if(client.session != null)
        {
            client.session.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTipsToQueue("服务器未连接");
            InitSvc();
        }
    }

    public void AddMsgQue(GameMsg msg)
    {
        lock(obj)
        {
            msgQueue.Enqueue(msg);
        }
    }

    private void Update()
    {
        if(msgQueue.Count > 0)
        {
            PECommon.Log("msgPackCount:" + msgQueue.Count);
            lock(obj)
            {
                GameMsg msg = msgQueue.Dequeue();
                HandOutMsg(msg);
            }
        }
    }

    public void HandOutMsg(GameMsg msg)
    {
        //有错误
        if(msg.err != (int)ErrorCode.None)
        {
            switch((ErrorCode)msg.err)
            {
                case ErrorCode.ServerDataError:
                    GameRoot.AddTipsToQueue("服务器数据错误，请重新登录");
                    break;
                case ErrorCode.AcctIsOnLine:
                    GameRoot.AddTipsToQueue("账号已上线");
                    break;
                case ErrorCode.WrongPwd:
                    GameRoot.AddTipsToQueue("密码错误,请输入正确的密码");
                    break;
                case ErrorCode.UpdateDBError:
                    PECommon.Log("数据库更新异常", LogType.Error);
                    GameRoot.AddTipsToQueue("网络不稳定");
                    break;
                case ErrorCode.NameIsExist:
                    GameRoot.AddTipsToQueue("该角色名已存在");
                    break;
                case ErrorCode.ClientDataError:
                    PECommon.Log("客户端数据异常", LogType.Error);
                    GameRoot.AddTipsToQueue("网络不稳定");
                    break;
            }
            return;
        }

        switch((CMD)msg.cmd)
        {
            case CMD.RspLogin:
                LoginSys.Instance.RspLogin(msg);
                break;
            case CMD.RspRename:
                LoginSys.Instance.RspRename(msg);
                break;
            case CMD.RspTask:
                MainCitySys.Instance.RspTask(msg);
                break;
            case CMD.RspStrong:
                MainCitySys.Instance.RspStrong(msg);
                break;
            case CMD.PshChat:
                MainCitySys.Instance.PshChat(msg);
                break;
            case CMD.RspPurchase:
                MainCitySys.Instance.RspPurchase(msg);
                break;
            case CMD.PshStamina:
                MainCitySys.Instance.PshStamina(msg);
                break;
            case CMD.RspTakeTaskReward:
                MainCitySys.Instance.RspTakeTaskReward(msg);
                break;
            case CMD.PshTaskProgs:
                MainCitySys.Instance.PshTaskProgs(msg);
                break;
            case CMD.RspFBFight:
                FuBenSys.Instance.RspFBFight(msg);
                break;
            case CMD.RspFBFFightEnd:
                BattleSys.Instance.RspFBFightEnd(msg);
                break;
        }
    }
}