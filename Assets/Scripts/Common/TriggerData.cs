/****************************************************
    文件：TriggerData.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/14 14:56:53
	功能：Nothing
*****************************************************/

using UnityEngine;

public class TriggerData : MonoBehaviour 
{
    public MapMgr mapMgr;
    public int waveID;

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(mapMgr != null)
            {
                mapMgr.ActiveTriggerMonster(waveID);
                this.gameObject.GetComponent<BoxCollider>().isTrigger = false;
            }
        }
    }
}