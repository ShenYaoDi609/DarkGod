/****************************************************
    文件：LoginWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/2 18:54:0
	功能：登录界面逻辑
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class LoginWindow : WindowRoot 
{
    public InputField idInput;
    public InputField pwInput;
    public Button enterGameBtn;
    public Button noticeBtn;


    protected override void InitWindow()
    {
        base.InitWindow();

        enterGameBtn.onClick.AddListener(OnEnterButtonClick);
        noticeBtn.onClick.AddListener(OnNoticButtonClick);

       //获取本地存储的账号密码
       if(PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("PW"))
        {
            idInput.text = PlayerPrefs.GetString("Acct");
            pwInput.text = PlayerPrefs.GetString("PW");
        }
       else
        {
            idInput.text = "";
            pwInput.text = "";
        }
    }

    /// <summary>
    /// 进入游戏按钮点击事件
    /// </summary>
    public void  OnEnterButtonClick()
    {
        //播放按钮点击音效
        audioSvc.PlayUIAudio(Constant.UILoginEnter);

        string acct = idInput.text;
        string passwd = pwInput.text;
        if(acct != "" && passwd != "")
        {
            PlayerPrefs.SetString("Acct", acct);
            PlayerPrefs.SetString("PW", passwd);

            //发送网络消息 请求登录
            GameMsg msg = new GameMsg {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin
                {
                    acct = acct,
                    pwd = passwd
                }
            };
            netSvc.SendMsg(msg);  
            //LoginSys.Instance.RspLogin();
        }
        else
        {
            GameRoot.AddTipsToQueue("账号或 密码为空");
        }
    }

    public void OnNoticButtonClick()
    {
        GameRoot.AddTipsToQueue("公告按钮正在开发中......");
        //按钮点击音效
        audioSvc.PlayUIAudio(Constant.UICommonClick);
    }
}