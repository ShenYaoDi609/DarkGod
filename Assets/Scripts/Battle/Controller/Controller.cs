/****************************************************
    文件：Controller.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 14:19:55
	功能：表现实体控制器抽象类
*****************************************************/

using UnityEngine;
using System.Collections.Generic;

public abstract class Controller : MonoBehaviour 
{
    public Animator animator;
    public bool isMove = false;
    public CharacterController controller;
    protected Transform cameraTrans;
    public Transform hpRoot;
    private Vector2 dir = Vector2.zero;
    public Vector2 Dir
    {
        get
        {
            return dir;
        }
        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
                SetBlend(Constant.BlendIdle);
            }
            else
            {
                isMove = true;
                SetBlend(Constant.BlendMove);
            }
            dir = value;
        }
    }
    protected bool isSkillMove = false;
    protected float skillMoveSpeed = 0f;
    protected TimerSvc timeSvc;
    //存储特效的字典
    protected Dictionary<string, GameObject> fxObjDicts = new Dictionary<string, GameObject>();

    public virtual void Init()
    {
        timeSvc = TimerSvc.Instance;
    }

    public virtual void SetAtkDirLocal(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    public virtual void SetAtkDirCam(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, cameraTrans.eulerAngles.y + angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    public virtual void SetBlend(float blend) {
        animator.SetFloat("Blend", blend);
    }

    public virtual void SetAction(int actionID)
    {
        animator.SetInteger("Action", actionID);
    }

    public virtual void SetFx(string name,float delayTime)
    {

    }

    public void SetSkillMoveState(bool isMove,float skillSpeed = 0f)
    {
        isSkillMove = isMove;
        skillMoveSpeed = skillSpeed;
    }
}