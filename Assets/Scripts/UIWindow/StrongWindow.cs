/****************************************************
    文件：StrongWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/7 15:57:20
	功能：强化界面
*****************************************************/

using UnityEngine;
using PEProtocol;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class StrongWindow : WindowRoot 
{
    #region ButtonDefine
    public Button strongBtn;
    public Button closeBtn;
    #endregion
    #region TextDefine
    public Text txtStar;
    public Text txtCurPlusHp;
    public Text txtStrongPlusHp;
    public Text txtCurPlusAttk;
    public Text txtStrongPlusAttk;
    public Text txtCurPlusDefence;
    public Text txtStrongPlusDefence;
    public Text txtNeedLv;
    public Text txtNeedCoin;
    public Text txtNeedMaterial;
    public Text txtOwnCoin;
    public Text txtAfterStrong;
    #endregion
    #region TransformDefine
    public Transform posBtnTrans;
    public Transform posChooseTrans;
    public Transform starGroupTrans;
    public Transform conditionTrans;
    #endregion
    #region ImageDefine
    public Image curPosImage;
    public Image propHpArrow;
    public Image propAttkArrow;
    public Image propDefArrow;
    #endregion

    private int currentIndex;
    private StrongCfg nextStrongCfg;
    private PlayerData playerData;

    protected override void InitWindow()
    {
        base.InitWindow();
        playerData = GameRoot.Instance.PlayerData;
        ClickPosItem(0);
        RegisterMouseEvent();

        closeBtn.onClick.AddListener(OnCloseBtnClick);
        strongBtn.onClick.AddListener(OnStrongBtnClick);
    }

    protected override void ClearWindow()
    {
        base.ClearWindow();
        closeBtn.onClick.RemoveAllListeners();
    }

   
    /// <summary>
    /// 更新UI
    /// </summary>
    public void RefreshItemUI()
    {
        playerData = GameRoot.Instance.PlayerData;
        //设置总金币数
        SetText(txtOwnCoin, playerData.coin);
        //设置当前选中装备图片
        switch(currentIndex)
        {
            case 0:
                SetImage(curPosImage, PathDefine.ImageHead);
                break;
            case 1:
                SetImage(curPosImage, PathDefine.ImageBody);
                break;
            case 2:
                SetImage(curPosImage, PathDefine.ImageWaist);
                break;
            case 3:
                SetImage(curPosImage, PathDefine.ImageHand);
                break;
            case 4:
                SetImage(curPosImage, PathDefine.ImageLeg);
                break;
            case 5:
                SetImage(curPosImage, PathDefine.ImageFoot);
                break;

        }
        int curStarLv = playerData.strongArr[currentIndex];
        //设置星级
        SetText(txtStar, curStarLv + "星级");
        for(int i = 0; i < starGroupTrans.childCount; i++)
        {
            Image img = starGroupTrans.GetChild(i).GetComponent<Image>();
            if(i < curStarLv)
            {
                SetImage(img, PathDefine.yellowStar);
            }
            else
            {
                SetImage(img, PathDefine.whiteStar);
            }
        }

        //获取当前等级装备的信息
        StrongCfg curStrongCfg = resSvc.GetStrongCfg(currentIndex, curStarLv);
        //当前装备星级为0
        if (curStrongCfg == null)
        {
            SetText(txtCurPlusHp, "生命  +0");
            SetText(txtCurPlusAttk, "伤害  +0");
            SetText(txtCurPlusDefence, "防御  +0");
        }
        else
        {
            int addHp = resSvc.GetPropAddPreLv(currentIndex, curStarLv, 1);
            int addAttk = resSvc.GetPropAddPreLv(currentIndex, curStarLv, 2);
            int addDef = resSvc.GetPropAddPreLv(currentIndex, curStarLv, 3);

            SetText(txtCurPlusHp, "生命  +" + addHp);
            SetText(txtCurPlusAttk, "伤害  +" + addAttk);
            SetText(txtCurPlusDefence, "防御  +" + addDef);
        }

        //获取下个等级装备信息
        int nextStarLv = curStarLv + 1;
        nextStrongCfg = resSvc.GetStrongCfg(currentIndex, nextStarLv);
        //为空 说明已经到了最大等级
        if (nextStrongCfg == null)
        {
            //隐藏升级后的相关信息
            SetActive(conditionTrans,false);
            SetActive(propHpArrow,false);
            SetActive(propAttkArrow,false);
            SetActive(propDefArrow,false);
            SetActive(txtAfterStrong, false);
            SetActive(txtStrongPlusHp,false);
            SetActive(txtStrongPlusAttk,false);
            SetActive(txtStrongPlusDefence,false);           
        }
        else
        {
            SetActive(conditionTrans);
            SetActive(propHpArrow);
            SetActive(propAttkArrow);
            SetActive(propDefArrow);
            SetActive(txtAfterStrong);
            SetActive(txtStrongPlusHp);
            SetActive(txtStrongPlusAttk);
            SetActive(txtStrongPlusDefence);

            SetText(txtStrongPlusHp,"+" +  resSvc.GetPropAddPreLv(currentIndex, nextStarLv, 1));
            SetText(txtStrongPlusAttk, "+" + resSvc.GetPropAddPreLv(currentIndex, nextStarLv, 2));
            SetText(txtStrongPlusDefence, "+" + resSvc.GetPropAddPreLv(currentIndex, nextStarLv, 3));

            SetText(txtNeedCoin,nextStrongCfg.coin);
            SetText(txtNeedLv,"需要等级：" + nextStrongCfg.minlv);
            SetText(txtNeedMaterial, nextStrongCfg.crystal + "/" + playerData.crystal);
        }
    }

    public void RegisterMouseEvent()
    {
        for (int i = 0; i < posBtnTrans.childCount; i++)
        {
            Image img = posBtnTrans.GetChild(i).GetComponent<Image>();
            OnClick(img.gameObject, (object obj) =>
            {
                audioSvc.PlayUIAudio(Constant.UICommonClick);
                ClickPosItem((int)obj);
            },i);

        }
    }

    public void ClickPosItem(int index)
    {
        currentIndex = index;
        //设置被点击的UI的位置
        posChooseTrans.position = posBtnTrans.GetChild(index).transform.position + new Vector3(14.3f,0,0);
        //Debug.Log(posBtnTrans.GetChild(index).transform.position + posBtnTrans.GetChild(index).gameObject.name);
        //Debug.Log(posChooseTrans.position);
        RefreshItemUI();
    }


    #region BtnClick Event
    /// <summary>
    /// 关闭按钮点击事件
    /// </summary>
    public void OnCloseBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        MainCitySys.Instance.CloseStrongWindow();
    }

    /// <summary>
    /// 强化按钮点击事件
    /// </summary>
    public void OnStrongBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        if(playerData.strongArr[currentIndex] != 10)
        {
            if (playerData.lv < nextStrongCfg.minlv)
            {
                GameRoot.AddTipsToQueue("等级不足", Constant.ColorRed);
                return;
            }
            if (playerData.coin < nextStrongCfg.coin)
            {
                GameRoot.AddTipsToQueue("金钱不足", Constant.ColorRed);
                return;
            }
            if (playerData.crystal < nextStrongCfg.crystal)
            {
                GameRoot.AddTipsToQueue("水晶不足", Constant.ColorRed);
                return;
            }
            //向服务器发送强化请求
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqStrong,
                reqStrong = new ReqStrong
                {
                    pos = currentIndex,
                    starlv = playerData.strongArr[currentIndex]
                }
            };
            netSvc.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTipsToQueue("星级已升满", Constant.ColorRed);
        }
    }
    #endregion
}