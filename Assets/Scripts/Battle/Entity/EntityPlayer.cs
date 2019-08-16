/****************************************************
    文件：EntityPlayer.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 14:8:55
	功能：玩家实体数据类
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public class EntityPlayer : EntityBase 
{
    public EntityPlayer()
    {
        entityType = EntityType.Player;
    }

    public override Vector2 GetCurrentDirInput()
    {
        return battleMgr.GetCurrentDirInput();
    }

    public override Vector2 CalcTargetDir()
    {
        EntityMonster target = GetCloseTarget();
        if(target != null)
        {
            Vector3 targetPos = target.GetPos();
            Vector3 selfPos = GetPos();
            Vector2 dir = new Vector2(targetPos.x - selfPos.x, targetPos.z - selfPos.z);
            return dir.normalized;
        }
        return Vector2.zero;
    }

    private EntityMonster GetCloseTarget()
    {
        float minDis = float.MaxValue;
        List<EntityMonster> monsterLists = battleMgr.GetEntityMonster();
        EntityMonster targetMonster = null;
        foreach (EntityMonster monster in monsterLists)
        {
            float curDis = Vector2.Distance(monster.GetPos(), GetPos());
            if(curDis < minDis)
            {
                minDis = curDis;
                targetMonster = monster;
            }
        }
        return targetMonster;
    }

    public override void SetHpVal(string name, int oldVal, int newVal)
    {
        BattleSys.Instance.playerCtrlWindow.SetSelfHpBar(newVal);
    }

    public override void SetDodge(string name)
    {
        GameRoot.Instance.dynamicWindow.SetSelfDodge();
    }
}