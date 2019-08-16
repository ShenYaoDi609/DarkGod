/****************************************************
    文件：AudioSvc.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/6/3 10:18:49
	功能：音效播放服务
*****************************************************/

using UnityEngine;

public class AudioSvc : MonoBehaviour 
{
    #region 单例模式
    private static AudioSvc _instance = null;

    public static AudioSvc Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameRoot").GetComponent<AudioSvc>();
            }
            return _instance;
        }
    }
    #endregion

    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public void InitSvc()
    {
        PECommon.Log("Init AudioSvc......");
    }

    public  void StopBGMusic()
    {
        if(bgAudio  != null)
        {
            bgAudio.Stop();
        }       
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    public void PlayBGMusic(string name, bool isLoop)
    {
        AudioClip audio = ResSvc.Instance.LoadAudioClip("ResAudio/" + name, true);
        if (bgAudio.clip == null || bgAudio.clip.name != audio.name)
        {
            bgAudio.clip = audio;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        } 
    }

    /// <summary>
    /// 播放UI音效
    /// </summary>
    /// <param name="name"></param>
    public void PlayUIAudio(string name)
    {
        AudioClip audio = ResSvc.Instance.LoadAudioClip("ResAudio/" + name, true);
        uiAudio.clip = audio;
        uiAudio.Play();
    }

    public void PlayCharHitAudio(AudioSource audio,string name)
    {
        AudioClip clip = ResSvc.Instance.LoadAudioClip("ResAudio/" + name, true);
        audio.clip = clip;
        audio.Play();
    }


}