using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cPausePanel : MonoBehaviour
{
    public Text m_StageNumText;
    public Text m_TimeText;

    public Image m_SoundButtonImage;
    public Sprite m_SoundOn;
    public Sprite m_SoundOff;

    void Start()
    {
        m_StageNumText = GameObject.Find("PauseStageNumText").GetComponent<Text>();
        m_TimeText = GameObject.Find("TimeText").GetComponent<Text>();

        string stage = "Stage ";

        stage += cMapManager.GetInstance.m_nMapNum;

        m_StageNumText.text = stage;

        float fTime = cActorManager.GetInstance.m_fGamePassedTime;
        float fMinutes = fTime / 60;
        float fSecond = fTime % 60;

        string timeStr="";

        
        if (fMinutes < 10)
        {
            timeStr += "0" + (int)fMinutes;
        }
        else
        {
            timeStr += (int)fMinutes;
        }

        if (fSecond < 10)
        {
            timeStr += " : " + "0" + (int)fSecond;
        }
        else
        {
            timeStr += " : " + (int)fSecond;
        }

        m_TimeText.text = timeStr;
        SoundState();
    }


    public void SoundState()
    {
        if (cUIManager.GetInstance.m_isSoundPlay)
        {
            m_SoundButtonImage.sprite = m_SoundOn;
        }
        else
        {
            m_SoundButtonImage.sprite = m_SoundOff;
        }
    }

    public void SoundChange()
    {
        if(cUIManager.GetInstance.m_isSoundPlay)
        {
            cUIManager.GetInstance.m_isSoundPlay = false;
            cSoundManager.GetInstance.SoundOff();
            m_SoundButtonImage.sprite = m_SoundOff;

        }
        else
        {
            cUIManager.GetInstance.m_isSoundPlay = true;
            cSoundManager.GetInstance.SoundOn();
            m_SoundButtonImage.sprite = m_SoundOn;
        }
    }


    public void CreateReturnTitlePanel()
    {
        cSoundManager.GetInstance.PlayButtonSound();
        cUIManager.GetInstance.CreateReturnTitlePopup();
    }

    public void DestroyPausePanel()
    {
        cSoundManager.GetInstance.PlayButtonSound();
        cUIManager.GetInstance.DestroyPausePopup();
    }
}
