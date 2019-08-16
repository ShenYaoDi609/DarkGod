/****************************************************
	文件：MapMgr.cs
	作者：Shen
	邮箱:  879085103@qq.com
	日期：2019/07/24 20:34   	
	功能：地图管理器
*****************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

public class MapMgr:MonoBehaviour
{
    private int waveIndex = 1;
    private BattleMgr battleMgr;

    public TriggerData[] triggerList;


    public void Init(BattleMgr battleMgr)
    {
        this.battleMgr = battleMgr;

        //实例化第一批怪物
        battleMgr.LoadMonsterByWaveID(waveIndex);

        PECommon.Log("MapMgr Init Done");
    }

    public void ActiveTriggerMonster(int waveID)
    {
        if(battleMgr != null)
        {
            battleMgr.isNextTriggerOn = false;
            battleMgr.LoadMonsterByWaveID(waveID);
            battleMgr.ActiveCurrentBatchMonster();
        }
    }

    public bool SetNextTriggerOn()
    {
        waveIndex++;
        for(int i = 0; i < triggerList.Length; i++)
        {
            if(triggerList[i].waveID == waveIndex)
            {
                triggerList[i].gameObject.GetComponent<BoxCollider>().isTrigger = true;
                return true;
            }
        }
        return false;   
    }
}

