/****************************************************
    文件：DynamicWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/3 16:26:20
	功能：Tips动态窗口
*****************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWindow : WindowRoot 
{
    public Animation anim;
    public Animation selfDodgeAnim;
    public Text tipText;
    public Transform hpItemRoot;

    private Queue<string> tipsQueue = new Queue<string>();
    private bool isTipsShow = false;

    private Dictionary<string, ItemEntityHp> itemHpDicts = new Dictionary<string, ItemEntityHp>();

    protected override void InitWindow()
    {
        base.InitWindow();
        //隐藏提示
        SetActive(tipText, false);
    }

    public void AddTipsToQueue(string tips,string color = Constant.ColorYellow)
    {
        lock(tipsQueue)
        {
            tipsQueue.Enqueue(color + tips + Constant.ColorEnd);
        }
    }

    #region Tips
    private void Update()
    {
        lock(tipsQueue)
        {
            if (tipsQueue.Count > 0 && isTipsShow == false)
            {
                string tips = tipsQueue.Dequeue();
                isTipsShow = true;

                SetTips(tips);
            }
        }
    }

    private void SetTips(string tips)
    {
        //显示tips
        SetActive(tipText, true);
        //设置文本
        SetText( tipText, tips);

        //获取动画并播放
        AnimationClip clip = anim.GetClip("ShowTips");
        anim.Play();

        //延时关闭激活状态
        StartCoroutine(DoAfterAnimDone(clip.length, () =>
        {
            SetActive(tipText, false);
            isTipsShow = false;
        }));
    }

    public  IEnumerator DoAfterAnimDone(float delay,Action finish)
    {
        yield return new WaitForSeconds(delay);
        if(finish !=  null)
        {
            finish();
        }
    }
    #endregion

    public void InitHpItemInfo(string name, Transform trans,int hp)
    {
        ItemEntityHp itemHp = null;
        if(itemHpDicts.TryGetValue(name, out itemHp))
        {
            return;
        }
        else
        {
            Transform itemHpTrans = resSvc.LoadPrefab(PathDefine.ItemEntityHp,true).transform;
            itemHpTrans.SetParent(hpItemRoot);
            itemHpTrans.localPosition = new Vector3(-1000, 0, 0);
            itemHp = itemHpTrans.gameObject.GetComponent<ItemEntityHp>();
            itemHp.InitItemInfo(trans,hp);
            itemHpDicts.Add(name, itemHp);
        }
    }

    public void RemoveItemHp(string name)
    {
        ItemEntityHp itemHp = null;
        if (itemHpDicts.TryGetValue(name, out itemHp))
        {
            itemHpDicts.Remove(name);
            Destroy(itemHp.gameObject);
        }
    }

    public void RemoveAllItemHp()
    {
        foreach(ItemEntityHp item in itemHpDicts.Values)
        {
            Destroy(item.gameObject);
        }
        itemHpDicts.Clear();
    }

    public void SetCritical(string name,int critical)
    {
        ItemEntityHp itemHp = null;
        if(itemHpDicts.TryGetValue(name,out itemHp))
        {
            itemHp.SetCritical(critical);
        }
    }

    public void SetDodge(string name)
    {
        ItemEntityHp itemHp = null;
        if(itemHpDicts.TryGetValue(name,out itemHp))
        {
            itemHp.SetDodge();
        }
    }

    public void SetHurt(string name,int hurt)
    {
        ItemEntityHp itemHp = null;
        if(itemHpDicts.TryGetValue(name,out itemHp))
        {
            itemHp.SetHurt(hurt);
        }
    }

    public void SetHp(string name,int oldVal,int newVal)
    {
        ItemEntityHp itemHp = null;
        if(itemHpDicts.TryGetValue(name,out itemHp))
        {
            itemHp.SetHp(oldVal,newVal);
        }
    }

    public void SetSelfDodge()
    {
        selfDodgeAnim.Stop();
        selfDodgeAnim.Play();
    }


}
