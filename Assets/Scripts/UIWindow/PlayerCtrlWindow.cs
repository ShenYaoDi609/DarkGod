/****************************************************
	文件：PlayerCtrlWindow.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/25 14:24   	
	功能：主角战斗控制窗口
*****************************************************/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PEProtocol;

public class PlayerCtrlWindow:WindowRoot
{
    //LeftTopPin
    public Image imgHp;
    public Text txtHp;
    public Image imgEnergy;
    public Button btnHead;
    public Button btnLv;
    public Text txtLv;
    //RightBottom
    public Button btnNormal;
    public Button btnSkill1;
    public Button btnSkill2;
    public Button btnSkill3;
    //Bottom
    public Text txtExp;
    public Transform expItemListTrans;
    //LeftBottom 
    public Image imgTouch;
    public Image imgDirBg;
    public Image imgDirPoint;
    //Center
    public Text txtName;

    private Vector2 startPos = Vector2.zero;
    public Vector2 currentDir = Vector2.zero;
    private float pointDis = 0;
    private int hpSum = 0;

    protected override void InitWindow()
    {
        base.InitWindow();

        pointDis = Screen.height * 1.0f / Constant.ScreenStandardHeight * Constant.TouchDirOpDis;
        SetActive(imgDirPoint, false);
        SetBossHpState(false);
        RefreshUI();
        RegisterMouseEvent();

        sk1CdTime = resSvc.GetSkillCfg(Constant.Skill1ID).cdTime/1000;
        sk2CdTime = resSvc.GetSkillCfg(Constant.Skill2ID).cdTime / 1000;
        sk3CdTime = resSvc.GetSkillCfg(Constant.Skill3ID).cdTime / 1000;

        btnNormal.onClick.AddListener(OnNormalAtkBtnClick);
        btnSkill1.onClick.AddListener(OnSkill1BtnClick);
        btnSkill2.onClick.AddListener(OnSkill2BtnClick);
        btnSkill3.onClick.AddListener(OnSkill3BtnClick);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnSkill1BtnClick();
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            OnSkill2BtnClick();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            OnSkill3BtnClick();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            OnNormalAtkBtnClick();
        }

        if (isSk1Cd)
        {
            curSk1Cd -= Time.deltaTime;
            sk1CdImg.fillAmount = curSk1Cd / sk1CdTime;
            SetText(sk1CdTxt, (int)curSk1Cd);
        }

        if(isSk2Cd)
        {
            curSk2Cd -= Time.deltaTime;
            sk2CdImg.fillAmount = curSk2Cd / sk2CdTime;
            SetText(sk2CdTxt, (int)curSk2Cd);
        }

        if(isSk3Cd)
        {
            curSk3Cd -= Time.deltaTime;
            sk3CdImg.fillAmount = curSk3Cd / sk3CdTime;
            SetText(sk3CdTxt, (int)curSk3Cd);
        }

        if(bossHpTrans.gameObject.activeInHierarchy)
        {
            BlendBossHp();
            imgHpYellow.fillAmount = currentPrg;
        }

    }

    #region About UI
    public void RefreshUI()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        //血量
        //int hpLimit = PECommon.GetHpByLv(playerData.lv);
        hpSum = playerData.hp;
        SetSelfHpBar(hpSum);
        SetText(txtName, playerData.name);
        //等级
        SetText(txtLv, playerData.lv);
        //能量
        imgEnergy.fillAmount = playerData.energy * 1.0f / 100;
        //进度条
        int expLimit = PECommon.GetExpLimitByLv(playerData.lv);
        SetText(txtExp, Mathf.Round((playerData.exp * 1.0f / expLimit) * 100.0f).ToString() + "%");
        SetExpProgressBar(playerData.exp);
        AdaptProgressBar();
        
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

    public void SetExpProgressBar(int exp)
    {
        int expLimit = PECommon.GetExpLimitByLv(GameRoot.Instance.PlayerData.lv);
        int fullItem = exp * 10 / expLimit;
        for (int i = 0; i < expItemListTrans.childCount; i++)
        {
            Image img = expItemListTrans.GetChild(i).GetComponent<Image>();
            if (i < fullItem)
            {
                img.fillAmount = 1;
            }
            else if (i == fullItem)
            {
                img.fillAmount = ((exp * 1.0f) - expLimit * fullItem * 1.0f / 10) / 100;
            }
            else
            {
                img.fillAmount = 0;
            }
        }
    }

    public void SetSelfHpBar(int hp)
    {
        SetText(txtHp, hp + "/" + hpSum);
        imgHp.fillAmount = hp * 1.0f / hpSum;
    }
    #endregion

    #region Register MouseEvent
    public void RegisterMouseEvent()
    {
        OnClickDown(imgTouch.gameObject, (PointerEventData eventData) =>
         {
             imgDirBg.transform.position = eventData.position;
             SetActive(imgDirPoint, true);
             startPos = eventData.position;
         });

        OnClickUp(imgTouch.gameObject, (PointerEventData eventData) =>
         {
             imgDirBg.transform.localPosition = Vector2.zero;
             SetActive(imgDirPoint, false);
             imgDirPoint.transform.localPosition = Vector2.zero;
             currentDir = Vector2.zero;
             BattleSys.Instance.SetPlayerMoveDir(Vector2.zero);
         });
        OnDrag(imgTouch.gameObject, (PointerEventData eventData) =>
        {
            Vector2 dir = eventData.position - startPos;
            float len = dir.magnitude;
            if (len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = startPos + dir;
            }
            currentDir = dir.normalized;
            //传递方向信息
            BattleSys.Instance.SetPlayerMoveDir(currentDir);
        });
    }
    #endregion

    #region Button Event
    public void OnNormalAtkBtnClick()
    {
        BattleSys.Instance.ReqReleaseSkill(0);
    }

    public Image sk1CdImg;
    public Text sk1CdTxt;
    private bool isSk1Cd = false;
    private float sk1CdTime;
    private float curSk1Cd;

    public void OnSkill1BtnClick()
    {
        if(isSk1Cd == false && CanReleaseSkill())
        {
            isSk1Cd = true;
            BattleSys.Instance.ReqReleaseSkill(1);
            sk1CdImg.fillAmount = 1;
            SetActive(sk1CdImg,true);
            curSk1Cd = sk1CdTime;
            SetText(sk1CdTxt, (int)curSk1Cd);
            TimerSvc.Instance.AddTimeTask((int tid) =>
            {
                isSk1Cd = false;
                SetActive(sk1CdImg, false);
            }, sk1CdTime * 1000);
        }        
    }

    public Image sk2CdImg;
    public Text sk2CdTxt;
    private bool isSk2Cd = false;
    private float sk2CdTime;
    private float curSk2Cd;

    public void OnSkill2BtnClick()
    {
        if(isSk2Cd == false && CanReleaseSkill())
        {
            isSk2Cd = true;
            BattleSys.Instance.ReqReleaseSkill(2);
            SetActive(sk2CdImg, true);
            sk2CdImg.fillAmount = 1;
            curSk2Cd = sk2CdTime;
            SetText(sk2CdTxt, (int)curSk2Cd);
            TimerSvc.Instance.AddTimeTask((int tid) =>
            {
                isSk2Cd = false;
                SetActive(sk2CdImg, false);
            }, sk2CdTime * 1000);
        }
     
    }

    public Image sk3CdImg;
    public Text sk3CdTxt;
    private bool isSk3Cd = false;
    private float sk3CdTime;
    private float curSk3Cd;

    public void OnSkill3BtnClick()
    {
        if (isSk3Cd == false && CanReleaseSkill())
        {
            isSk3Cd = true;
            BattleSys.Instance.ReqReleaseSkill(3);
            SetActive(sk3CdImg, true);
            sk3CdImg.fillAmount = 1;
            curSk3Cd = sk3CdTime;
            SetText(sk3CdTxt, (int)curSk3Cd);
            timeSvc.AddTimeTask((int tid) =>
            {
                isSk3Cd = false;
                SetActive(sk3CdImg, false);
            }, sk3CdTime * 1000);
        }
        
    }

    public void OnHeadBtnClick()
    {
        BattleSys.Instance.battleMgr.isPauseGame = true;
        BattleSys.Instance.SetBattleEndWindow(FBEndType.Pause,true);
    }
    #endregion


    #region Boss Hp
    public Transform bossHpTrans;
    public Image imgHpRed;
    public Image imgHpYellow;
    public Text txtBoss;
    private float currentPrg = 1.0f;
    private float targetPrg = 1.0f;

    public void SetBossHpState(bool isActive = true)
    {
        bossHpTrans.gameObject.SetActive(isActive);
    }

    public void SetBossHp(int oldHp, int newHp,int hpSum)
    {
        currentPrg = oldHp * 1.0f / hpSum;
        targetPrg = newHp * 1.0f / hpSum;
        imgHpRed.fillAmount = targetPrg;
    }

    private void BlendBossHp()
    {
        if (Mathf.Abs(targetPrg - currentPrg) < Constant.smoothHpSpeed * Time.deltaTime)
        {
            currentPrg = targetPrg;
        }
        else if(currentPrg > targetPrg)
        {
            currentPrg -= Constant.smoothHpSpeed * Time.deltaTime;
        }
        else
        {
            currentPrg += Constant.smoothHpSpeed * Time.deltaTime;
        }
    }
    #endregion
    private bool CanReleaseSkill()
    {
        return BattleSys.Instance.GetEntityPlayer().canReleaseSkill;
    }
}

