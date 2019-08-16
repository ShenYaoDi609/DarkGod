/****************************************************
	文件：StateMgr.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/24 20:24   	
	功能：状态管理器
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMgr:MonoBehaviour
{
    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();

    public void Init()
    {
        fsm.Add(AniState.Idle, new StateIdle());
        fsm.Add(AniState.Move, new StateMove());
        fsm.Add(AniState.Attack, new StateAttack());
        fsm.Add(AniState.Born, new StateBorn());
        fsm.Add(AniState.Die, new StateDie());
        fsm.Add(AniState.Hit, new StateHit());
        PECommon.Log("StateMgr Init Done");
    }

    public void ChangeState(EntityBase entity,AniState targetAniState,params object[] args)
    {
        if (entity.currentAniState == targetAniState)
        {
            return;
        }

        //状态切换的处理
        if(fsm.ContainsKey(targetAniState))
        {
            if(entity.currentAniState != AniState.None)
            {
                IState currentState = fsm[entity.currentAniState];
                currentState.Exit(entity,args);
            }
            IState targetState = fsm[targetAniState];
            targetState.Enter(entity,args);
            targetState.Process(entity,args);
            //entity.currentAniState = targetAniState;
        }
    }
}

