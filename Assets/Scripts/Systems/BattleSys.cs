/****************************************************
	文件：BattleSys.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/24 20:02   	
	功能：战斗业务系统
*****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;
using PEProtocol;

public class BattleSys:SystemRoot
{
    #region 单例模式
    private static BattleSys _instance = null;

    public static BattleSys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<BattleSys>();
            }
            return _instance;
        }
    }
    #endregion

    public PlayerCtrlWindow playerCtrlWindow;
    public BattleEndWindow battleEndWindow;
    public BattleMgr battleMgr;

    private int fbid;
    private double startTime;

    public override void InitSys()
    {
        base.InitSys();
        PECommon.Log("Init BattleSys.....");
    }

    public void StartBattle(int fbid)
    {
        GameObject go = new GameObject {
            name = "BattleRoot"
        };
        go.transform.SetParent(GameRoot.Instance.transform);

        battleMgr = go.AddComponent<BattleMgr>();
        battleMgr.Init(fbid,()=>
        {
            startTime = timerSvc.GetNowTime();
        });
        this.fbid = fbid;
    }

    /// <summary>
    /// 副本结束处理
    /// </summary>
    /// <param name="endType"></param>
    /// <param name="isWin"></param>
    /// <param name="restHp"></param>
    public void EndBattle(FBEndType endType,bool isWin,int restHp)
    {
        playerCtrlWindow.SetWindowState(false);
        GameRoot.Instance.dynamicWindow.RemoveAllItemHp();
        if(isWin)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)(CMD.ReqFBFightEnd),
                reqFBFightEnd = new ReqFBFightEnd
                {
                    isWin = isWin,
                    fbid = fbid,
                    restHp = restHp,
                    costTime = (int)(timerSvc.GetNowTime() - startTime),
                }
            };
            netSvc.SendMsg(msg);
        }
        SetBattleEndWindow(endType, true);
    }

    /// <summary>
    /// 销毁副本
    /// </summary>
    public void DestroyBattle()
    {
        Destroy(battleMgr.gameObject);
        GameRoot.Instance.dynamicWindow.RemoveAllItemHp();
        playerCtrlWindow.SetWindowState(false);
    }

    public void SetPlayerCtrlWindow(bool isActive = true)
    {
        playerCtrlWindow.SetWindowState(isActive);
    }

    public void SetBattleEndWindow(FBEndType endType,bool isActive = true)
    {
        battleEndWindow.SetWindType(endType);
        battleEndWindow.SetWindowState(isActive);
    }

    public void SetPlayerMoveDir(Vector2 dir)
    {
        battleMgr.SetMoveDir(dir);
    }

    public void ReqReleaseSkill(int index)
    {
        battleMgr.ReqReleaseSkill(index);
    }

    public Vector2 GetCurrentDirInput()
    {
        return playerCtrlWindow.currentDir;
    }

    public EntityPlayer GetEntityPlayer()
    {
        return battleMgr.GetEntityPlayer();
    }

    public void RspFBFightEnd(GameMsg msg)
    {
        RspFBFightEnd rspFBFightEnd = msg.rspFBFightEnd;
        GameRoot.Instance.SetPlayerDataByRspFBFightEnd(rspFBFightEnd);

        battleEndWindow.RefreshWinInfo(rspFBFightEnd.fbid, rspFBFightEnd.costTime, rspFBFightEnd.restHp);
    }

}

