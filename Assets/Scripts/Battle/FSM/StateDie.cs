/****************************************************
    文件：StateDie.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/3 10:30:22
	功能：死亡状态处理
*****************************************************/

using UnityEngine;

public class StateDie : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Die;
        entity.ClearSkillCB();
    }

    public void Exit(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        entity.SetAction(Constant.ActionDie);
        if(entity.entityType == EntityType.Monster)
        {
            entity.GetCharController().enabled = false;
            TimerSvc.Instance.AddTimeTask((int tid) =>
            {
                entity.SetActive(false);
            }, Constant.DieAniLength);
        }
    }
}