/****************************************************
    文件：TaskWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/7/21 8:24:41
	功能：Nothing
*****************************************************/

using UnityEngine;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine.UI;

public class TaskWindow : WindowRoot 
{
    #region Button Define
    public Button closeBtn;
    #endregion
    public Transform itemGroupTrans;
    public Transform verticalScrollBar;
    private PlayerData playerData = null;
    //存放每条任务数据的列表
    private List<TaskRewardData> curTaskDataList = new List<TaskRewardData>();

    protected override void InitWindow()
    {
        base.InitWindow();

        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();

        closeBtn.onClick.AddListener(OnCloseBtnClick);
    }

    protected override void ClearWindow()
    { 
        base.ClearWindow();
        closeBtn.onClick.RemoveAllListeners();
    }

    public void RefreshUI()
    {
        curTaskDataList.Clear();
        verticalScrollBar.GetComponent<Scrollbar>().value = 1;
        for(int i = 0; i < itemGroupTrans.childCount; i++)
        {
            Destroy(itemGroupTrans.GetChild(i).gameObject);
        }

        List<TaskRewardData> todoList = new List<TaskRewardData>();
        List<TaskRewardData> waitDoneList = new List<TaskRewardData>();
        List<TaskRewardData> doneList = new List<TaskRewardData>();

        for(int i = 0; i < playerData.taskArr.Length; i++)
        {
            string[] taskInfo = playerData.taskArr[i].Split('|');
            TaskRewardData data = new TaskRewardData
            {
                ID = int.Parse(taskInfo[0]),
                progress = int.Parse(taskInfo[1]),
                taked = taskInfo[2].Equals("1"),
            };

            if(data.taked == false)
            {
                if(data.progress == resSvc.GetTaskRewardCfg(data.ID).count)
                {
                    waitDoneList.Add(data);
                }
                else
                {
                    todoList.Add(data);
                }
            }
            else
            {
                doneList.Add(data);
            }
        }


        //将未完成任务、待领取任务与已完成任务按顺序加入列表
        curTaskDataList.AddRange(waitDoneList);
        curTaskDataList.AddRange(todoList);
        curTaskDataList.AddRange(doneList);
        for (int j = 0; j < curTaskDataList.Count; j++)
        {
            GameObject itemTask = resSvc.LoadPrefab(PathDefine.ItemTaskPrefab);
            itemTask.transform.SetParent(itemGroupTrans);
            itemTask.transform.localPosition = Vector3.zero;
            itemTask.transform.localScale = Vector3.one;
            itemTask.name = "itemTask_" + j;

            TaskRewardData data = curTaskDataList[j];
            TaskRewardCfg trf = resSvc.GetTaskRewardCfg(data.ID);
            itemTask.GetComponent<ItemTask>().RefreshUI(trf, data);
        }
    }

    public void OnCloseBtnClick()
    {
        audioSvc.PlayUIAudio(Constant.UICommonClick);
        MainCitySys.Instance.CloseTaskWindow();
    }
}