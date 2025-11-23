using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cCharacterSelectPanel : MonoBehaviour
{
    private int m_nSelectNum;

    public GameObject m_SelectImage;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_nSelectNum--;
            if (m_nSelectNum < 0) m_nSelectNum = 2;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_nSelectNum++;
            if (m_nSelectNum > 2) m_nSelectNum = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectCharacter();
            
        }

        SelectImagePositionSetting();
    }

    public void AssassinSelect()
    {
        cSoundManager.GetInstance.PlayUISound(cSoundManager.GetInstance.m_CharacterClick);
        m_nSelectNum = 0;
        m_SelectImage.transform.localPosition = new Vector3(-330, 30, 0);    
    }

    public void WarriorSelect()
    {
        cSoundManager.GetInstance.PlayUISound(cSoundManager.GetInstance.m_CharacterClick);
        m_nSelectNum = 1;
        m_SelectImage.transform.localPosition = new Vector3(30, 30, 0);
    }

    public void MagicianSelect()
    {
        cSoundManager.GetInstance.PlayUISound(cSoundManager.GetInstance.m_CharacterClick);
        m_nSelectNum = 2;
        m_SelectImage.transform.localPosition = new Vector3(380, 30, 0);
    }

    public void SelectCharacter()
    {
        AudioListener.volume = 1.0f;
        cSoundManager.GetInstance.PlayUISound(cSoundManager.GetInstance.m_CharacterSelect);
       
        cCharacterSelectInfo info = GameObject.Find("CharacterSelectInfo").
            GetComponent<cCharacterSelectInfo>();
        switch (m_nSelectNum)
        {
            case 0:
                info.m_eCharacterInfo = cCharacterSelectInfo.eCharacterInfo.ASSASSIN;
                break;
            case 1:
                info.m_eCharacterInfo = cCharacterSelectInfo.eCharacterInfo.WARRIOR;
                break;
            case 2:
                info.m_eCharacterInfo = cCharacterSelectInfo.eCharacterInfo.MAGICIAN;
                break;
        }
        ChangeScene();
    }

    void SelectImagePositionSetting()
    {
       
        switch (m_nSelectNum)
        {
            case 0:
                m_SelectImage.transform.localPosition = new Vector3(-330, 30, 0);
                break;
            case 1:
                m_SelectImage.transform.localPosition = new Vector3(30, 30, 0);
                break;
            case 2:
                m_SelectImage.transform.localPosition = new Vector3(380, 30, 0);
                break;
        }
    }


     void ChangeScene()
    {
		SceneManager.LoadScene("GameScene");
    }

    public void DestroyReturnTitlePanel()
    {
        cSoundManager.GetInstance.PlayUISound(cSoundManager.GetInstance.m_ButtonClick);
    }
}
