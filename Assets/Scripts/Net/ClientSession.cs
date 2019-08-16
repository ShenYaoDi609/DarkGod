/****************************************************
    文件：ClientSession.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/5 19:51:14
	功能：网络会话连接（客户端）
*****************************************************/

using PEProtocol;
using PENet;

public class ClientSession :  PESession<GameMsg>
{
    protected override void OnConnected()
    {
        GameRoot.AddTipsToQueue("连接服务器成功");
        PECommon.Log("Client Connected To Server Succ");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("RcvPack CMD:" + ((CMD)msg.cmd).ToString());
        NetSvc.Instance.AddMsgQue(msg);
    }

    protected override void OnDisConnected()
    {
        GameRoot.AddTipsToQueue("服务器断开连接");
        PECommon.Log("Client DisConnected To Server");
    }
}