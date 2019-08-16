/****************************************************
    文件：Constant.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/2 17:53:42
	功能：常量配置
*****************************************************/

using UnityEngine;

public class Constant : MonoBehaviour 
{
    //字体颜色
    public const string ColorRed = "<color=#FF0000FF>";
    public const string ColorGreen = "<color=#00FF00FF>";
    public const string ColorBlue = "<color=#0000FFFF>";
    public const string ColorYellow = "<color=#FFFF00FF>";
    public const string ColorEnd = "</color>";

    public static string GetColoredString(string msg,string color)
    {
        return color + msg + ColorEnd;
    }

    //场景名称
    public const string SceneLogin = "SceneLogin";
    public const string SceneMainCity = "SceneMainCity";
    public const string SceneOrge = "SceneOrge";

    //场景ID
    public const int SceneMainCityID = 10000;
    public const int SceneOrgeID = 10001;

    //背景音乐
    public const string BGLogin = "bgLogin";
    public const string BGMainCity = "bgMainCity";
    public const string BGOrge = "bgHuangYe";

    //登录按钮点击音效
    public const string UILoginEnter = "uiLoginBtn";

    //普通UI点击音效
    public const string UICommonClick = "uiClickBtn";
    //角色受击音效
    public const string AssassinHit = "assassin_Hit";

    //MainMenu点击音效
    public const string UIMainMenuClick = "uiExtenBtn";
    public const string UIFBClick = "fbitem";
    public const string UIOpenPageClick = "uiOpenPage";

    public const string FBLose = "fblose";
    public const string FBWin = "fbwin";

    //标准屏幕宽高
    public const int ScreenStandardWidth = 1334;
    public const int ScreenStandardHeight = 750;

    public const int TouchDirOpDis = 90;

    //动画名称
    public const string MainMenuClose = "MainMenuClose";
    public const string MainMenuOpen = "MainMenuOpen";
    public const string RoleInfoWindowOpen = "RoleInfoWindowOpen";
    public const string RoleInfoWindowClose = "RoleInfoWindowClose";

    //移动速度
    public const int PlayerMoveSpeed = 8;
    public const int MonsterMoveSpeed = 3;

    //平滑速度
    public const float smoothSpeed = 5;
    public const float smoothHpSpeed = 0.3f;

    //动画名称
    public const string Blend = "Blend";
    public const int BlendIdle = 0;
    public const int BlendMove = 1;

    //动画参数
    public const int ActionSkill1 = 1;
    public const int ActionSkill2 = 2;
    public const int ActionSkill3 = 3;

    public const int ActionAtk1 = 11;
    public const int ActionAtk2 = 12;
    public const int ActionAtk3 = 13;
    public const int ActionAtk4 = 14;
    public const int ActionAtk5 = 15;
    
    public const int ActionDefault = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 50;

    public const int DieAniLength = 5000;

    //npcID
    public const int NPCNumber = 4;
    public const int NPCDefaultID = -1;
    public const int NPCWisemanID = 0;
    public const int NPCGeneralID = 1;
    public const int NPCArtisanID = 2;
    public const int NPCTraderID = 3;

    //SkillID
    public const int Skill1ID = 101;
    public const int Skill2ID = 102;
    public const int Skill3ID = 103;
    public const int Atk1ID = 111;
    public const int Atk2ID = 112;
    public const int Atk3ID = 113;
    public const int Atk4ID = 114;
    public const int Atk5ID = 115;

    //普攻连招有效间隔
    public const int interval = 500;

}

public enum EntityType
{
    None,
    Player,
    Monster,
}

public enum EntityState
{
    None,
    //霸体状态：
    //可以受到伤害但不被控制
    StoicState,
}

public enum MonsterType
{
    None,
    Normal = 1,
    Boss = 2,
}