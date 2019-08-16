/****************************************************
    文件：StateHit.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/3 12:5:43
	功能：受击状态
*****************************************************/

using UnityEngine;

public class StateHit : IState
{
    public void Enter(EntityBase entity, params object[] args)
    {
        entity.currentAniState = AniState.Hit;
        entity.ClearSkillCB();
    }

    public void Exit(EntityBase entity, params object[] args)
    {
       
    }

    public void Process(EntityBase entity, params object[] args)
    {
        //播放受击音效
        if(entity.entityType == EntityType.Player)
        {
            AudioSource audio = entity.GetAudioSource();
            AudioSvc.Instance.PlayCharHitAudio(audio, Constant.AssassinHit);
        }

        if(entity.entityType == EntityType.Player)
        {
            entity.canReleaseSkill = false;
        }
        //受击停止移动
        entity.SetDir(Vector2.zero);
        entity.SetAction(Constant.ActionHit);
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            entity.SetAction(Constant.ActionDefault);
            entity.Idle();
        }, (int)(GetHitAniLength(entity) * 1000));
    }

    private float GetHitAniLength(EntityBase entity)
    {
        AnimationClip[] clips = entity.GetAniClip();
        for(int i = 0; i < clips.Length; i++)
        {
            string clipName = clips[i].name;
            if (clipName.Contains("hit")||clipName.Contains("Hit")||clipName.Contains("HIT"))
            {
                return clips[i].length;
            }
        }
        return 1;
    }
}