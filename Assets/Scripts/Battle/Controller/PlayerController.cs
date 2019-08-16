/****************************************************
    文件：PlayerController.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/20 14:56:25
	功能：角色控制器
*****************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class PlayerController : Controller 
{
    public GameObject dagger_skill1;
    public GameObject dagger_skill2;
    public GameObject dagger_skill3;

    public GameObject dagger_atk1;
    public GameObject dagger_atk2;
    public GameObject dagger_atk3;
    public GameObject dagger_atk4;
    public GameObject dagger_atk5;


    private Vector3 offset;

    private float targetBlend;
    private float currentBlend;

    public  override void Init()
    {
        base.Init();

        cameraTrans = Camera.main.transform;
        offset = transform.position - cameraTrans.position;

        if(dagger_skill1 != null)
        {
            fxObjDicts.Add("dagger_skill1", dagger_skill1);
        }
       if(dagger_skill2 != null)
        {
            fxObjDicts.Add("dagger_skill2", dagger_skill2);
        }
       if(dagger_skill3 != null)
        {
            fxObjDicts.Add("dagger_skill3", dagger_skill3);
        }
       if(dagger_atk1 != null)
        {
            fxObjDicts.Add("dagger_atk1", dagger_atk1);
        }
        if (dagger_atk2 != null)
        {
            fxObjDicts.Add("dagger_atk2", dagger_atk2);
        }
        if (dagger_atk3 != null)
        {
            fxObjDicts.Add("dagger_atk3", dagger_atk3);
        }
        if (dagger_atk4 != null)
        {
            fxObjDicts.Add("dagger_atk4", dagger_atk4);
        }
        if (dagger_atk5 != null)
        {
            fxObjDicts.Add("dagger_atk5", dagger_atk5);

        }
    }

    private void Update()
    {
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");

        //Vector2 _dir = new Vector2(h, v).normalized;
        //Dir = _dir;

        if(currentBlend !=  targetBlend)
        {
            UpdateMixBlend();
        }
        
        //当人物移动时才调用
        if(isMove)
        {       
            //控制方向
            SetDir();
            //控制移动
            SetMove();
            //相机跟随
            CameraFollow();
        }

        if(isSkillMove)
        {
            SetSkillMove();
            CameraFollow();
        }
    }


    public void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir,new Vector2(0, 1));
        //注意摄像机的偏移
        Vector3 eulerAngles = new Vector3(0, cameraTrans.eulerAngles.y + angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    public void SetMove()
    {
        controller.Move(transform.forward * Constant.PlayerMoveSpeed * Time.deltaTime);
        controller.Move(Vector3.down * Constant.PlayerMoveSpeed * Time.deltaTime);
        MainCitySys.Instance.UpdateCharShowCam();
    }

    public void SetSkillMove()
    {
        controller.Move(transform.forward * skillMoveSpeed * Time.deltaTime);
    }

    public void CameraFollow()
    {
        if(cameraTrans != null)
        {
            cameraTrans.position = transform.position - offset;
        }      
    }

    public override void SetBlend(float blend)
    {
        targetBlend = blend;
    }

    public override void SetFx(string name, float destroyTime)
    {
        GameObject fx = null;
        if(fxObjDicts.TryGetValue(name,out fx))
        {
            fx.SetActive(true);
            timeSvc.AddTimeTask((int timeID) =>
            {
                fx.SetActive(false);
            }, destroyTime);
        }
    }

    public override void SetAction(int actionID)
    {
        animator.SetInteger("Action", actionID);
    }


    private void UpdateMixBlend()
    {
        //如果目标值与当前值的差小于每帧要变化的值，直接将当前值设为目标值
        if(Mathf.Abs(targetBlend - currentBlend) < Constant.smoothSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
        }
        else 
        {
            if (targetBlend > currentBlend)
            {
                currentBlend += Constant.smoothSpeed * Time.deltaTime;
            }
            else if (targetBlend < currentBlend)
            {
                currentBlend -= Constant.smoothSpeed * Time.deltaTime;
            }
        }
        
        animator.SetFloat(Constant.Blend, currentBlend);
    }


}