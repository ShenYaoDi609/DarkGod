/****************************************************
    文件：Tools.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/4 14:15:0
	功能：工具类
*****************************************************/

using UnityEngine;

public class Tools 
{
    public static int GetRandomInt(int min,int max)
    {
        return Random.Range(min, max + 1);
    }
}