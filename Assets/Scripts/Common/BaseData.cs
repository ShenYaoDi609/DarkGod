/****************************************************
	文件：BaseData.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/06/20 18:30   	
	功能：配置数据类
*****************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MonsterData:BaseData<MonsterData>
{
    public int mWave;//批次
    public int mIndex;//序号 
    public MonsterCfg monsterCfg;
    public Vector3 mBornPos;
    public Vector3 mBornRote;
    public int mLevel;
}

public class MapCfg:BaseData<MapCfg>
{
    public string mapName;
    public string sceneName;
    public int costStamina;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;
    public List<MonsterData> monsterList;
    public int exp;
    public int coin;
    public int crystal;
}

public class AutoGuideCfg:BaseData<AutoGuideCfg>
{
    public int npcID;
    public string[] dilogArr;
    public int actID;
    public int coin;
    public int exp;
}

public class StrongCfg:BaseData<StrongCfg>
{
    public int pos;
    public int starlv;
    public int addhp;
    public int addhurt;
    public int adddef;
    public int minlv;
    public int coin;
    public int crystal;
}

public class TaskRewardCfg:BaseData<TaskRewardCfg>
{
    public string taskName;
    public int count;
    public int exp;
    public int coin;
}

public class TaskRewardData : BaseData<TaskRewardData>
{
    public int progress;
    public bool taked;
}

public class SkillCfg:BaseData<SkillCfg>
{
    public string skillName;
    public int cdTime;
    public int skillTime;
    public int aniAction;
    public string fx;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;
    public DamageType dmgType;
    public List<int> skillMoveList;
    public List<int> skillActionList;
    public List<int> skillDamageList;
}

public class SkillMoveCfg : BaseData<SkillMoveCfg>
{
    public int delayTime;
    public int moveTime;
    public float moveDis;
}

public class  SkillActionCfg:BaseData<SkillActionCfg>
{
    public int delayTime;
    public float radius; //伤害计算范围
    public int angle;   //伤害有效角度
}

public class MonsterCfg:BaseData<MonsterCfg>
{
    public string name;
    public string resPath;
    public MonsterType mType;
    public bool isStop;
    public int skillID;
    public float atkDis;
    public BattleProps bps;
}



public class BaseData<T>
{
    public int ID;
}

public class BattleProps
{
    public int hp;
    public int ad;
    public int ap;
    public int addef;
    public int apdef;
    public int dodge;
    public int pierce;
    public int critical;
}

public enum DamageType
{
    None,
    AD = 1,
    AP = 2
}

