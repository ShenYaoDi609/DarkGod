/****************************************************
    文件：EntityMonster.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/1 14:51:30
	功能：怪物数据逻辑实体
*****************************************************/

using UnityEngine;

public class EntityMonster : EntityBase 
{
    public EntityMonster()
    {
        entityType = EntityType.Monster;
    }

    public MonsterData monsterData;

    //AI逻辑检测间隔
    private float checkTime = 2;
    private float checkCount = 0;

    public override void SetBattleProps(BattleProps battleProps)
    {
        int level = monsterData.mLevel;

        BattleProps props = new BattleProps
        {
            hp = battleProps.hp * level,
            ad = battleProps.ad * level,
            ap = battleProps.ap * level,
            addef = battleProps.addef * level,
            apdef = battleProps.apdef * level,
            dodge = battleProps.dodge * level,
            pierce = battleProps.pierce * level,
            critical = battleProps.critical * level,
        };

        Props = props;
        HP = Props.hp;
    }

    private bool runAI = true;
    private float atkTime = 2;
    private float atkCount = 0;
    public override void TickAILogic()
    {
        if (runAI == false)
        {
            Idle();
            return;
        }         

        if(currentAniState == AniState.Idle || currentAniState == AniState.Move)
        {
            if (battleMgr.isPauseGame)
            {
                Idle();
                return;
            }

            checkCount += Time.deltaTime;

            if (checkCount < checkTime)
            {
                return;
            }
            else
            {

                //计算目标方向
                Vector2 dir = CalcTargetDir();
                //判断目标是否在攻击范围内
                if (!InAtkRange())
                {
                    //不在范围内：设置方向，进入移动状态
                    SetDir(dir);
                    Move();
                }
                else
                {
                    //在范围内：停止移动，进行攻击
                    SetDir(Vector2.zero);
                    atkCount += checkCount;
                    //判断攻击间隔
                    if (atkCount > atkTime)
                    {
                        //达到攻击时间，转向并攻击
                        SetAtkDirLocal(dir);
                        Attack(monsterData.monsterCfg.skillID);
                        atkCount = 0;
                    }
                    else
                    {
                        //未达到攻击时间，Idle等待
                        Idle();
                    }
                    checkCount = 0;
                    checkTime = Tools.GetRandomInt(1, 5) * 1.0f / 10;
                }
            }
        

        }
    }

    public override Vector2 CalcTargetDir()
    {
        EntityPlayer entityPlayer = battleMgr.GetEntityPlayer();
        if(entityPlayer == null || entityPlayer.currentAniState == AniState.Die)
        {
            runAI = false;
            return Vector2.zero;
        }
        Vector3 targetPos = entityPlayer.GetPos();
        Vector3 selfPos = GetPos();
        Vector2 dir = new Vector2(targetPos.x - selfPos.x, targetPos.z - selfPos.z);
        return dir.normalized;
    }

    private bool InAtkRange()
    { 
        EntityPlayer entityPlayer = battleMgr.GetEntityPlayer();
        if(entityPlayer == null || entityPlayer.currentAniState == AniState.Die)
        {
            runAI = false;
            return false;
        }
        Vector3 targetPos = entityPlayer.GetPos();
        Vector3 selfPos = GetPos();
        targetPos.y = 0;
        selfPos.y = 0;
        float dis = Vector3.Distance(targetPos, selfPos);
        float atkDis = monsterData.monsterCfg.atkDis;
        if(dis <= atkDis)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override bool GetBreakState()
    {
        if(monsterData.monsterCfg.isStop)
        {
            if(curSkillData != null)
            {
                return curSkillData.isBreak;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public override void SetHpVal (string name, int oldVal, int newVal)
    {
        if (monsterData.monsterCfg.mType == MonsterType.Boss)
        {
            BattleSys.Instance.playerCtrlWindow.SetBossHp(oldVal,newVal, Props.hp);
            if(newVal == 0)
            {
                BattleSys.Instance.playerCtrlWindow.SetBossHpState(false);
            }
        }
        else if (monsterData.monsterCfg.mType == MonsterType.Normal)
        {
            base.SetHpVal(name, oldVal, newVal);
        }
    }
}