/****************************************************
    文件：MainCitySys.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/12 22:32:27
	功能：主城业务 系统
*****************************************************/

using UnityEngine;
using UnityEngine.AI;
using PEProtocol;

public class MainCitySys : SystemRoot 
{
    #region 单例模式
    private static MainCitySys _instance = null;

    public static MainCitySys Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<MainCitySys>();
            }
            return _instance;
        }
    }
    #endregion

    public MainCityWindow mainCityWindow;
    public RoleInfoWindow roleInfoWindow;
    public DialogueWindow dialogueWindow;
    public StrongWindow strongWindow;
    public ChatWindow chatWindow;
    public PurchaseWindow purchaseWindow;
    public TaskWindow taskWindow;

    public AutoGuideCfg curAutoGuideCfg;

    private Transform[] npcTrans;

    private Transform cameraTrans;
    private Transform characterTrans;

    //RenderTexture相机
    private Transform characterCamTrans;

    private float startRotate;

    private PlayerController playerController;

    private NavMeshAgent agent;
    private CharacterController charController;

    private bool isGuiding = false;

    public override void InitSys()
    {
        base.InitSys();
       
        PECommon.Log("Init MainCitySys.....");
    }

    

    #region EnterMainCityInit Operation
    //进入主城
    public void EnterMainCity()
    {
        MapCfg mapCfg = resSvc.GetMapCfg(Constant.SceneMainCityID);
        resSvc.AsyncLoadScene(Constant.SceneMainCity, () =>
         {
             //设置主角
             SetCharacter(mapCfg);
             //设置摄像机
             SetCamera(mapCfg);
             InitNpcTransData();
             mainCityWindow.SetWindowState(true);
             GameRoot.Instance.GetComponent<AudioListener>().enabled = false;

             audioSvc.PlayBGMusic(Constant.BGMainCity, true);

             if(characterCamTrans != null)
             {
                 characterCamTrans.gameObject.SetActive(false);
             }
         });
    }

    public void SetCharacter(MapCfg mapCfg)
    {
        characterTrans = resSvc.LoadPrefab(PathDefine.AssassinCityPrefab, true).GetComponent<Transform>();
        characterTrans.position = mapCfg.playerBornPos;
        characterTrans.rotation = Quaternion.Euler(mapCfg.playerBornRote);
        characterTrans.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        //获取主角的CharacterController和NaveMeshAgent
        agent = characterTrans.gameObject.GetComponent<NavMeshAgent>();
        charController = characterTrans.gameObject.GetComponent<CharacterController>();
    }

    public void SetCamera(MapCfg mapCfg)
    {
        cameraTrans = Camera.main.transform;
        cameraTrans.position = mapCfg.mainCamPos;
        cameraTrans.rotation = Quaternion.Euler(mapCfg.mainCamRote);

        playerController = characterTrans.GetComponent<PlayerController>();
        playerController.Init();
    }

    /// <summary>
    /// 获取npc的transform信息
    /// </summary>
    public void InitNpcTransData()
    {
        npcTrans = new Transform[Constant.NPCNumber];
        for (int i = 0; i < Constant.NPCNumber; i++)
        {
            npcTrans[i] = GameObject.Find("NPC" + i).transform;
        }      
    }

    public void SetMoveDir(Vector2 dir)
    {
        StopGuide();
        playerController.Dir = dir;
    }
    #endregion

    #region StrongWindow Operation
    /// <summary>
    /// 打开强化界面
    /// </summary>
    public void OpenStrongWindow()
    {
        StopGuide();
        if(!strongWindow.gameObject.activeInHierarchy)
        {
            strongWindow.SetWindowState(true);
        }
    }
    
    /// <summary>
    /// 关闭强化界面
    /// </summary>
    public void CloseStrongWindow()
    {
        if(strongWindow.gameObject.activeInHierarchy)
        {
            strongWindow.SetWindowState(false);
        }
    }
    #endregion
  
    #region RoleInfoWindow Operation
    /// <summary>
    /// 打开人物信息界面
    /// </summary>
    public void OpenRoleInfoWindow()
    {
        StopGuide();
        //TODO
        if(characterCamTrans == null)
        {
            characterCamTrans = GameObject.FindGameObjectWithTag("CharacterShowCam").transform;
        }

        //设置人物展示相机相对于人物的位置
        characterCamTrans.localPosition = playerController.transform.position + playerController.transform.forward * 3.8f + new Vector3(0,1.5f,0);
        characterCamTrans.eulerAngles = new Vector3(0, 180 + characterTrans.eulerAngles.y, 0);
        characterCamTrans.localScale = Vector3.one;
        characterCamTrans.gameObject.SetActive(true);
        roleInfoWindow.SetWindowState(true);
        
    }

    /// <summary>
    /// 在人物移动且人物信息界面打开时更新相机位置
    /// </summary>
    public void UpdateCharShowCam()
    {
        if(characterCamTrans != null)
        {
            characterCamTrans.localPosition = playerController.transform.position + playerController.transform.forward * 3.8f + new Vector3(0, 1.5f, 0);
            characterCamTrans.eulerAngles = new Vector3(0, 180 + characterTrans.eulerAngles.y, 0);
        }       
    }
    /// <summary>
    /// 关闭人物信息界面
    /// </summary>
    public void CloseRoleInfoWindow()
    {
        if(characterCamTrans != null)
        {
            characterCamTrans.gameObject.SetActive(false);
            characterTrans.eulerAngles = new Vector3(0, startRotate, 0);
        }
        roleInfoWindow.SetWindowState(false);
    }

    public void SetStartRotate()
    {
        startRotate = playerController.transform.eulerAngles.y;
    }


    //旋转模型rotate角
    public void ChangeModelRotate(float rotate)
    {
        playerController.transform.eulerAngles = new Vector3(0, startRotate + rotate, 0);
    }
    #endregion

    #region AutoTask Operation
    public void RunTask(AutoGuideCfg AutoGuideCfg)
    {
        if(AutoGuideCfg != null)
        {
            curAutoGuideCfg = AutoGuideCfg;
        }

        agent.enabled = true;
        if(AutoGuideCfg.npcID != -1)
        {
            //寻路后打开对话界面
            //设置寻路目标
            Transform aimTrans = npcTrans[AutoGuideCfg.npcID];
            float distance = Vector3.Distance(aimTrans.position, characterTrans.position);
            if (distance > 0.5f)
            {
                StartGuide();
            }
            else
            {
                characterTrans.position = npcTrans[curAutoGuideCfg.npcID].position;
            }

        }
        else
        {
            //直接打开对话界面
            OpenDialogueWindow();
        }
    }

    /// <summary>
    /// 开始寻路
    /// </summary>
    public void StartGuide()
    {
        isGuiding = true;
        agent.enabled = true;
        charController.enabled = false;
        agent.speed = Constant.PlayerMoveSpeed;
        agent.SetDestination(npcTrans[curAutoGuideCfg.npcID].position);
        playerController.SetBlend(1);
    }

    /// <summary>
    /// 停止寻路
    /// </summary>
    public void StopGuide()
    {
        isGuiding = false;
        agent.enabled = false;
        charController.enabled = true;
        playerController.SetBlend(0);
    }
    

    /// <summary>
    /// 判断是否到达目标点
    /// </summary>
    public void IsArriveAim()
    {
        if(Vector3.Distance(npcTrans[curAutoGuideCfg.npcID].position, characterTrans.position) < 0.5f)
        {
            //设置为目标点位置
            characterTrans.position = npcTrans[curAutoGuideCfg.npcID].position;
            StopGuide();
            OpenDialogueWindow();
        }
    }

    /// <summary>
    /// 打开对话界面
    /// </summary>
    public void OpenDialogueWindow()
    {
        dialogueWindow.SetWindowState(true);
    }

    public AutoGuideCfg GetCurAutoGuideCfg()
    {
        return curAutoGuideCfg;
    }


    public void RspTask(GameMsg msg)
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        RspTask taskData = msg.rspTask;
       
        GameRoot.AddTipsToQueue("任务奖励 金币+" + curAutoGuideCfg.coin + " 经验+" + curAutoGuideCfg.exp,Constant.ColorRed);

        switch (curAutoGuideCfg.actID)
        {
            case 0:
                //与智者对话
                break;
            case 1:
                //进入副本
                EnterFuBen();
                break;
            case 2:
                //进入强化
                OpenStrongWindow();
                break;
            case 3:
                //进入体力购买
                OpenPurchaseWindow(0);
                break;
            case 4:
                //进入金币制造
                OpenPurchaseWindow(1);
                break;
            case 5:
                //进入世界聊天
                OpenChatWindow();
                break;

        }
        //更新信息
        GameRoot.Instance.SetPlayerDataByRspTask(taskData);
        //更新UI
        mainCityWindow.RefreshUI();
        roleInfoWindow.RefreshUI();

        if (msg.pshTaskProgs != null)
        {
            GameRoot.Instance.SetPlayerDataByPshTaskProgs(msg.pshTaskProgs);
            if (taskWindow.gameObject.activeInHierarchy)
            {
                taskWindow.RefreshUI();
            }
        }
    }
    #endregion

    #region StrongOperation
    public void RspStrong(GameMsg msg)
    {
        //提升战力
        int preFight = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByRspStrong(msg.rspStrong);
        int nowFight = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.AddTipsToQueue("战力提升：" + preFight + " --> " + nowFight,Constant.ColorRed);
        //更新UI
        strongWindow.RefreshItemUI();
        mainCityWindow.RefreshUI();

        if (msg.pshTaskProgs != null)
        {
            GameRoot.Instance.SetPlayerDataByPshTaskProgs(msg.pshTaskProgs);
            if (taskWindow.gameObject.activeInHierarchy)
            {
                taskWindow.RefreshUI();
            }
        }
    }
    #endregion

    #region Chat Operation
    public void OpenChatWindow()
    {
        if(!chatWindow.gameObject.activeInHierarchy)
        {
            chatWindow.SetWindowState(true);
        }
        else
        {
            GameRoot.AddTipsToQueue("聊天界面已打开");
        }
    }

    public void CloseChatWindow()
    {
        if(chatWindow.gameObject.activeInHierarchy)
        {
            chatWindow.SetWindowState(false);
        }
    }

    public void PshChat(GameMsg msg)
    {
        chatWindow.AddChatMsg(msg.pshChat.name, msg.pshChat.msg);

        if (msg.pshTaskProgs != null)
        {
            GameRoot.Instance.SetPlayerDataByPshTaskProgs(msg.pshTaskProgs);
            if (taskWindow.gameObject.activeInHierarchy)
            {
                taskWindow.RefreshUI();
            }
        }
    }
    #endregion

    #region Purchase Operation
    public void OpenPurchaseWindow(int buyType)
    {
        StopGuide();
        if(!purchaseWindow.gameObject.activeInHierarchy)
        {
            purchaseWindow.SetBuyType(buyType);
            purchaseWindow.SetWindowState(true);
            if(purchaseWindow.gameObject.activeInHierarchy)
            {
                purchaseWindow.RefreshUI();
            }
        }
    }

    public void ClosePurchaseWindow()
    {
        if(purchaseWindow.gameObject.activeInHierarchy)
        {
            purchaseWindow.SetWindowState(false);
        }
    }

    public void  RspPurchase(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByRspPurchase(msg.rspPurchase);
        switch(msg.rspPurchase.buyType)
        {
            case 0:
                GameRoot.AddTipsToQueue("体力购买成功");
                break;
            case 1:
                GameRoot.AddTipsToQueue("金币铸造成功");
                break;
        }   
        mainCityWindow.RefreshUI();

        if(msg.pshTaskProgs != null)
        {
            GameRoot.Instance.SetPlayerDataByPshTaskProgs(msg.pshTaskProgs);
            if (taskWindow.gameObject.activeInHierarchy)
            {
                taskWindow.RefreshUI();
            }
        }
    }

    public void PshStamina(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByPshStamina(msg.pshStamina);
        if(mainCityWindow.gameObject.activeInHierarchy)
        {
            mainCityWindow.RefreshUI();
        }        
    }
    #endregion

    #region TaskWindow Operation
    public void OpenTaskWindow()
    {
        StopGuide();
        if(!taskWindow.gameObject.activeInHierarchy)
        {
            taskWindow.SetWindowState(true);
        }
    }

    public void CloseTaskWindow()
    {
        if(taskWindow.gameObject.activeInHierarchy)
        {
            taskWindow.SetWindowState(false);
        }
    }

    public void RspTakeTaskReward(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByRspTakeTaskReward(msg.rspTakeTaskReward);
        if(taskWindow.gameObject.activeInHierarchy)
        {
            taskWindow.RefreshUI();
        }
        mainCityWindow.RefreshUI();
    }

    public void PshTaskProgs(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByPshTaskProgs(msg.pshTaskProgs);
        if (taskWindow.gameObject.activeInHierarchy)
        {
            taskWindow.RefreshUI();
        }
    }
    #endregion

    #region Enter FuBenSys
    public void EnterFuBen()
    {
        StopGuide();
        FuBenSys.Instance.OpenFuBenWindow();
    }

    #endregion
    private void Update()
    {
        if(isGuiding)
        {
            IsArriveAim();
            playerController.CameraFollow();
        }
    }

}