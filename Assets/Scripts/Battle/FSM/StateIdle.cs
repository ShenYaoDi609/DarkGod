/****************************************************
    文件：StateIdle.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 13:41:47
	功能：Nothing
*****************************************************/

using UnityEngine;

public class StateIdle : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Idle;

        entity.skillEndID = -1;
        //PECommon.Log("Enter Idle");
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        //PECommon.Log("Exit Idle");
    }

    public void Process(EntityBase entity, params object[] args)
    {
        if(entity.nextAtkID != 0)
        {
            entity.Attack(entity.nextAtkID);
        }
        else
        {
            if (entity.entityType == EntityType.Player)
            {
                entity.canReleaseSkill = true;
            }

            if (entity.GetCurrentDirInput() != Vector2.zero)
            {
                entity.Move();
                entity.SetDir(entity.GetCurrentDirInput());
            }
            else
            {
                entity.SetDir(Vector2.zero);
                entity.SetBlend(Constant.BlendIdle);
            }
        }
        //PECommon.Log("Process Idle");
    }
}