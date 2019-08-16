/****************************************************
    文件：LoadingWindow.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/2 17:59:52
	功能：加载进度场景
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : WindowRoot 
{
    public Text tipsText;
    public Image progressBarFg;
    public Image flicker;
    public Text progressText;

    //进度条的长度 
    private float fgLength;

    protected override void InitWindow()
    {
        base.InitWindow();

        fgLength = progressBarFg.GetComponent<RectTransform>().sizeDelta.x;

        SetText(tipsText, "Tips：这是一条游戏Tips");
        progressBarFg.fillAmount = 0;
        SetText(progressText, "0%");
        flicker.transform.localPosition = new Vector3(-546,0,0);
    }

    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="progress"></param>
    public void SetProgress(float progress)
    {
        progressBarFg.fillAmount = progress;
        SetText(progressText, (int)(progress * 100) + "%");

        float posX = progress * fgLength - fgLength / 2;
        flicker.transform.localPosition = new Vector3(posX, 0, 0);
    }

}