/****************************************************
    文件：ItemTask.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/21 19:59:9
	功能：更新每条任务显示
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class ItemTask : MonoBehaviour 
{
    public Text txtName;
    public Text txtPrg;
    public Image proVal;
    public Image takeImg;
    public Button takeBtn;
    public Transform compTrans;
    public Text txtExp;
    public Text txtCoin;

    private int rewardID;

    private void Start()
    {
        takeBtn.onClick.AddListener(OnTakeBtnClick);
    }

    public void RefreshUI(TaskRewardCfg trf,TaskRewardData trc)
    {
        rewardID = trf.ID;
        txtName.text = trf.taskName;
        txtExp.text = trf.exp.ToString();
        txtCoin.text = "金币 " + trf.coin.ToString();
        txtPrg.text = trc.progress + "/" + trf.count;
        float pec = trc.progress * 1.0f / trf.count;
        proVal.fillAmount = pec;
        bool taked = trc.taked;
        if(taked == false)
        {
            compTrans.gameObject.SetActive(false);
            if (trf.count != trc.progress)
            {
                takeBtn.interactable = false;
            }
            else
            {
                takeBtn.interactable = true;
            }
        }
        else
        {
            takeBtn.interactable = false;
            compTrans.gameObject.SetActive(true);
        }
    }

    public void OnTakeBtnClick()
    {
        AudioSvc.Instance.PlayUIAudio(Constant.UICommonClick);
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqTakeTaskReward,
            reqTakeTaskReward = new ReqTakeTaskReward
            {
                rid = rewardID,
            }
        };
        NetSvc.Instance.SendMsg(msg);

        TaskRewardCfg trc = ResSvc.Instance.GetTaskRewardCfg(rewardID);
        int coin = trc.coin;
        int exp = trc.exp;
        GameRoot.AddTipsToQueue("获得奖励：" + "金币 " + coin + " 经验 " + exp, Constant.ColorRed);
    }
}