/****************************************************
    文件：IState.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 13:36:24
	功能：战斗状态接口
*****************************************************/

using UnityEngine;

public interface IState
{
    void Enter(EntityBase entity,params object[] args);
    void Process(EntityBase entity,params object[] args);
    void Exit(EntityBase entity,params object[] args);
}

public enum AniState
{
    None,
    Idle,
    Move,
    Attack,
    Born,
    Die,
    Hit,
}