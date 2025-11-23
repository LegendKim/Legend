using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cUIManager : cSingleTon<cUIManager>
{
    public GameObject m_optionPopupPrefab;

    public GameObject m_returnTitlePopupPrefab;

    public GameObject m_PlayerDiePopupPrefab;

    public GameObject m_AbilitySelectPopupPrefab;

    public GameObject m_EndingPopupPrefab;

    public GameObject m_pauseButton;

    public GameObject m_playerHuD;
    public GameObject m_expHUD;

    public Image m_ExpBarImage;
    public Image m_HpImage;
    public Text m_LevelText;
    public Text m_HpText;


    public Button m_SkillButton;

    Stack<GameObject> m_StackPausePopup;
    Stack<GameObject> m_StackPlayerDiePopup;
    Stack<GameObject> m_StackReturnTitlePopup;
    Stack<GameObject> m_StackAbilitySelectPopup;
    Stack<GameObject> m_StackEndingPopup;

    public bool m_isPauseGame;

    public bool m_isPlayerLevelUp;

    public GameObject m_StageNumLabel;

    public cPlayer m_player;

    public bool m_isSoundPlay;

    protected override void Awake()
    {
        base.Awake();
        m_isPauseGame = false;
        m_isPlayerLevelUp = false;
        m_isSoundPlay = true;

        m_StackPausePopup = new Stack<GameObject>();
        m_StackReturnTitlePopup = new Stack<GameObject>();
        m_StackAbilitySelectPopup = new Stack<GameObject>();
        m_StackPlayerDiePopup = new Stack<GameObject>();
        m_StackEndingPopup = new Stack<GameObject>();

        cCharacterSelectInfo characterInfo = GameObject.Find("CharacterSelectInfo").GetComponent<cCharacterSelectInfo>();
        switch (characterInfo.m_eCharacterInfo)
        {
            case cCharacterSelectInfo.eCharacterInfo.ASSASSIN:
                m_SkillButton.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/AssassinSkill");
                break;
            case cCharacterSelectInfo.eCharacterInfo.WARRIOR:
                m_SkillButton.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/WarriorSkill");
                break;
            case cCharacterSelectInfo.eCharacterInfo.MAGICIAN:
                m_SkillButton.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/MagicianSkill");
                break;

        }


    }

    protected void Start()
    {
        m_player = GameObject.FindWithTag("Player")
     .GetComponent<cPlayer>();
    }


    private void Update()
    {
        if(!m_isPlayerLevelUp)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (!m_isPauseGame)
                {
                    CreatePausePopup();
                }
                else
                {
                    DestroyPausePopup();
                }
            }
        }

        if (cMapManager.GetInstance.m_eMapState == cMapManager.eMapState.NORMAL)
        {
            m_expHUD.gameObject.SetActive(false);
        }
        else
        {
            m_expHUD.gameObject.SetActive(true);

            m_ExpBarImage.fillAmount = (float)m_player.m_nExp / (float)m_player.m_nMaxExp;

            
            m_LevelText.text = "Lv." + cActorManager.GetInstance.m_Player.m_nLevel;
        }

        m_HpImage.fillAmount = (float)m_player.m_nHp / (float)m_player.m_nMaxHp;
        m_HpText.text = "" + m_player.m_nHp;
    }

    public void PlayerSkillButtonClick()
    {
        cActorManager.GetInstance.m_Player.PlayerSkill();
    }


    public void CreatePausePopup()
    {
        cSoundManager.GetInstance.PlayButtonSound();
        m_isPauseGame = true;
        m_pauseButton.SetActive(false);
        Time.timeScale = 0;
        GameObject obj = m_optionPopupPrefab;
        obj = Instantiate(obj, this.transform);
        obj.transform.localPosition = Vector3.zero;

        m_StackPausePopup.Push(obj);
    }

    public void CreatePlayerDiePopup()
    {
        cSoundManager.GetInstance.SetBGMVolume(0.2f);

        m_isPauseGame = true;
        m_pauseButton.SetActive(false);
        Time.timeScale = 0;
        GameObject obj = m_PlayerDiePopupPrefab;
        obj = Instantiate(obj, this.transform);
        obj.transform.localPosition = Vector3.zero;

        m_StackPlayerDiePopup.Push(obj);
    }

    public void CreateReturnTitlePopup()
    {
        m_isPauseGame = true;
        m_pauseButton.SetActive(false);
        Time.timeScale = 0;
        GameObject obj = m_returnTitlePopupPrefab;
        obj = Instantiate(obj, this.transform);
        obj.transform.localPosition = Vector3.zero;

        m_StackReturnTitlePopup.Push(obj);
    }

    public void CreateAbilitySelectPopup()
    {
        m_isPlayerLevelUp = true;
        m_isPauseGame = true;
        m_pauseButton.SetActive(false);
        Time.timeScale = 0;
        GameObject obj = m_AbilitySelectPopupPrefab;
        obj = Instantiate(obj, this.transform);
        obj.transform.localPosition = Vector3.zero;

        m_StackAbilitySelectPopup.Push(obj);
    }

    public void CreateEndingPopup()
    {
        m_pauseButton.SetActive(false);
        Time.timeScale = 0;
        GameObject obj = m_EndingPopupPrefab;
        obj = Instantiate(obj, this.transform);
        obj.transform.localPosition = Vector3.zero;

        m_StackEndingPopup.Push(obj);
    }

    public void DestroyPausePopup()
    {
        m_isPauseGame = false;
        m_pauseButton.SetActive(true);

        Time.timeScale = 1;
        if (m_StackPausePopup.Count > 0)
        {
            Destroy(m_StackPausePopup.Pop());
        }
    }

    public void DestroyReturnTitlePopup()
    {
        if (m_StackReturnTitlePopup.Count > 0)
        {
            Destroy(m_StackReturnTitlePopup.Pop());
        }
    }

    public void DestroyAbilitySelectPopup()
    {
        m_isPlayerLevelUp = false;
        m_isPauseGame = false;
        m_pauseButton.SetActive(true);

        Time.timeScale = 1;
        if (m_StackAbilitySelectPopup.Count > 0)
        {
            Destroy(m_StackAbilitySelectPopup.Pop());
        }
    }

    public void DestroyEndingPopup()
    {
        Time.timeScale = 1;
        if (m_StackEndingPopup.Count > 0)
        {
            Destroy(m_StackEndingPopup.Pop());
        }
    }

    public void DestroyPlayerDiePopup()
    {
        Time.timeScale = 1;

        m_isPauseGame = false;
        m_pauseButton.SetActive(true);

        if (m_StackPlayerDiePopup.Count > 0)
        {
            Destroy(m_StackPlayerDiePopup.Pop());
        }
    }

}
