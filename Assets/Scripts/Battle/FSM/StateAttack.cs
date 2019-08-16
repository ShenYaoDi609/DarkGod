/****************************************************
    文件：StateAttack.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 21:2:20
	功能：攻击状态
*****************************************************/

using UnityEngine;

public class StateAttack : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Attack;
        entity.curSkillData = ResSvc.Instance.GetSkillCfg((int)args[0]);
        //PECommon.Log("Enter Attack");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        entity.ExitCurAtkState();
        //PECommon.Log("Exit Attack");
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if(entity.entityType == EntityType.Player)
        {
            entity.canReleaseSkill = false;
        }
        entity.SkillAttack((int)args[0]);
        //PECommon.Log("Process Attack");
    }
}