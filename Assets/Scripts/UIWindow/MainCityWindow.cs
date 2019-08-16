/****************************************************
    文件：MainCityWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/12 22:11:31
	功能：主城界面
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using UnityEngine.EventSystems;

public class MainCityWindow : WindowRoot 
{
    #region UIDefine
    //LeftTop
    public Button enhanceFightBtn;
    public Text txtFight;
    public Button iconBtn;
    public Text txtStamina;
    public Button buyStaminaBtn;
    public Image sliderStamina;
    public Text txtLevel;
    //RightTop
    public Button guideBtn;
    public Button rechargeBtn;
    public Button vipBtn;
    public Text txtVip;
    //RightBottom
    public Button menuBtn;
    public Button arenaBtn;
    public Button taskBtn;
    public Button mkcoinBtn;
    public Button strongBtn;
    //Center
    public Text txtName;
    //Bottom
    public Text txtExp;
    public Button chatBtn;
    //LeftBottom 
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    #endregion
    public Transform expItemListTrans;

    public Animation anim;
    public bool isMainMenuOpen = true;

    private Vector2 startPos = Vector2.zero;
    private float pointDis = 0;

    public AutoGuideCfg curTaskCfg;
    

    protected override void InitWindow()
    {
        base.InitWindow();

        pointDis = Screen.height * 1.0f / Constant.ScreenStandardHeight * Constant.TouchDirOpDis;
        SetActive(imgDirPoint, false);
        RefreshUI();
        RegisterMouseEvent();

        enhanceFightBtn.onClick.AddListener(OnEnhanceFightBtnClick);
        iconBtn.onClick.AddListener(OnIconBtnClick);
        buyStaminaBtn.onClick.AddListener(OnBuyStaminaBtnClick);
        guideBtn.onClick.AddListener(OnGuideBtnClick);
        rechargeBtn.onClick.AddListener(OnRechargeBtnClick);
        vipBtn.onClick.AddListener(OnVipBtnClick);
        menuBtn.onClick.AddListener(OnMenuBtnClick);
        taskBtn.onClick.AddListener(OnTaskBtnClick);
        strongBtn.onClick.AddListener(OnStrongBtnClick);
        arenaBtn.onClick.AddListener(OnArenaBtnClick);
        mkcoinBtn.onClick.AddListener(OnMkCoinBtnClick);
        chatBtn.onClick.AddListener(OnChatBtnClick);

    }
    

    /// <summary>
    /// 更新UI
    /// </summary>
    public void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        //计算战斗力并更新
        int fight = PECommon.GetFightByProps(playerData);
        SetText(txtFight, fight);
        //更新体力UI
        int staminaLimit = PECommon.GetStaminaLimitByLv(playerData.lv);
        SetText(txtStamina, "体力:" + playerData.stamina + "/" + staminaLimit);
        sliderStamina.fillAmount = playerData.stamina * 1.0f / staminaLimit;
        //更新等级UI
        SetText(txtLevel, playerData.lv);
        //更新名字
        SetText(txtName, playerData.name);
        //更新进度条UI
        int expLimit = PECommon.GetExpLimitByLv(playerData.lv);
        SetText(txtExp, Mathf.Round((playerData.exp * 1.0f / expLimit) * 100.0f).ToString() + "%");
        SetExpProgressBar(playerData.exp);

        AdaptProgressBar();

        #region TaskIconRefresh
        curTaskCfg = resSvc.GetTaskCfg(playerData.taskid);
        if(curTaskCfg != null)
        {
            SetAutoTaskIcon(curTaskCfg.npcID);
        }
        //如果当前没有任务,显示默认图片
        else
        {
            SetAutoTaskIcon(-1);
        }
        #endregion
        //TODO
    }

    public void SetAutoTaskIcon(int npcID)
    {
        string imgPath = "";
        Image image = guideBtn.GetComponent<Image>();
        switch(npcID)
        {
            case Constant.NPCDefaultID:
                imgPath = PathDefine.AutoTaskDefault;
                break;
            case Constant.NPCWisemanID:
                imgPath = PathDefine.AutoTaskWiseMan;
                break;
            case Constant.NPCGeneralID:
                imgPath = PathDefine.AutoTaskGeneral;
                break;
            case Constant.NPCArtisanID:
                imgPath = PathDefine.AutoTaskArtisan;
                break;
            case Constant.NPCTraderID:
                imgPath = PathDefine.AutoTaskTrader;
                break;
        }
        SetImage(image, imgPath);
    }

    public void AdaptProgressBar()
    {
        //不同分辨率下适配进度条
        GridLayoutGroup grid = expItemListTrans.GetComponent<GridLayoutGroup>();
        //获取当前缩放比
        float aspectRadio = 1.0f * Constant.ScreenStandardHeight / Screen.height;
        //获取当前屏幕宽度
        float screenWidth = Screen.width * aspectRadio;
        //计算grid的cellsize
        float cellSizeX = (screenWidth - 148) / 10;
        grid.cellSize = new Vector2(cellSizeX, 10);
    }

    /// <summary>
    /// 设置进度条进度
    /// </summary>
    public void SetExpProgressBar(int exp)
    {
        int expLimit = PECommon.GetExpLimitByLv(GameRoot.Instance.PlayerData.lv);
        int fullItem = exp * 10 / expLimit ;
        for(int i = 0; i < expItemListTrans.childCount; i++)
        {
            Image img = expItemListTrans.GetChild(i).GetComponent<Image>();
            if (i < fullItem)
            {
                img.fillAmount = 1;
            }
            else if(i == fullItem)
            {
                img.fillAmount = ((exp * 1.0f) - expLimit * fullItem * 1.0f /10 ) / 100;
            }
            else
            {
                img.fillAmount = 0;
            }      
        }
    }

    /// <summary>
    /// 注册鼠标事件
    /// </summary>
    public void RegisterMouseEvent()
    {
        OnClickDown(imgTouch.gameObject, (PointerEventData eventData) =>
         {
             imgDirBg.transform.position = eventData.position;
             SetActive(imgDirPoint, true);
             startPos = eventData.position;
         });

        OnClickUp(imgTouch.gameObject, (PointerEventData eventData) => {
            imgDirBg.transform.localPosition = Vector2.zero;
            SetActive(imgDirPoint, false);
            imgDirPoint.transform.localPosition = Vector2.zero;
            //松开时方向归0
            MainCitySys.Instance.SetMoveDir(Vector2.zero);
        });

        OnDrag(imgTouch.gameObject, (PointerEventData eventData) =>
         {
             Vector2 dir = eventData.position - startPos;
             float len = dir.magnitude;
             if(len > pointDis)
             {
                 Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                 imgDirPoint.transform.position = startPos + clampDir;
             }
             else
             {
                 imgDirPoint.transform.position = startPos + dir;
             }
             //传递方向信息
             MainCitySys.Instance.SetMoveDir(dir.normalized);
         });
    }

    #region Button Click Event
    public void OnChatBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIOpenPageClick);
        MainCitySys.Instance.OpenChatWindow();
    }

    public void OnEnhanceFightBtnClick()
    {
        //TODO
    }

    public void OnIconBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIOpenPageClick);
        MainCitySys.Instance.OpenRoleInfoWindow();
        //TODO
    }

    public void OnBuyStaminaBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIOpenPageClick);
        MainCitySys.Instance.OpenPurchaseWindow(0);
    }

    public void OnGuideBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        if(curTaskCfg != null)
        {
            //执行引导任务
            MainCitySys.Instance.RunTask(curTaskCfg);
        }
        else
        {
            //弹出tips
            PECommon.Log("任务已完成，更多任务正在开发中.....");
        }
        //TODO
    }

    public void OnRechargeBtnClick()
    {
        //TODO
    }

    public void OnVipBtnClick()
    {
        //TODO
    }

    public void OnMenuBtnClick()
    {
        AnimationClip clip = null;
        if(isMainMenuOpen)
        {
            isMainMenuOpen = false;
            clip = anim.GetClip(Constant.MainMenuClose) ;
        }
        else
        {
            isMainMenuOpen = true;
            clip = anim.GetClip(Constant.MainMenuOpen);
        }
        anim.clip = clip;
        anim.Play();
        audioSvc.PlayUIAudio(Constant.UIMainMenuClick);
        //TODO
    }

    public void OnTaskBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIOpenPageClick);
        MainCitySys.Instance.OpenTaskWindow();
    }

    public void OnStrongBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIOpenPageClick);
        MainCitySys.Instance.OpenStrongWindow();
    }

    public void OnArenaBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIFBClick);
        MainCitySys.Instance.EnterFuBen();
    }

    public void OnMkCoinBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIOpenPageClick);
        MainCitySys.Instance.OpenPurchaseWindow(1);
    }
    #endregion
}

