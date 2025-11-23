using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class cEndingPanel : MonoBehaviour
{
    public Image m_SkillImage;
    public Text m_TimeText;

    void Start()
    {
        float fTime = cActorManager.GetInstance.m_fGamePassedTime;


        float fMinutes = fTime / 60;
        float fSecond = fTime % 60;

        string timeStr = "";


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

        cCharacterSelectInfo characterInfo = GameObject.Find("CharacterSelectInfo").GetComponent<cCharacterSelectInfo>();
        switch (characterInfo.m_eCharacterInfo)
        {
            case cCharacterSelectInfo.eCharacterInfo.ASSASSIN:
                m_SkillImage.sprite = Resources.Load<Sprite>("MyGame/UIImage/AssassinSkill");
                break;
            case cCharacterSelectInfo.eCharacterInfo.WARRIOR:
                m_SkillImage.sprite = Resources.Load<Sprite>("MyGame/UIImage/WarriorSkill");
                break;
            case cCharacterSelectInfo.eCharacterInfo.MAGICIAN:
                m_SkillImage.sprite = Resources.Load<Sprite>("MyGame/UIImage/MagicianSkill");
                break;

        }
    }


    public void ChangeEndingScene()
    {
        GameObject characterInfo = GameObject.Find("CharacterSelectInfo");
        Destroy(characterInfo);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("EndingScene");
    }

    public void DestroyEndingPanel()
    {

        cUIManager.GetInstance.DestroyReturnTitlePopup();
    }
}
