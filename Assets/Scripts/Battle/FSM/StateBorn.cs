/****************************************************
    文件：StateBorn.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/3 10:5:21
	功能：出生状态
*****************************************************/

using UnityEngine;

public class StateBorn : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Born;
        PECommon.Log("Enter Born");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        //entity.SetAction(Constant.ActionDefault);
        PECommon.Log("Enter Born");
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constant.ActionBorn);
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            entity.SetAction(Constant.ActionDefault);
        },500);
        PECommon.Log("Process Born");
    }
}