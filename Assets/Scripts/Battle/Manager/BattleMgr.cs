/****************************************************
	文件：BattleMgr.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/24 20:24   	
	功能：战斗管理器
*****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using PEProtocol;

public class BattleMgr:MonoBehaviour
{
    private ResSvc resSvc;
    private AudioSvc audioSvc;

    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;
    private MapCfg mapCfg;

    private PlayerController playerController;
    public EntityPlayer entityPlayer;

    public bool isNextTriggerOn = false;
    public bool isPauseGame = false;

    private Dictionary<string, EntityMonster> monsterDicts = new Dictionary<string, EntityMonster>();

    private void Update()
    {
        foreach(EntityMonster em in monsterDicts.Values)
        {
            em.TickAILogic();
        }

        if(mapMgr != null)
        {
            if(!isNextTriggerOn && monsterDicts.Count == 0 )
            {
                bool isExist = mapMgr.SetNextTriggerOn();
                isNextTriggerOn = true;
                if(!isExist)
                {
                    //副本结算
                    entityPlayer.canControl = false;
                    EndBattle(FBEndType.Win ,true, entityPlayer.HP);
                }
            }
        }
    }

    public void EndBattle(FBEndType endType, bool isWin,int restHp)
    {
        AudioSvc.Instance.StopBGMusic();
        BattleSys.Instance.EndBattle(endType, isWin, restHp);
    }

    public void Init(int fbid,Action cb)
    {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;

        //初始化各状态管理器
        stateMgr = gameObject.AddComponent<StateMgr>();
        stateMgr.Init();
        skillMgr = gameObject.AddComponent<SkillMgr>();
        skillMgr.Init();

        //加载战斗的地图
        mapCfg = resSvc.GetMapCfg(fbid);
        resSvc.AsyncLoadScene(mapCfg.sceneName, () =>
         {
             //地图初始化
             GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
             mapMgr = map.GetComponent<MapMgr>();
             mapMgr.Init(this);

             
             audioSvc.PlayBGMusic(Constant.BGOrge,true);

             map.transform.localPosition = Vector3.zero;
             map.transform.localScale = Vector3.one;

             Camera.main.transform.position = mapCfg.mainCamPos;
             Camera.main.transform.localEulerAngles = mapCfg.mainCamRote;

             LoadPlayer(mapCfg);
             BattleSys.Instance.SetPlayerCtrlWindow(true);

             //激活第一批怪物
             ActiveCurrentBatchMonster();

             if(cb != null)
             {
                 cb();
             }
         });

        PECommon.Log("BattleMgr Init Done");
    }


    private void LoadPlayer(MapCfg mapCfg)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssassinBattlePrefab, false);

        player.transform.position = mapCfg.playerBornPos;
        player.transform.localEulerAngles = mapCfg.playerBornRote;
        player.transform.localScale = Vector3.one;

        playerController = player.GetComponent<PlayerController>();
        playerController.Init();

        PlayerData playerData = GameRoot.Instance.PlayerData;
        BattleProps battleProps = new BattleProps
        {
            hp = playerData.hp,
            ad = playerData.ad,
            ap = playerData.ap,
            addef = playerData.addef,
            apdef = playerData.apdef,
            dodge = playerData.dodge,
            pierce = playerData.pierce,
            critical = playerData.critical,
        };

        entityPlayer = new EntityPlayer
        {
            currentAniState = AniState.Idle,
            stateMgr = this.stateMgr,
            skillMgr = this.skillMgr,
            battleMgr = this,
            Name = "AssassinBattle",
        };
        entityPlayer.SetController(playerController);
        entityPlayer.SetBattleProps(battleProps);
    }

    public void LoadMonsterByWaveID(int waveID)
    {
        for(int i =  0; i < mapCfg.monsterList.Count; i++)
        {
            MonsterData md = mapCfg.monsterList[i];
            if(md.mWave == waveID)
            {
                GameObject monster = resSvc.LoadPrefab(md.monsterCfg.resPath, true);
                monster.transform.localPosition = md.mBornPos;
                monster.transform.localEulerAngles = md.mBornRote;
                monster.transform.localScale = Vector3.one;
                monster.name = "monster" + md.mWave + "_" + md.mIndex;

                EntityMonster entityMonster = new EntityMonster
                {
                    stateMgr = this.stateMgr,
                    skillMgr = this.skillMgr,
                    battleMgr = this,
                    Name = monster.name,
                };
                MonsterController monsterController = monster.GetComponent<MonsterController>();
                monsterController.Init();
                entityMonster.SetController(monsterController);

                entityMonster.monsterData = md;
                entityMonster.SetBattleProps(md.monsterCfg.bps);
                if(entityMonster.monsterData.monsterCfg.mType == MonsterType.Normal)
                {
                    entityMonster.InitItemInfo(monster.name, monsterController.hpRoot, entityMonster.Props.hp);
                }
                else if(entityMonster.monsterData.monsterCfg.mType == MonsterType.Boss)
                {
                    BattleSys.Instance.playerCtrlWindow.SetBossHpState(true);
                }
                

                monster.SetActive(false);
                monsterDicts.Add(monster.name, entityMonster);
                
            }
        }
    }

    //激活当前批次怪物
    public void ActiveCurrentBatchMonster()
    {
        TimerSvc.Instance.AddTimeTask((int tid) =>
        {
            foreach(var item in monsterDicts)
            {
                item.Value.SetActive(true);
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask((int ttid) =>
                {
                    item.Value.Idle();
                }, 1000);
            }
        },500);
    }

    //获取所有怪物实体
    public List<EntityMonster> GetEntityMonster()
    {
        List<EntityMonster> monsterList = new List<EntityMonster>();
        foreach(EntityMonster monster in monsterDicts.Values)
        {
            monsterList.Add(monster);
        }
        return monsterList;
    }

    public void RemoveMonster(string name)
    {
        EntityMonster entityMonster = null;
        if(monsterDicts.TryGetValue(name,out entityMonster))
        {
            monsterDicts.Remove(name);
            GameRoot.Instance.dynamicWindow.RemoveItemHp(name);
        }
    }

    public EntityPlayer GetEntityPlayer()
    {
        return entityPlayer;
    }

    #region 技能施放和角色控制
    public void SetMoveDir(Vector2 dir)
    {
        if (entityPlayer.canControl == false)
            return;
        //playerController.Dir = dir;
        if (entityPlayer.currentAniState == AniState.Idle || entityPlayer.currentAniState == AniState.Move)
        {
            if (dir == Vector2.zero)
            {
                entityPlayer.SetDir(dir);
                entityPlayer.Idle();
            }
            else
            {
                entityPlayer.SetDir(dir);
                entityPlayer.Move();
            }
        }
        
    }

    public void ReqReleaseSkill(int index)
    {
        switch (index)
        {
            case 0:
                ReleaseNormalAtk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }

    }

    private int[] comboArr = new int[]
    {
        111,112,113,114,115
    };
    public int curComboIndex = 0;
    public double lastClickTime = 0;

    private void ReleaseNormalAtk()
    {
        if(entityPlayer.currentAniState == AniState.Attack)
        {
            double curClickTime = TimerSvc.Instance.GetNowTime();
            //500ms内按下触发连招
            if (curClickTime- lastClickTime < Constant.interval  && lastClickTime != 0)
            {
                //当combo数没有满时
                if(curComboIndex < comboArr.Length - 1)
                {
                    curComboIndex++;
                    entityPlayer.atkComboQue.Enqueue(comboArr[curComboIndex]);
                    lastClickTime = curClickTime;
                }
                //combo满时，重置连招
                else
                {
                    lastClickTime = 0;
                    curComboIndex = 0;
                }
            }
        }
        else if(entityPlayer.currentAniState == AniState.Idle || entityPlayer.currentAniState == AniState.Move)
        {
            lastClickTime = TimerSvc.Instance.GetNowTime();
            entityPlayer.Attack(Constant.Atk1ID);
        }
        
        //PECommon.Log("NormalAtk");
    }

    private void ReleaseSkill1()
    {
        entityPlayer.Attack(Constant.Skill1ID);
        //PECommon.Log("Skill1");
    }

    private void ReleaseSkill2()
    {
        entityPlayer.Attack(Constant.Skill2ID);
        //PECommon.Log("Skill2");
    }

    private void ReleaseSkill3()
    {
        entityPlayer.Attack(Constant.Skill3ID);
        //PECommon.Log("Skill3");
    }

    public Vector2 GetCurrentDirInput()
    {
        return BattleSys.Instance.GetCurrentDirInput();
    }
    #endregion
}

