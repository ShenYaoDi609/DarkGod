/****************************************************
    文件：BattleEndWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/15 14:43:44
	功能：战斗结束界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class BattleEndWindow : WindowRoot 
{
    public Button exitBtn;
    public Button closeBtn;
    public Button enSureBtn;
    public Text txtTime;
    public Text txtRestHp;
    public Text txtReward;
    public Image imgLv;
    public Transform centerPin;

    private FBEndType endType = FBEndType.None;

    protected override void InitWindow()
    {
        base.InitWindow();

        RefreshUI();
    }

    private void RefreshUI()
    {
        switch(endType)
        {
            case FBEndType.Pause:
                SetActive(centerPin, false);
                SetActive(closeBtn.gameObject);
                SetActive(exitBtn.gameObject);
                break;
            case FBEndType.Win:
                SetActive(centerPin,false);
                SetActive(closeBtn.gameObject, false);
                SetActive(exitBtn.gameObject, false);
                timeSvc.AddTimeTask((int tid) =>
                {
                    SetActive(centerPin);
                    timeSvc.AddTimeTask((int tid1) =>
                    {
                        audioSvc.PlayUIAudio(Constant.UIFBClick);
                        timeSvc.AddTimeTask((int tid2) =>
                        {
                            audioSvc.PlayUIAudio(Constant.UIFBClick);
                            timeSvc.AddTimeTask((int tid3) =>
                            {
                                audioSvc.PlayUIAudio(Constant.UIFBClick);
                                timeSvc.AddTimeTask((int tid4) =>
                                {
                                    audioSvc.PlayUIAudio(Constant.FBWin);
                                }, 300);
                            }, 270);
                        },270);
                    }, 325);
                },1000);
                break;
            case FBEndType.Lose:
                SetActive(centerPin, false);
                SetActive(closeBtn.gameObject, false);
                SetActive(exitBtn.gameObject);
                audioSvc.PlayUIAudio(Constant.FBLose);
                break;
            default:
                break;
        }
    }

    public void RefreshWinInfo(int fbid,int costTime,int restHp)
    {
        MapCfg mapCfg = resSvc.GetMapCfg(fbid);
        txtReward.text = "关卡奖励：" + mapCfg.exp + "经验 " + mapCfg.coin + "金币 " + mapCfg.crystal + "水晶";
        txtRestHp.text = "剩余血量：" + restHp;

        int second = costTime / 1000;
        int min = second / 60;
        second =  second % 60;
        if(min >= 10 )
        {
            txtTime.text = "通关时间：" + min + "：" ;
        }
        else
        {
            txtTime.text = "通关时间：0" + min + "：";
        }
        if(second >= 10)
        {
            txtTime.text += second;
        }
        else
        {
            txtTime.text += "0" + second;
        }
        

    }

    public void SetWindType(FBEndType endType)
    {
        this.endType = endType;
    }

    public void OnExitBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.FBLose);
        BattleSys.Instance.battleMgr.isPauseGame = false;

        //进入主城 销毁战斗
        BattleSys.Instance.DestroyBattle();
        MainCitySys.Instance.EnterMainCity();

        SetWindowState(false);
    }

    public void OnCloseBtnClose()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        BattleSys.Instance.battleMgr.isPauseGame = false;
        if (gameObject.activeInHierarchy)
        {
            SetWindowState(false);
        }
    }

    public void OnSureBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        //进入主城，销毁战斗
        BattleSys.Instance.DestroyBattle();
        MainCitySys.Instance.EnterMainCity();
        SetWindowState(false);
        //激活下一关
    }


}

public enum FBEndType
{
    None,
    Pause,
    Win,
    Lose,
}