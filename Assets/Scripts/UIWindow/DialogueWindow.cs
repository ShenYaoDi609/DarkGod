/****************************************************
    文件：DialogueWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/4 16:30:1
	功能：Nothing
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using UnityEngine.EventSystems;

public class DialogueWindow : WindowRoot 
{
    public Text txtName;
    public Text txtDialogue;
    public Image icon;
    public Button nextBtn;

    private AutoGuideCfg curAutoGuideCfg;
    private int index;

    protected override void InitWindow()
    {
        base.InitWindow();
        index = 1;
        curAutoGuideCfg = MainCitySys.Instance.GetCurAutoGuideCfg();
        SetDialogue();

        nextBtn.onClick.AddListener(OnNextBtnClick);
    }

    private void SetDialogue()
    {
        Sprite iconSprite = null;
        string name = null;
        //将对话和代表icon的id分割开来
        string[] dialogue = curAutoGuideCfg.dilogArr[index].Split('|');
        dialogue[1] = dialogue[1].Replace("$name", GameRoot.Instance.PlayerData.name);
        if(dialogue[0].Equals("0"))
        {
            //设置图标
            iconSprite = resSvc.LoadSprite(PathDefine.DialogueIconPlayer);
            //设置名字
            name = GameRoot.Instance.PlayerData.name;
        }
        else
        {
            switch(curAutoGuideCfg.npcID)
            {
                case -1:
                    iconSprite = resSvc.LoadSprite(PathDefine.DialogueIconGuide);
                    name = "精灵";
                    break;
                case 0:
                    iconSprite = resSvc.LoadSprite(PathDefine.DialogueIconWiseMan);
                    name = "智者";
                    break;
                case 1:
                    iconSprite = resSvc.LoadSprite(PathDefine.DialogueIconGeneral);
                    name = "将军";
                    break;
                case 2:
                    iconSprite = resSvc.LoadSprite(PathDefine.DialogueIconArtisan);
                    name = "工匠";
                    break;
                case 3:
                    iconSprite = resSvc.LoadSprite(PathDefine.DialogueIconTrader);
                    name = "商人";
                    break;
            }
        }
        if (iconSprite != null && name != null)
        {
            SetImage(icon, iconSprite);
            icon.SetNativeSize();
            SetText(txtName, name);
            SetText(txtDialogue, dialogue[1]);
        }
            
    }


    public void OnNextBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        if (index >= curAutoGuideCfg.dilogArr.Length - 1)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqTask,
                reqTask = new ReqTask
                {
                    taskID = curAutoGuideCfg.ID
                }
            };
            netSvc.SendMsg(msg);
            SetWindowState(false);
            nextBtn.onClick.RemoveAllListeners();
        }
        else
        {
            index++;
            SetDialogue();
        }
    }
}