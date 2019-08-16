/****************************************************
    文件：RoleInfoWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/24 23:20:45
	功能：Nothing
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using UnityEngine.EventSystems;

public class RoleInfoWindow : WindowRoot 
{
    #region UIDefine
    public RawImage characterModel;

    public Image characterBg;

    public Text txtNameLv;
    public Image expFill;
    public Text txtExp;
    public Image staminaFill;
    public Text txtStamina;
    public Text txtProfession;
    public Text txtFight;
    public Text txtHp;
    public Text txtHurt;
    public Text txtDefence;

    public Button closeBtn;
    public Button detailInfoBtn;
    public Button detailCloseBtn;

    public Transform detailWindow;

    public Text txthp;
    public Text txtad;
    public Text txtap;
    public Text txtaddef;
    public Text txtapdef;
    public Text txtdodge;
    public Text txtpierce;
    public Text txtcritical;

    public Animation anim;

    #endregion

    //记录鼠标开始转动模型的位置
    private Vector2 startPos;

    protected override void InitWindow()
    {
        base.InitWindow();
        SetActive(detailWindow, false);
        Debug.Log(audioSvc);
        RefreshUI();
        RegisterMouseEvent();

        closeBtn.onClick.AddListener(OnCloseBtnClick);
        detailInfoBtn.onClick.AddListener(OnDetailInfoBtnClick);
        detailCloseBtn.onClick.AddListener(OnDetailCloseBtnClick);
    }

    protected override void ClearWindow()
    {
        base.ClearWindow();
        //因为人物信息窗口要频繁开关，在关闭时清空按钮事件，防止重复注册
        closeBtn.onClick.RemoveAllListeners();
    }


    public void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(txtNameLv, playerData.name + " LV" + playerData.lv);

        int staminaLimit = PECommon.GetStaminaLimitByLv(playerData.lv);
        SetText(txtStamina, playerData.stamina + "/" + staminaLimit);
        float percentStamina = playerData.stamina * 1.0f / staminaLimit;
        staminaFill.fillAmount = percentStamina;

        int expLimit = PECommon.GetExpLimitByLv(playerData.lv);
        SetText(txtExp, playerData.exp + "/" + expLimit);
        float percentExp = playerData.exp * 1.0f / expLimit;
        expFill.fillAmount = percentExp;

        SetText(txtProfession, "职业      暗夜刺客");

        int fight = PECommon.GetFightByProps(playerData);
        SetText(txtFight, "战力         " + fight);
        SetText(txtHp, "生命        " + playerData.hp);
        SetText(txtHurt, "伤害         " + (playerData.ad + playerData.ap).ToString());
        SetText(txtDefence, "防御         " + (playerData.addef + playerData.apdef).ToString());

        SetText(txthp, playerData.hp);
        SetText(txtad, playerData.ad);
        SetText(txtap, playerData.ap);
        SetText(txtaddef, playerData.addef);
        SetText(txtapdef, playerData.apdef);
        SetText(txtdodge, playerData.dodge + "%");
        SetText(txtpierce, playerData.pierce + "%");
        SetText(txtcritical, playerData.critical + "%");

    }

    /// <summary>
    /// 注册鼠标事件，鼠标移动旋转模型
    /// </summary>
    public void RegisterMouseEvent()
    {
        OnClickDown(characterBg.gameObject, (PointerEventData eventData) =>
         {
             startPos = eventData.position;
             MainCitySys.Instance.SetStartRotate();
         });
        OnDrag(characterBg.gameObject, (PointerEventData eventData) =>
         {
             //旋转角度
             float rotate = -(eventData.position.x - startPos.x) * 0.4f;
             MainCitySys.Instance.ChangeModelRotate(rotate);
         });


    }


    public void OnCloseBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        MainCitySys.Instance.CloseRoleInfoWindow();
    }

    public void OnDetailInfoBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UIOpenPageClick);
        SetActive(detailWindow, true);
    }

    public void OnDetailCloseBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        SetActive(detailWindow, false);
    }

    //public void PlayAnimation()
    //{
    //    AnimationClip clip = null;
    //    if(gameObject.activeInHierarchy == false)
    //    {
    //        clip = anim.GetClip(Constant.RoleInfoWindowOpen);
    //    }
    //    if(clip != null)
    //    {
    //        anim.clip = clip;
    //        anim.Play();
    //    }        
    //}
}