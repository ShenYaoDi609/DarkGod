/****************************************************
    文件：ItemEntityHp.cs
	作者：Shen
    邮箱: 879085103@qq.com
    日期：2019/8/3 18:35:1
	功能：血条控制
*****************************************************/

using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHp : MonoBehaviour 
{
    public Text txtCritical;
    public Text txtDodge;
    public Text txtHp;
    public Image imgHpGray;
    public Image imgHpRed;
    public Animation criticalAnim;
    public Animation dodgeAnim;
    public Animation hpAnim;

    private Transform monsterTrans;
    private RectTransform rect;
    private int hpVal;
    private float scaleRate = 1.0f * Constant.ScreenStandardHeight / Screen.height;
    private float currentPrg;
    private float targetPrg;

    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(monsterTrans.position);
        
        rect.anchoredPosition = screenPos * scaleRate;

        imgHpGray.fillAmount = currentPrg;
        UpdateMixBlend();
    }

    private void UpdateMixBlend()
    {
        if(Mathf.Abs(currentPrg - targetPrg) < Constant.smoothHpSpeed * Time.deltaTime)
        {
            currentPrg = targetPrg;
        }
        else
        {
            if(targetPrg > currentPrg)
            {
                currentPrg += Constant.smoothHpSpeed * Time.deltaTime;
            }
            else
            {
                currentPrg -= Constant.smoothHpSpeed * Time.deltaTime;
            }
        }
    }

    public void InitItemInfo(Transform trans,int hp)
    {
        rect = gameObject.GetComponent<RectTransform>();
        monsterTrans = trans;
        hpVal = hp;
        imgHpGray.fillAmount = 1;
        imgHpRed.fillAmount = 1;
    }

    public void SetCritical(int critical)
    {
        criticalAnim.Stop();
        txtCritical.text = "暴击 " + critical;
        criticalAnim.Play();
    }

    public void SetDodge()
    {
        dodgeAnim.Stop();
        dodgeAnim.Play();
    }

    public void SetHurt(int hurt)
    {
        hpAnim.Stop();
        txtHp.text = "-" + hurt;
        hpAnim.Play();
    }

    public void SetHp(int oldVal,int newVal)
    {
        currentPrg = oldVal * 1.0f / hpVal;
        targetPrg = newVal * 1.0f / hpVal;
        imgHpRed.fillAmount = targetPrg;
    }

}