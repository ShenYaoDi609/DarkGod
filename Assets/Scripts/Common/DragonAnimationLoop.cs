/****************************************************
    文件：DragonAnimationLoop.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/3 11:11:46
	功能：飞龙循环动画
*****************************************************/

using UnityEngine;

public class DragonAnimationLoop : MonoBehaviour 
{
    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    private void Start()
    {
        if(anim != null)
        {
            InvokeRepeating("PlayDragonAnimation", 0, 20);
        }
    }

    private void PlayDragonAnimation()
    {
        if(anim != null)
        {
            anim.Play();
        }
    }
}