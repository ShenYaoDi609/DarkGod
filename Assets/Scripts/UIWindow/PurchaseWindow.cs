/****************************************************
    文件：PurchaseWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/17 8:52:10
	功能：购买窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class PurchaseWindow : WindowRoot 
{
    #region UIDefine
    public Button sureBtn;
    public Button closeBtn;
    public Text infoTxt;
    #endregion

    private int buyType;//0：体力 1：金币
    private int costDiamond = 10;

    public void SetBuyType(int type)
    {
        buyType = type;
    }

    protected override void InitWindow()
    {
        base.InitWindow();

        RefreshUI();
        sureBtn.onClick.AddListener(OnSureBtnClick);
        closeBtn.onClick.AddListener(OnCloseBtnClick);
    }

    protected override void ClearWindow()
    {
        base.ClearWindow();

        closeBtn.onClick.RemoveAllListeners();
        sureBtn.onClick.RemoveAllListeners();
    }

    public void RefreshUI()
    {
        switch (buyType)
        {
            case 0:
                infoTxt.text = "确定花费<color=#00FFFF>" + costDiamond + "钻石</color>购买<color=#00FF00FF>100体力</color>？";
                break;
            case 1:
                infoTxt.text = "确定花费<color=#00FFFF>" + costDiamond + "钻石</color>铸造<color=#FFFF00FF>100金币</color>？";
                break;
        }
    }


    public void OnSureBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        //发送消息到服务器
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqPurchase,
            reqPurchase = new ReqPurchase
            {
                buyType = buyType,
                costDiamond = costDiamond,
            }
        };
        netSvc.SendMsg(msg);
        MainCitySys.Instance.ClosePurchaseWindow();
    }

    public void OnCloseBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        MainCitySys.Instance.ClosePurchaseWindow();
    }
}