/****************************************************
    文件：MonsterController.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/1 14:54:7
	功能：Nothing
*****************************************************/

using UnityEngine;

public class MonsterController : Controller 
{
    private Vector2 curDir;
    private Vector2 targetDir;

    private void Update()
    {
        if(curDir != targetDir)
        {
            UpdateMixDir();
        }

        if(isMove)
        {
            SetDir();
            SetMove();
        }
    }

    public void SetDir()
    {
        targetDir = Dir;
    }

    private void SetMove()
    {
        controller.Move(transform.forward * Time.deltaTime * Constant.MonsterMoveSpeed);
        //controller.Move(Vector3.down * Time.deltaTime * Constant.MonsterMoveSpeed);
    }

    private void UpdateMixDir()
    {
        if(Mathf.Abs(targetDir.x - curDir.x) < Constant.smoothSpeed *Time.deltaTime && Mathf.Abs(targetDir.y - curDir.y) < Constant.smoothSpeed * Time.deltaTime)
        {
            curDir = targetDir;
        }
        else
        {
            if(curDir.x < targetDir.x)
            {
                curDir.x += Constant.smoothSpeed * Time.deltaTime;
            }
            else
            {
                curDir.x -= Constant.smoothSpeed * Time.deltaTime;
            }

            if (curDir.y < targetDir.y)
            {
                curDir.y += Constant.smoothSpeed * Time.deltaTime;
            }
            else
            {
                curDir.y -= Constant.smoothSpeed * Time.deltaTime;
            }
        }

        float angle = Vector2.SignedAngle(curDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.eulerAngles = eulerAngles;
    }

}