/****************************************************
	文件：SkillMgr.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/24 20:25   	
	功能：技能管理器
*****************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class SkillMgr : MonoBehaviour
{
    private ResSvc resSvc;
    private TimerSvc timeSvc;
    private Dictionary<int, bool> skillCdDicts = new Dictionary<int, bool>();

    private bool IsCritical = false;

    public void Init()
    {
        resSvc = ResSvc.Instance;
        timeSvc = TimerSvc.Instance;

        Dictionary<int, SkillCfg> skillDicts = resSvc.skillCfgDicts;
        foreach(var skill in skillDicts.Values)
        {
            skillCdDicts.Add(skill.ID, true);
        }
        PECommon.Log("SkillMgr Init Done");
    }

    public void SkillAttack(EntityBase entity,int skillID)
    {
        entity.skillMoveCBList.Clear();
        entity.skillActionCBList.Clear();

        AttackEffect(entity, skillID);
        AttackDamage(entity, skillID);
    }

    /// <summary>
    /// 技能效果表现
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillID"></param>
    public void AttackEffect(EntityBase entity, int skillID)
    {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);

        if(!skillData.isCollide)
        {
            //忽略刚体碰撞
            Physics.IgnoreLayerCollision(9, 11);
            timeSvc.AddTimeTask((int tid) =>
            {
                Physics.IgnoreLayerCollision(9, 11, false);
            },skillData.skillTime);
        }

        if(!skillData.isBreak)
        {
            entity.entityState = EntityState.StoicState;
        }

        if(entity.entityType == EntityType.Player)
        {
            if (entity.battleMgr.GetCurrentDirInput() == Vector2.zero)
            {
                //自动寻找怪物
                Vector2 targetDir = entity.CalcTargetDir();
                if (targetDir != Vector2.zero)
                {
                    entity.SetAtkDirLocal(targetDir);
                }
            }
            else
            {
                entity.SetAtkDirCam(entity.battleMgr.GetCurrentDirInput());
            }
        }


        entity.SetAction(skillData.aniAction);
        entity.SetFx(skillData.fx, skillData.skillTime);


        entity.canControl = false;
        entity.SetDir(Vector2.zero);

        CalcSkillMove(entity, skillData);

        int endID = timeSvc.AddTimeTask((int tid) =>
        {
            entity.skillEndID = -1;
            entity.Idle();
        }, skillData.skillTime);
        entity.skillEndID = endID;
    }

    public void CalcSkillMove(EntityBase entity, SkillCfg skillData)
    {
        List<int> skillMoveList = skillData.skillMoveList;

        //多阶段位移
        int sumDelayTime = 0;
        for (int i = 0; i < skillMoveList.Count; i++)
        {
            SkillMoveCfg skillMoveData = resSvc.GetSkillMoveCfg(skillMoveList[i]);
            float speed = skillMoveData.moveDis / (skillMoveData.moveTime * 1.0f / 1000);
            sumDelayTime += skillMoveData.delayTime;

            if (sumDelayTime > 0)
            {
                int moveID =  timeSvc.AddTimeTask((int tid) =>
                {
                    entity.SetSkillMoveState(true, speed);
                    entity.RemoveMoveCB(tid);
                }, sumDelayTime);
                entity.skillMoveCBList.Add(moveID);
            }
            else
            {
                entity.SetSkillMoveState(true, speed);
            }

            sumDelayTime += skillMoveData.moveTime;
            int stopID = timeSvc.AddTimeTask((int tid) =>
            {
                entity.SetSkillMoveState(false);
                entity.RemoveMoveCB(tid);
            }, sumDelayTime);
            entity.skillMoveCBList.Add(stopID);

        }
    }

    /// <summary>
    /// 计算技能伤害
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="skillID"></param>
    public void AttackDamage(EntityBase entity,int skillID)
    {
        SkillCfg skillData = resSvc.GetSkillCfg(skillID);
        List<int> actionList = skillData.skillActionList;

        int sumDelayTime = 0;
        for (int i = 0; i < actionList.Count; i++)
        {
            SkillActionCfg skillAction = resSvc.GetSkillActionCfg(actionList[i]);

            sumDelayTime += skillAction.delayTime;
            int index = i;
            if (sumDelayTime > 0)
            {
                int dmgID = timeSvc.AddTimeTask((int tid) =>
                {
                    SkillAction(entity, skillData,index);
                    entity.RemoveActionCB(tid);
                },sumDelayTime);
                entity.skillActionCBList.Add(dmgID);
            }
            else
            {
                //瞬时技能
                SkillAction(entity, skillData,index);
            }
        }
    }

    public void SkillAction(EntityBase caster,SkillCfg skillCfg,int index)
    {
        SkillActionCfg skillActionData = resSvc.GetSkillActionCfg(skillCfg.skillActionList[index]);
        int damage = skillCfg.skillDamageList[index];
        if(caster.entityType == EntityType.Player)
        {
            //获取场景中所有怪物实体
            List<EntityMonster> monsterList = caster.battleMgr.GetEntityMonster();

            for (int i = 0; i < monsterList.Count; i++)
            {
                EntityMonster target = monsterList[i];
                //判断距离和角度，对满足条件的怪物进行伤害运算
                if (InRange(caster.GetPos(), target.GetPos(), skillActionData.radius) && InAngle(caster.GetTrans(), target.GetPos(), skillActionData.angle))
                {
                    CalcDamage(caster, target, skillCfg, damage);
                }
            }
        }
        else if(caster.entityType == EntityType.Monster)
        {
            EntityPlayer entityPlayer = caster.battleMgr.GetEntityPlayer();
            if(entityPlayer == null)
            {
                return;
            }
            if(InRange(caster.GetPos(),entityPlayer.GetPos(),skillActionData.radius) && InAngle(caster.GetTrans(),entityPlayer.GetPos(),skillActionData.angle))
            {
                CalcDamage(caster, entityPlayer, skillCfg, damage);
            }
        }
       

    }

    private void CalcDamage(EntityBase caster,EntityBase target, SkillCfg skillCfg, int damage)
    {

        int dmgSum = damage;
        DamageType dmgType = skillCfg.dmgType;
        if(dmgType == DamageType.AD)
        {
            //计算闪避
            int dodgeNum = Tools.GetRandomInt(1, 100);
            if(dodgeNum <=  target.Props.dodge)
            {
                //UI显示闪避
                target.SetDodge(target.Name);
                PECommon.Log("闪避：" + dodgeNum + "/" + target.Props.dodge);
                return;
            }
            //计算属性加成
            dmgSum += caster.Props.ad;
           
            //计算暴击
            int criticalNum = Tools.GetRandomInt(1, 100);
            if(criticalNum <= caster.Props.critical)
            {
                float criticalRate = 1 + Tools.GetRandomInt(1, 100) / 100.0f;
                dmgSum = (int)(criticalRate * dmgSum);
                IsCritical = true;              
                PECommon.Log("暴击：" + criticalNum + "/" + target.Props.critical);
            }

            //计算穿甲
            int addef = (int)((1 - caster.Props.pierce / 100.0f) * target.Props.addef);
            dmgSum -= addef;
        }
        else if(dmgType == DamageType.AP)
        {
            //计算属性加成
            dmgSum += caster.Props.ap;
            //计算魔法抗性
            dmgSum -= target.Props.apdef;
        }

        if(dmgSum < 0)
        {
            dmgSum = 0;
            return;
        }

        if(target.HP < dmgSum)
        {
            target.HP = 0;

            target.Die();
            if(target.entityType == EntityType.Monster)
            {
                target.battleMgr.RemoveMonster(target.Name);
            }
            else if(target.entityType == EntityType.Player)
            {                
                target.battleMgr.EndBattle(FBEndType.Lose,false, 0);
                target.battleMgr.entityPlayer = null;
            }

        }
        else
        {
            if(IsCritical)
            {
                target.SetCritical(target.Name, dmgSum);
                IsCritical = false;
            }
            else
            {
                target.SetHurt(target.Name, dmgSum);
            }

            target.HP -= dmgSum;

            if (target.entityState == EntityState.None && target.GetBreakState())
            {
                target.Hit();
            }

        }
        //target.SetHpVal(target.controller.gameObject.name, target.HP);
    }

    private bool InRange(Vector3 from,Vector3 to,float range)
    {
        float dis = Vector3.Distance(from, to);
        if(dis <= range)
        {
            return true;
        }
        return false;
    }

    private bool InAngle(Transform trans,Vector3 to, float angle)
    {
        Vector3 delta = to - trans.position;
        float tmpAngle = Mathf.Acos(Vector3.Dot(delta.normalized, trans.forward)) * Mathf.Rad2Deg;
        
        if (tmpAngle <= angle * 0.5f)
        {
            return true;
        }
        return false;
    }

   
}
    

