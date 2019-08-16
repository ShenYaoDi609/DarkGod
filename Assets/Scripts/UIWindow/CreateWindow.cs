/****************************************************
    文件：CreateWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/3 8:27:17
	功能：角色创建界面
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class CreateWindow : WindowRoot 
{
    public Text characterName;
    public Text characterDes;
    public Image characterImage;
    public Button renameBtn;
    public Button enterBtn;
    public InputField nameInput;


    protected override void InitWindow()
    {
        base.InitWindow();

        renameBtn.onClick.AddListener(OnRenameButtonClick);
        enterBtn.onClick.AddListener(OnEnterButtonClick);

        SetText(characterName, "暗夜刺客");
        SetText(characterDes, "拥有性感的躯壳却掩饰不了黑暗与冷酷，善用匕首和月刃，可以杀人于无形之中");
        nameInput.text = resSvc.GetRandomName(false);

    }

    public void OnRenameButtonClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        //随机名字
        string newName = resSvc.GetRandomName(false);
        nameInput.text = newName;
    }

    public void OnEnterButtonClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);

        if(nameInput  != null)
        {
            //发送名字数据到服务器
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = nameInput.text
                }
            };
            netSvc.SendMsg(msg);
        }
        else
        {
            GameRoot.AddTipsToQueue("你输入的名字不合法 ");
        }
    }
}