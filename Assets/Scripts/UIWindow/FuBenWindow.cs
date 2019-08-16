/****************************************************
    文件：FuBenWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/22 21:36:10
	功能：副本选择界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class FuBenWindow : WindowRoot 
{
    public Transform pointerTrans;
    public Button[] fbBtns;
    public Button closeBtn;

    private PlayerData playerData;

    protected override void InitWindow()
    {
        base.InitWindow();

        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    protected override void ClearWindow()
    {
        base.ClearWindow();
    }


    public void RefreshUI()
    {
        int fubenID = playerData.fuben;
        for(int i = 0; i < fbBtns.Length;i++)
        {
            if (i < fubenID % 10000)
            {
                SetActive(fbBtns[i].gameObject);
                if (i == fubenID % 10000 - 1)
                {
                    pointerTrans.SetParent(fbBtns[i].transform);
                    pointerTrans.localPosition = new Vector3(7, 101,0);
                }
            }
            else
            {
                SetActive(fbBtns[i].gameObject, false);
            }
        }

    }

    public void OnCloseBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        SetWindowState(false);
    }

    public void OnTaskBtnClick(int fbid)
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);

        //检查体力是否足够TODO
        int costStamina = resSvc.GetMapCfg(fbid).costStamina;
        if (playerData.stamina < costStamina)
        {
            GameRoot.AddTipsToQueue("体力不足");
        }
        else
        {
            //向服务器发送数据,请求进入战斗场景
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqFBFight,
                reqFBFight = new ReqFBFight
                {
                    fbid = fbid,
                }
            };
            netSvc.SendMsg(msg);
        }
        
        SetWindowState(false);
        
    }
}