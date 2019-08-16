/****************************************************
    文件：ChatWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/16 9:21:40
	功能：聊天界面
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using UnityEngine.EventSystems;

public class ChatWindow : WindowRoot
{
    #region UIDefine
    //Button
    public Button worldBtn;
    public Button laborBtn;
    public Button friendBtn;
    public Button closeBtn;
    public Button sendBtn;
    //Text
    public Text chatTxt;
    //InputField
    public InputField inputTxt;
    //Image
    public Image worldImg;
    public Image laborImg;
    public Image friendImg;
    #endregion

    private int chatType;
    //存储消息
    private List<string> chatList = new List<string>();

    protected override void InitWindow()
    {
        base.InitWindow();
        chatType = 0;

        RefreshUI();

        worldBtn.onClick.AddListener(OnWorldBtnClick);
        laborBtn.onClick.AddListener(OnLaborBtnClick);
        friendBtn.onClick.AddListener(OnFriendBtnClick);
        closeBtn.onClick.AddListener(OnCloseBtnClick);
        sendBtn.onClick.AddListener(OnSendBtnClick);
    }

    protected override void ClearWindow()
    {
        base.ClearWindow();
        closeBtn.onClick.RemoveAllListeners();
    }

    public void RefreshUI()
    {     
        if (chatType == 0)
        {
            string chatMsg = "";
            for (int i = 0; i < chatList.Count; i++)
            {
                chatMsg += chatList[i] + "\n";
            }
            SetText(chatTxt, chatMsg);
            SetImage(worldImg, PathDefine.ImageSelect);
            SetImage(laborImg, PathDefine.ImageNotSelect);
            SetImage(friendImg, PathDefine.ImageNotSelect);
        }
        else if(chatType == 1)
        {
            SetText(chatTxt, "尚未加入公会");
            SetImage(worldImg, PathDefine.ImageNotSelect);
            SetImage(laborImg, PathDefine.ImageSelect);
            SetImage(friendImg, PathDefine.ImageNotSelect);
        }
        else if(chatType == 2)
        {
            SetText(chatTxt, "暂无好友信息");
            SetImage(worldImg, PathDefine.ImageNotSelect);
            SetImage(laborImg, PathDefine.ImageNotSelect);
            SetImage(friendImg, PathDefine.ImageSelect);
        }

    }

    #region Button Click Event
    public void OnCloseBtnClick()
    {
        chatType = 0;
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        MainCitySys.Instance.CloseChatWindow();
    }

    public void OnWorldBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        chatType = 0;
        RefreshUI();
    }

    public void OnLaborBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        chatType = 1;
        RefreshUI();
    }

    public void OnFriendBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        chatType = 2;
        RefreshUI();
    }


    private bool canSend = true;
    public void OnSendBtnClick()
    {
        if (!canSend)
        {
            GameRoot.AddTipsToQueue("聊天消息每5秒才能发送一条");
            return;
        }
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        //string chatMsg = Constant.GetColoredString(GameRoot.Instance.PlayerData.name, Constant.ColorBlue) + "：" +inputTxt.text;
        if(inputTxt.text != "")
        {
            if(inputTxt.text.Length > 12 )
            {
                GameRoot.AddTipsToQueue("输入信息不能超过12个字");
            }
            else
            {
                //发送消息到服务器
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.SndChat,
                    sndChat = new SndChat
                    {
                        msg = inputTxt.text
                    }
                };
                inputTxt.text = "";
                netSvc.SendMsg(msg);
                canSend = false;

                timeSvc.AddTimeTask((int timeID) =>
                {
                    canSend = true;
                }, 5,PETimeUnit.Second,0);
            }
        }
        else
        {
            GameRoot.AddTipsToQueue("发送内容不能为空");
        }
       
    }

    public void AddChatMsg(string name,string msg)
    {
        string chatMsg = Constant.GetColoredString(name, Constant.ColorBlue) + "：" + msg;
        chatList.Add(chatMsg);
        if(chatList.Count > 12)
        {
            chatList.RemoveAt(0);
        }
        if(gameObject.activeInHierarchy)
        {
            RefreshUI();
        }        
    }
    #endregion
}