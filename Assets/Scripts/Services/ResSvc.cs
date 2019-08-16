/****************************************************
    文件：Resvc.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/2 17:29:15
	功能：资源加载模块
*****************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResSvc : MonoBehaviour
{
    #region 单例模式
    private static ResSvc _instance = null;

    public static ResSvc Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<ResSvc>();
            }
            return _instance;
        }
    }
    #endregion

    public void InitSvc()
    {
        InitRandomNameCfgs(PathDefine.nameCfgs);
        InitMonsterCfgs(PathDefine.monsterCfgs);
        InitMapCfgs(PathDefine.mapCfgs);
        InitAutoGuideCfgs(PathDefine.autoGuideCfgs);
        InitStrongCfg(PathDefine.strongCfgs);
        InitTaskRewardCfgs(PathDefine.taskRewardCfgs);
        InitSkillCfgs(PathDefine.skillCfgs);
        InitSkillMoveCfgs(PathDefine.skillMoveCfgs);
        InitSkilActionCfgs(PathDefine.skillActionCfgs);


        PECommon.Log("Init ResSvc....");
    }

    //更新进度条的委托
    private Action progressUpdate = null;
    //存储音乐的字典
    private Dictionary<string, AudioClip> audioDicts = new Dictionary<string, AudioClip>();
    //存储动画的字典
    private Dictionary<string, AnimationClip> animationDicts = new Dictionary<string, AnimationClip>();
    //存储Prefab的字典
    private Dictionary<string, GameObject> prefabDicts = new Dictionary<string, GameObject>();
    //存储Sprite的字典
    private Dictionary<string, Sprite> spriteDicts = new Dictionary<string, Sprite>();

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="finish"></param>
    public void AsyncLoadScene(string sceneName,Action finish)
    {
        //loadingWindow设置窗口状态
        GameRoot.Instance.loadingWindow.SetWindowState(true);

        AsyncOperation sceneAsync = SceneManager.LoadSceneAsync(sceneName);
        progressUpdate = () =>
        {
            float progress = sceneAsync.progress;
            //设置进度条
            GameRoot.Instance.loadingWindow.SetProgress(progress);
            //加载结束隐藏加载界面
            if (progress == 1)
            {
                if(finish != null)
                {
                    finish();
                }
                //将委托置空 否则将一直被调用
                progressUpdate = null;
                sceneAsync = null;
                GameRoot.Instance.loadingWindow.SetWindowState(false);
            }
        };
    }

    /// <summary>
    /// 加载音效资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="cache"></param>
    /// <returns></returns>
    public AudioClip LoadAudioClip(string path,bool cache = false)
    {
        AudioClip res = null;
        if(!audioDicts.TryGetValue(path,out res))
        {
            res = Resources.Load<AudioClip>(path);
            if(cache)
            {
                audioDicts.Add(path, res);
            }
        }
        return res;
    }

    /// <summary>
    /// 加载动画资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public AnimationClip LoadAnimationClip(string path,bool cache = false)
    {
        AnimationClip ani = null;
        if(!animationDicts.TryGetValue(path,out ani))
        {
            ani = Resources.Load<AnimationClip>(path);
            if(cache)
            {
                animationDicts.Add(path, ani);
            }
        }
        return ani;
    }

    /// <summary>
    /// 加载预制体资源
    /// </summary>
    public GameObject LoadPrefab(string path,bool cache = false)
    {
        GameObject prefab = null;
        if(!prefabDicts.TryGetValue(path,out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if(cache)
            {
                prefabDicts.Add(path, prefab);
            }
        }
        GameObject go = null;
        if(prefab != null)
        {
            go = Instantiate(prefab);
        }
        return go;
    }

    /// <summary>
    /// 加载Sprite资源
    /// </summary>
    public Sprite LoadSprite(string path,bool cache = false)
    {
        Sprite sprite = null;
        if(!spriteDicts.TryGetValue(path,out sprite))
        {
            sprite = Resources.Load<Sprite>(path);
            if(cache)
            {
                spriteDicts.Add(path, sprite);
            }
        }
        return sprite;
    }

    #region InitCfgs
    #region 技能配置
    public Dictionary<int, SkillCfg> skillCfgDicts = new Dictionary<int, SkillCfg>();
    public Dictionary<int, SkillMoveCfg> skillMoveCfgDicts = new Dictionary<int, SkillMoveCfg>();
    public Dictionary<int, SkillActionCfg> skillActionCfgDicts = new Dictionary<int, SkillActionCfg>();

    public void InitSkillCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;
        foreach(XmlNode node in  nodeList)
        {
            int ID = int.Parse(node.Attributes["ID"].Value);
            SkillCfg skillCfg = new SkillCfg
            {
                ID = ID,
                skillMoveList = new List<int>(),
                skillActionList = new List<int>(),
                skillDamageList = new List<int>(),
            };
            XmlNodeList fieldNodeList = node.ChildNodes;
            foreach(XmlNode fieldNode in fieldNodeList)
            {
                switch(fieldNode.Name)
                {
                    case "skillName":
                        skillCfg.skillName = fieldNode.InnerText;
                        break;
                    case "cdTime":
                        skillCfg.cdTime = int.Parse(fieldNode.InnerText);
                        break;
                    case "skillTime":
                        skillCfg.skillTime = int.Parse(fieldNode.InnerText);
                        break;
                    case "aniAction":
                        skillCfg.aniAction = int.Parse(fieldNode.InnerText);
                        break;
                    case "fx":
                        skillCfg.fx = fieldNode.InnerText;
                        break;
                    case "isCombo":
                        skillCfg.isCombo = fieldNode.InnerText.Equals("1");
                        break;
                    case "isCollide":
                        skillCfg.isCollide = fieldNode.InnerText.Equals("1");
                        break;
                    case "isBreak":
                        skillCfg.isBreak = fieldNode.InnerText.Equals("1");
                        break;
                    case "dmgType":
                        if(fieldNode.InnerText.Equals("1"))
                        {
                            skillCfg.dmgType = DamageType.AD;
                        }
                        else if(fieldNode.InnerText.Equals("2"))
                        {
                            skillCfg.dmgType = DamageType.AP;
                        }
                        else
                        {
                            PECommon.Log("Error DamageType");
                        }
                        break;
                    case "skillMoveLst":
                        string[] skillMoveList = fieldNode.InnerText.Split('|');

                        for (int i = 0; i < skillMoveList.Length; i++)
                        {
                            if(skillMoveList[i] != null)
                            {                                
                                skillCfg.skillMoveList.Add(int.Parse(skillMoveList[i]));
                            }
                        }
                        break;
                    case "skillActionLst":
                        string[] skillActionList = fieldNode.InnerText.Split('|');
                        for(int i = 0; i < skillActionList.Length; i++)
                        {
                            if(skillActionList[i] != null)
                            {
                                skillCfg.skillActionList.Add(int.Parse(skillActionList[i]));
                            }
                        }
                        break;
                    case "skillDamageLst":
                        string[] skillDamageList = fieldNode.InnerText.Split('|');
                        for(int i = 0; i < skillDamageList.Length; i++)
                        {
                            if(skillDamageList[i] != null)
                            {
                                skillCfg.skillDamageList.Add(int.Parse(skillDamageList[i]));
                            }
                        }
                        break;
                }
            }
            if(!skillCfgDicts.ContainsKey(ID))
            {
                skillCfgDicts.Add(ID, skillCfg);
            }
        }
    }

    public SkillCfg GetSkillCfg(int id)
    {
        SkillCfg skillCfg = null;
        if (skillCfgDicts.TryGetValue(id, out skillCfg))
        {
            return skillCfg;
        }
        return null;
    }


    public void InitSkillMoveCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;
        foreach(XmlNode node in nodeList)
        {
            int ID = int.Parse(node.Attributes["ID"].Value);
            SkillMoveCfg skillMoveCfg = new SkillMoveCfg
            {
                ID = ID,
            };
            XmlNodeList fieldNodeList = node.ChildNodes;
            foreach(XmlNode fieldNode in fieldNodeList)
            {
                switch(fieldNode.Name)
                {
                    case "delayTime":
                        skillMoveCfg.delayTime = int.Parse(fieldNode.InnerText);
                        break;
                    case "moveTime":
                        skillMoveCfg.moveTime = int.Parse(fieldNode.InnerText);
                        break;
                    case "moveDis":
                        skillMoveCfg.moveDis = float.Parse(fieldNode.InnerText);
                        break;
                }
            }
            if(!skillMoveCfgDicts.ContainsKey(ID))
            {
                skillMoveCfgDicts.Add(ID, skillMoveCfg);
            }
        }
    }

    public SkillMoveCfg GetSkillMoveCfg(int id)
    {
        SkillMoveCfg skillMoveCfg = null;
        if (skillMoveCfgDicts.TryGetValue(id, out skillMoveCfg))
        {
            return skillMoveCfg;
        }
        return null;
    }


    public void InitSkilActionCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;
        foreach (XmlNode node in nodeList)
        {
            int ID = int.Parse(node.Attributes["ID"].Value);
            SkillActionCfg skillActionCfg = new SkillActionCfg
            {
                ID = ID,
            };
            XmlNodeList fieldNodeList = node.ChildNodes;
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "delayTime":
                        skillActionCfg.delayTime = int.Parse(fieldNode.InnerText);
                        break;
                    case "radius":
                        skillActionCfg.radius = float.Parse(fieldNode.InnerText);
                        break;
                    case "angle":
                        skillActionCfg .angle = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            if (!skillActionCfgDicts.ContainsKey(ID))
            {
                skillActionCfgDicts.Add(ID, skillActionCfg);
            }
        }
    }

    public SkillActionCfg GetSkillActionCfg(int id)
    {
        SkillActionCfg skillActionCfg = null;
        if (skillActionCfgDicts.TryGetValue(id, out skillActionCfg))
        {
            return skillActionCfg;
        }
        return null;
    }
    #endregion

    #region 怪物配置
    private Dictionary<int, MonsterCfg> monsterCfgDicts = new Dictionary<int, MonsterCfg>();
    public void InitMonsterCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;
        foreach (XmlNode node in nodeList)
        {
            int ID = int.Parse(node.Attributes["ID"].Value);
            MonsterCfg monsterCfg = new MonsterCfg
            {
                ID = ID,
                bps = new BattleProps(),
            };
            XmlNodeList fieldNodeList = node.ChildNodes;
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "mName":
                        monsterCfg.name = fieldNode.InnerText;
                        break;
                    case "resPath":
                        monsterCfg.resPath = fieldNode.InnerText;
                        break;
                    case "mType":
                        monsterCfg.mType = (MonsterType)Enum.Parse(typeof(MonsterType), fieldNode.InnerText);
                        break;
                    case "isStop":
                        monsterCfg.isStop = fieldNode.InnerText.Equals("1");
                        break;
                    case "skillID":
                        monsterCfg.skillID = int.Parse(fieldNode.InnerText);
                        break;
                    case "atkDis":
                        monsterCfg.atkDis = float.Parse(fieldNode.InnerText);
                        break;
                    case "hp":
                        monsterCfg.bps.hp = int.Parse(fieldNode.InnerText);
                        break;
                    case "ad":
                        monsterCfg.bps.ad = int.Parse(fieldNode.InnerText);
                        break;
                    case "ap":
                        monsterCfg.bps.ap = int.Parse(fieldNode.InnerText);
                        break;
                    case "addef":
                        monsterCfg.bps.addef = int.Parse(fieldNode.InnerText);
                        break;
                    case "apdef":
                        monsterCfg.bps.apdef = int.Parse(fieldNode.InnerText);
                        break;
                    case "dodge":
                        monsterCfg.bps.dodge = int.Parse(fieldNode.InnerText);
                        break;
                    case "pierce":
                        monsterCfg.bps.pierce = int.Parse(fieldNode.InnerText);
                        break;
                    case "critical":
                        monsterCfg.bps.critical = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            if (!monsterCfgDicts.ContainsKey(ID))
            {
                monsterCfgDicts.Add(ID, monsterCfg);
            }
        }
    }
    public MonsterCfg GetMonsterCfg(int id)
    {
        MonsterCfg monsterCfg = null;
        if(monsterCfgDicts.TryGetValue(id,out monsterCfg))
        {
            return monsterCfg;
        }
        return null;
    }
    #endregion

    #region 随机名字
    private List<string> surnameList = new List<string>();
    private List<string> manNameList = new List<string>();
    private List<string> womanNameList = new List<string>();
    //加载有关名字的xml文件
    public void  InitRandomNameCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        //取得根结点
        XmlNode root = xmlDoc.SelectSingleNode("root");
        //取得根节点下所有子节点
        XmlNodeList nodeList = root.ChildNodes;

        foreach (XmlNode nameNode in nodeList)
        {
            XmlNodeList fieldNodeList = nameNode.ChildNodes;
            foreach(XmlNode fieldNode in fieldNodeList)
            {
                switch(fieldNode.Name)
                {
                    case "surname":
                        surnameList.Add(fieldNode.InnerText);
                        break;
                    case "man":
                        manNameList.Add(fieldNode.InnerText);
                        break;
                    case "woman":
                        womanNameList.Add(fieldNode.InnerText);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 获取随机名字
    /// </summary>
    public string GetRandomName(bool man = true)
    {
        string name = surnameList[Tools.GetRandomInt(0, surnameList.Count - 1)];
        if(man)
        {
            name += manNameList[Tools.GetRandomInt(0, manNameList.Count - 1)];
        }
        else
        {
            name += womanNameList[Tools.GetRandomInt(0, womanNameList.Count - 1)];
        }
        return name;
    }
    #endregion

    #region 地图信息
    public Dictionary<int, MapCfg> mapCfgDicts = new Dictionary<int, MapCfg>();
    public void InitMapCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        //取得根结点
        XmlNode root = xmlDoc.SelectSingleNode("root");
        //取得根节点下所有子节点
        XmlNodeList nodeList = root.ChildNodes;

        foreach (XmlNode mapNode in nodeList)
        {
            int ID = int.Parse( mapNode.Attributes["ID"].Value);
            MapCfg mapCfg = new MapCfg {
                ID = ID,
                monsterList = new List<MonsterData>(),
            };

            XmlNodeList fieldNodeList = mapNode.ChildNodes;
            foreach (XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "mapName":
                        mapCfg.mapName = fieldNode.InnerText;
                        break;
                    case "sceneName":
                        mapCfg.sceneName = fieldNode.InnerText;
                        break;
                    case "power":
                        mapCfg.costStamina = int.Parse(fieldNode.InnerText);
                        break;
                    //加上中括号为了让values在不同的变量域内
                    case "mainCamPos":
                        {
                            string[] values = fieldNode.InnerText.Split(',');
                            mapCfg.mainCamPos = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                        }
                        break;
                    case "mainCamRote":
                        {
                            string[] values = fieldNode.InnerText.Split(',');
                            mapCfg.mainCamRote = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                        }
                        break;
                    case "playerBornPos":
                        {
                            string[] values = fieldNode.InnerText.Split(',');
                            mapCfg.playerBornPos = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                        }
                        break;
                    case "playerBornRote":
                        {
                            string[] values = fieldNode.InnerText.Split(',');
                            mapCfg.playerBornRote = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
                        }
                        break;
                    case "monsterLst":
                        {
                            string[] monsterArr = fieldNode.InnerText.Split('#');
                            for(int waveIndex = 0; waveIndex < monsterArr.Length; waveIndex++)
                            {
                                if (waveIndex == 0)
                                    continue;
                                string[] tempArr = monsterArr[waveIndex].Split('|');
                                for(int j = 0; j < tempArr.Length; j++)
                                {
                                    if (j == 0)
                                        continue;
                                    string[] arr = tempArr[j].Split(',');
                                    MonsterData monsterData = new MonsterData
                                    {
                                        ID = int.Parse(arr[0]),
                                        mWave = waveIndex,
                                        mIndex = j,
                                        monsterCfg = GetMonsterCfg(int.Parse(arr[0])),
                                        mBornPos = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[3])),
                                        mBornRote = new Vector3(0, float.Parse(arr[4]), 0),
                                        mLevel = int.Parse(arr[5]),
                                    };
                                    mapCfg.monsterList.Add(monsterData);
                                }
                            }
                        }
                        break;
                    case "exp":
                        mapCfg.exp = int.Parse(fieldNode.InnerText);
                        break;
                    case "coin":
                        mapCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                    case "crystal":
                        mapCfg.crystal = int.Parse(fieldNode.InnerText);
                        break;
                    default:
                        break;
                }
            }
            if(!mapCfgDicts.ContainsKey(ID))
            {
                mapCfgDicts.Add(ID, mapCfg);
            }
        }
    }
    public MapCfg GetMapCfg(int id)
    {
        MapCfg mapCfg;
        if(mapCfgDicts.TryGetValue(id,out mapCfg))
        {
            return mapCfg;
        }
        return null;
    }
    #endregion

    #region 自动任务信息
    public Dictionary<int, AutoGuideCfg> autoGuideCfgDicts = new Dictionary<int, AutoGuideCfg>();
    public void InitAutoGuideCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;

        foreach(XmlNode taskNode in nodeList)
        {
            int _taskID = int.Parse(taskNode.Attributes["ID"].Value);
            AutoGuideCfg autoGuideCfg = new AutoGuideCfg
            {
                ID = _taskID
            };
            XmlNodeList fieldNodeList = taskNode.ChildNodes;
            foreach(XmlNode fieldNode in fieldNodeList)
            {
                switch(fieldNode.Name)
                {
                    case "npcID":
                        autoGuideCfg.npcID = int.Parse(fieldNode.InnerText);
                        break;
                    case "dilogArr":
                        string[] _dilogArr = fieldNode.InnerText.Split('#');
                        autoGuideCfg.dilogArr = _dilogArr;
                        break;
                    case "actID":
                        autoGuideCfg.actID = int.Parse(fieldNode.InnerText);
                        break;
                    case "coin":
                        autoGuideCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                    case "exp":
                        autoGuideCfg.exp = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            if(!autoGuideCfgDicts.ContainsKey(_taskID))
            {
                autoGuideCfgDicts.Add(_taskID, autoGuideCfg);
                
            }
        }
    }

    public AutoGuideCfg GetTaskCfg(int id)
    {
        AutoGuideCfg autoGuideCfg = null;
        if(autoGuideCfgDicts.TryGetValue(id,out autoGuideCfg))
        {
            return autoGuideCfg;
        }
        return null;
    }
    #endregion

    #region 强化配置
    public Dictionary<int, Dictionary<int, StrongCfg>> strongDict = new Dictionary<int, Dictionary<int, StrongCfg>>();
    public void InitStrongCfg(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);

        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;
        foreach(XmlNode strongNode in nodeList)
        {
            int id = int.Parse(strongNode.Attributes["ID"].Value);
            StrongCfg strongCfg = new StrongCfg
            {
                ID = id
            };
            XmlNodeList fieldNodeList = strongNode.ChildNodes;
            foreach(XmlNode fieldNode in fieldNodeList)
            {
                switch (fieldNode.Name)
                {
                    case "pos":
                        strongCfg.pos = int.Parse(fieldNode.InnerText);
                        break;
                    case "starlv":
                        strongCfg.starlv = int.Parse(fieldNode.InnerText);
                        break;
                    case "addhp":
                        strongCfg.addhp = int.Parse(fieldNode.InnerText);
                        break;
                    case "addhurt":
                        strongCfg.addhurt = int.Parse(fieldNode.InnerText);
                        break;
                    case "adddef":
                        strongCfg.adddef = int.Parse(fieldNode.InnerText);
                        break;
                    case "minlv":
                        strongCfg.minlv = int.Parse(fieldNode.InnerText);
                        break;
                    case "coin":
                        strongCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                    case "crystal":
                        strongCfg.crystal = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            Dictionary<int, StrongCfg> dic = null;
            //如果有对应的键pos，说明这类装备已经有部分升级配置信息被录入,直接加入
            if(strongDict.TryGetValue(strongCfg.pos,out dic))
            {
                dic.Add(strongCfg.starlv, strongCfg);
            }
            else
            {
                dic = new Dictionary<int, StrongCfg>();
                dic.Add(strongCfg.starlv, strongCfg);
                strongDict.Add(strongCfg.pos, dic);
            }

        }
    }

    public StrongCfg GetStrongCfg(int pos,int starlv)
    {
        StrongCfg strongCfg = null;
        Dictionary<int, StrongCfg> dic = null;
        if(strongDict.TryGetValue(pos,out dic))
        {
            if(dic.ContainsKey(starlv))
            {
                strongCfg = dic[starlv];
            }
        }
        return strongCfg;
    }

    public int GetPropAddPreLv(int pos,int starLv,int type)
    {
        int val = 0;
        Dictionary<int, StrongCfg> dic = null;
        if(strongDict.TryGetValue(pos,out dic))
        {
            for(int i = 1; i <= starLv; i++)
            {
                StrongCfg strongCfg = null;
                if(dic.TryGetValue(i,out strongCfg))
                {
                    switch (type)
                    {
                        case 1://hp
                            val += strongCfg.addhp;
                            break;
                        case 2://attack
                            val += strongCfg.addhurt;
                            break;
                        case 3://def
                            val += strongCfg.adddef;
                            break;
                    }
                }
            }
        }
        return val;
    }

    #endregion

    #region 任务奖励配置
    public Dictionary<int, TaskRewardCfg> taskRewardCfgDicts = new Dictionary<int, TaskRewardCfg>();
    public void InitTaskRewardCfgs(string path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(path);
        XmlNode root = xmlDoc.SelectSingleNode("root");
        XmlNodeList nodeList = root.ChildNodes;

        foreach(XmlNode taskRewardNode in nodeList)
        {
            int id = int.Parse(taskRewardNode.Attributes["ID"].Value);
            TaskRewardCfg taskRewardCfg = new TaskRewardCfg
            {
                ID = id,
            };
            XmlNodeList fieldNodeList = taskRewardNode.ChildNodes;
            foreach(XmlNode fieldNode in fieldNodeList)
            {
                switch(fieldNode.Name)
                {
                    case "taskName":
                        taskRewardCfg.taskName = fieldNode.InnerText;
                        break;
                    case "count":
                        taskRewardCfg.count = int.Parse(fieldNode.InnerText);
                        break;
                    case "exp":
                        taskRewardCfg.exp = int.Parse(fieldNode.InnerText);
                        break;
                    case "coin":
                        taskRewardCfg.coin = int.Parse(fieldNode.InnerText);
                        break;
                }
            }
            if(!taskRewardCfgDicts.ContainsKey(id))
            {
                taskRewardCfgDicts.Add(id, taskRewardCfg);
                //Debug.Log(taskRewardCfg.ID + " " + taskRewardCfg.taskName + " " + taskRewardCfg.count + " " + taskRewardCfg.exp + " " + taskRewardCfg.coin);
            }
        }
    }

    public TaskRewardCfg GetTaskRewardCfg(int id)
    {
        TaskRewardCfg taskRewardCfg = null;
        if(taskRewardCfgDicts.TryGetValue(id,out taskRewardCfg))
        {
            return taskRewardCfg;
        }
        return null;
    }
    #endregion
    #endregion


    private void Update()
    {
        if(progressUpdate != null)
        {
            progressUpdate();
        }
    }
}