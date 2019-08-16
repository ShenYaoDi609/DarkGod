/****************************************************
    文件：Test.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/29 19:28:38
	功能：测试人物动画
*****************************************************/

using UnityEngine;

public class Test : MonoBehaviour 
{

    public CharacterController controller;
    public GameObject daggerskill1fx;

    private Transform cameraTrans;

    private Vector3 offset;

    private float targetBlend;
    private float currentBlend;

    public Animator animator;
    protected bool isMove = false;
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

    public void Start()
    {
        cameraTrans = Camera.main.transform;
        offset = transform.position - cameraTrans.position;
    }


    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 _dir = new Vector2(h, v).normalized;
        Dir = _dir;

        UpdateMixBlend();
        //当人物移动时才调用
        if (isMove)
        {
            //控制方向
            SetDir();
            //控制移动
            SetMove();
            //相机跟随
            CameraFollow();
        }
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Skill1") && info.normalizedTime >= 1.0f)
        {
            animator.SetInteger("Action", -1);
            daggerskill1fx.SetActive(false);
        }
    }

    public void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        //注意摄像机的偏移
        Vector3 eulerAngles = new Vector3(0, cameraTrans.eulerAngles.y + angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    public void SetMove()
    {
        controller.Move(transform.forward * Constant.PlayerMoveSpeed * Time.deltaTime);
        //MainCitySys.Instance.UpdateCharShowCam();
    }

    public void CameraFollow()
    {
        if (cameraTrans != null)
        {
            cameraTrans.position = transform.position - offset;
        }
    }

    public  void SetBlend(float blend)
    {
        targetBlend = blend;
        
    }

    private void UpdateMixBlend()
    {
        //如果目标值与当前值的差小于每帧要变化的值，直接将当前值设为目标值
        if (Mathf.Abs(targetBlend - currentBlend) < Constant.smoothSpeed * Time.deltaTime)
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

    public void ClickSkill1Btn()
    {
        animator.SetInteger("Action", 1);
        daggerskill1fx.gameObject.SetActive(true);
    }

}