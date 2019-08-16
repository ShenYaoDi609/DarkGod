/****************************************************
    文件：StateMove.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 13:42:17
	功能：Nothing
*****************************************************/

using UnityEngine;

public class StateMove : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Move;
        //PECommon.Log("Enter Move");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        //PECommon.Log("Exit Move");
    }

    public void Process(EntityBase entity, params object[] args)
    {
        //PECommon.Log("Process Move");
        entity.SetBlend(Constant.BlendMove);
    }
}