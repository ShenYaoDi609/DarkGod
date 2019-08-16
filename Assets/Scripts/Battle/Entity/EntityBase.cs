/****************************************************
    文件：EntityBase.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 13:35:19
	功能：逻辑实体基类
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public class EntityBase
{
    public AniState currentAniState = AniState.None;
    public StateMgr stateMgr = null;
    public SkillMgr skillMgr = null;
    public BattleMgr battleMgr = null;
    protected Controller controller = null;
    public bool canControl = true;
    public bool canReleaseSkill = true;

    public EntityType entityType = EntityType.None;
    public EntityState entityState = EntityState.None;

    //技能位移的回调列表，储存所有与技能位移有关延时操作的id
    public List<int> skillMoveCBList = new List<int>();
    //技能伤害回调列表
    public List<int> skillActionCBList = new List<int>();
    public int skillEndID = -1;

    public Queue<int> atkComboQue = new Queue<int>();
    public int nextAtkID = 0;

    public SkillCfg curSkillData;

    private string name;
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    private BattleProps props;
    public BattleProps Props
    {
        get
        {
            return props;
        }
        protected set
        {
            props = value;
        }
    }

    //当前战斗的血量
    private int hp;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            SetHpVal(Name, hp, value);
            hp = value;
        }
    }

    #region 逻辑实体状态改变
    public void Idle()
    {
        stateMgr.ChangeState(this, AniState.Idle,null);
    }

    public void Move()
    {
        stateMgr.ChangeState(this, AniState.Move,null);
    }

    public void Attack(int skillID)
    {
        stateMgr.ChangeState(this, AniState.Attack,skillID);
    }

    public void Born()
    {
        stateMgr.ChangeState(this, AniState.Born, null);
    }

    public void Die()
    {
        stateMgr.ChangeState(this, AniState.Die, null);
    }

    public void Hit()
    {
        stateMgr.ChangeState(this, AniState.Hit, null);
    }
    #endregion

    public void SetController(Controller ctr)
    {
        controller = ctr;
    }

    public void SetActive(bool active = true)
    {
        if(controller != null)
        {
            controller.gameObject.SetActive(active);
        }        
    }

    /// <summary>
    /// AI控制逻辑
    /// </summary>
    public virtual void TickAILogic()
    {

    }

    public virtual void SetBattleProps(BattleProps battleProps)
    {
        HP = battleProps.hp;
        Props = battleProps;
    }

    //技能施放时特效、伤害计算等处理
    public virtual void SkillAttack(int skillID)
    {
        skillMgr.SkillAttack(this, skillID);
    }

    public virtual void SetBlend(float blend)
    {
        if(controller !=  null)
        {
            controller.SetBlend(blend);
        }
    }

    public virtual void SetDir(Vector2 dir)
    {
        if(controller != null )
        {
            controller.Dir = dir;
        }
    }

    public virtual void SetAtkDirLocal(Vector2 atkDir)
    {
        if(controller != null)
        {
            controller.SetAtkDirLocal(atkDir);
        }
    }

    public virtual void SetAtkDirCam(Vector2 atkDir)
    {
        if(controller != null)
        {
            controller.SetAtkDirCam(atkDir);
        }
    }

    /// <summary>
    /// 设置人物动画
    /// </summary>
    /// <param name="actionID"></param>
    public virtual void SetAction(int actionID)
    {
        if(controller != null)
        {
            controller.SetAction(actionID);
        }
    }

    /// <summary>
    /// 播放技能特想笑
    /// </summary>
    /// <param name="name"></param>
    /// <param name="destroyTime"></param>
    public virtual void SetFx(string name,float destroyTime)
    {
        if(controller != null)
        {
            controller.SetFx(name, destroyTime);
        }
    }

    /// <summary>
    /// 设置技能位移
    /// </summary>
    /// <param name="isMove"></param>
    /// <param name="skillSpeed"></param>
    public virtual void SetSkillMoveState(bool isMove,float skillSpeed = 0f)
    {
        controller.SetSkillMoveState(isMove, skillSpeed);
    }

    public virtual Vector2 GetCurrentDirInput()
    {
        return Vector2.zero;
    }

    public virtual void SetBattleProps()
    {

    }

    public virtual Vector3 GetPos()
    {
        return controller.transform.position;
    }

    public virtual Transform GetTrans()
    {
        return controller.transform;
    }

    #region 伤害显示
    public virtual void InitItemInfo(string name,Transform trans,int hp)
    {
        if(controller != null)
        {
            GameRoot.Instance.dynamicWindow.InitHpItemInfo(name, trans, hp);
        }       
    }

    public virtual void SetCritical(string name, int critical)
    {
        if(controller != null)
        {
            GameRoot.Instance.dynamicWindow.SetCritical(name, critical);
        }
    }

    public virtual void SetDodge(string name)
    {
        if (controller != null)
        {
            GameRoot.Instance.dynamicWindow.SetDodge(name);
        }           
    }

    public virtual void SetHurt(string name, int hurt)
    {
        if(controller != null)
        {
            GameRoot.Instance.dynamicWindow.SetHurt(name, hurt);
        }        
    }

    public virtual void SetHpVal(string  name, int oldVal,int newVal)
    {
        if(controller != null)
        {
            GameRoot.Instance.dynamicWindow.SetHp(name, oldVal, newVal);
        }       
    }

    #endregion

    /// <summary>
    /// 获取当前动画状态机的动画
    /// </summary>
    /// <returns></returns>
    public AnimationClip[] GetAniClip()
    {
        if(controller != null)
        {
            return controller.animator.runtimeAnimatorController.animationClips;
        }
        return null;
    }

    public AudioSource GetAudioSource()
    {
        return controller.GetComponent<AudioSource>();
    }

    public CharacterController GetCharController()
    {
        return controller.GetComponent<CharacterController>();
    }

    public void ExitCurAtkState()
    {
        if(entityState == EntityState.StoicState)
        {
            entityState = EntityState.None;
        }
        canControl = true;
        if(curSkillData.isCombo)
        {
            if (atkComboQue.Count > 0)
            {
                nextAtkID = atkComboQue.Dequeue();
            }
            else
            {
                nextAtkID = 0;
            }
        }
        curSkillData = null;
        SetAction(Constant.ActionDefault);
    }

    //最近的敌人的方向
    public virtual Vector2 CalcTargetDir()
    {
        return Vector2.zero;
    }

    public void RemoveMoveCB(int moveID)
    {
        int index = -1;
        for(int i = 0; i < skillMoveCBList.Count; i++)
        {
            if(skillMoveCBList[i] == moveID)
            {
                index = i;
                break;
            }
        }
        if(index != -1)
        {
            skillMoveCBList.RemoveAt(index);
        }
    }

    public void RemoveActionCB(int actionID)
    {
        int index = -1;
        for(int i = 0; i < skillActionCBList.Count; i++)
        {
            if(skillActionCBList[i] == actionID)
            {
                index = i;
                break;
            }
        }
        if(index != -1)
        {
            skillActionCBList.RemoveAt(index);
        }
    }

    public void RemoveAllMoveCB()
    {
        for(int i = 0; i < skillMoveCBList.Count; i++)
        {
            int tid = skillMoveCBList[i];
            TimerSvc.Instance.DeleteTimeTask(tid);
        }
        skillMoveCBList.Clear();
    }

    public void RemoveAllAcitonCB()
    {
        for(int i = 0; i < skillActionCBList.Count; i++)
        {
            int tid = skillActionCBList[i];
            TimerSvc.Instance.DeleteTimeTask(tid);
        }
        skillActionCBList.Clear();
    }

    public void ClearSkillCB()
    {
        SetDir(Vector2.zero);
        SetSkillMoveState(false);
        RemoveAllAcitonCB();
        RemoveAllMoveCB();

        if (nextAtkID != 0 || atkComboQue.Count > 0)
        {
            nextAtkID = 0;
            atkComboQue.Clear();

            battleMgr.lastClickTime = 0;
            battleMgr.curComboIndex = 0;
        }

        if (skillEndID != -1)
        {
            TimerSvc.Instance.DeleteTimeTask(skillEndID);
            skillEndID = -1;
        }
    }

    public virtual bool GetBreakState()
    {
        return true;
    }

}