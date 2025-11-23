using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class cAbilitySelectPanel : MonoBehaviour
{
    cPlayer m_Player;

    public Button[] m_arrButton;

    public Text[] m_arrText;

    public GameObject m_SelectImage;

    int m_nSelectNum;
    int[] m_arrRandNum;

    // Start is called before the first frame update
    void Start()
    {
        m_arrRandNum = new int[3];
        m_Player = GameObject.FindWithTag("Player").GetComponent<cPlayer>();

        m_nSelectNum = 1;

        int startNum = (int)cAbility.eAbilityType.NONE + 1;
        int endNum = (int)cAbility.eAbilityType.ABILITY_COUNT;

        do
        {
            m_arrRandNum[0] = Random.Range(startNum, endNum);
            m_arrRandNum[1] = Random.Range(startNum, endNum);
            m_arrRandNum[2] = Random.Range(startNum, endNum);
        } while ((m_arrRandNum[0] == m_arrRandNum[1]) ||
                (m_arrRandNum[0] == m_arrRandNum[2]) ||
                (m_arrRandNum[1] == m_arrRandNum[2]));

        AbilityImageSetting(m_arrButton[0], m_arrText[0], m_arrRandNum[0]);
        AbilityImageSetting(m_arrButton[1], m_arrText[1], m_arrRandNum[1]);
        AbilityImageSetting(m_arrButton[2], m_arrText[2], m_arrRandNum[2]);

        SelectImagePositionSetting();

    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_nSelectNum--;
            if (m_nSelectNum < 0) m_nSelectNum = 2;
            SelectImagePositionSetting();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_nSelectNum++;
            if (m_nSelectNum > 2) m_nSelectNum = 0;
            SelectImagePositionSetting();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SelectAbility(m_arrRandNum[m_nSelectNum]);
        }
    }
    void AbilityImageSetting(Button button,Text buttonText, int num)
    {
        switch ((cAbility.eAbilityType)num)
        {
            case cAbility.eAbilityType.SHOT_DAMAGE:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotDamage");
                buttonText.text = "공격력 UP";
                break;
            case cAbility.eAbilityType.SHOT_SPEED:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotSpeed");
                buttonText.text = "공격 속도 UP";
                break;
            case cAbility.eAbilityType.SHOT_PASS:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotPass");
                buttonText.text = "관통";
                break;
            case cAbility.eAbilityType.SHOT_DIAGONAL:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotDiagonal");
                buttonText.text = "사선 화살";
                break;
            case cAbility.eAbilityType.SHOT_RIGHT_ANGLE:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotRightAngle");
                buttonText.text = "측면 화살";
                break;
            case cAbility.eAbilityType.SHOT_BACK:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotBack");
                buttonText.text = "후방 화살";
                break;
            case cAbility.eAbilityType.SHOT_WALL_REFLECT:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotWallReflect");
                buttonText.text = "벽 반사";
                break;
            case cAbility.eAbilityType.SHOT_FIRE:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/shotFire");
                buttonText.text = "파이어 샷";
                break;
            case cAbility.eAbilityType.HP_UP:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/hpUp");
                buttonText.text = "HP 회복";
                break;
            case cAbility.eAbilityType.MAXHP_UP:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/maxHpUp");
                buttonText.text = "최대 HP 증가";
                break;
            case cAbility.eAbilityType.CRITICAL_UP:
                button.image.sprite = Resources.Load<Sprite>("MyGame/UIImage/criticalUp");
                buttonText.text = "크리티컬 확률 증가";
                break;
        }


    }


    void SelectImagePositionSetting()
    {
        switch (m_nSelectNum)
        {
            case 0:
                m_SelectImage.transform.localPosition = new Vector3(-350, 0, 0);
                break;
            case 1:
                m_SelectImage.transform.localPosition = new Vector3(0, 0, 0);
                break;
            case 2:
                m_SelectImage.transform.localPosition = new Vector3(350, 0, 0);
                break;
        }
    }

    public void SelectLeft()
    {
        m_nSelectNum = 0;
        SelectAbility(m_arrRandNum[m_nSelectNum]);
    }

    public void SelectCenter()
    {
        m_nSelectNum = 1;
        SelectAbility(m_arrRandNum[m_nSelectNum]);
    }

    public void SelectRight()
    {
        m_nSelectNum = 2;
        SelectAbility(m_arrRandNum[m_nSelectNum]);
    }

    public void SelectAbility(int selectAbilityNum)
    {
        cSoundManager.GetInstance.PlayButtonSound();
        switch ((cAbility.eAbilityType)selectAbilityNum)
        {
            case cAbility.eAbilityType.SHOT_DAMAGE:
                m_Player.m_nDamage += 30;
                break;
            case cAbility.eAbilityType.SHOT_SPEED:
                m_Player.m_fMaxShotTime -= 0.2f;
                if(m_Player.m_fMaxShotTime < 0.2f)
                {
                    m_Player.m_fMaxShotTime = 0.2f;
                }
                m_Player.m_fAttackSpeed += 0.1f;
                if (m_Player.m_fAttackSpeed > 2.0f)
                {
                    m_Player.m_fMaxShotTime = 2.2f;
                }
                m_Player.m_animator.SetFloat("m_fAttackSpeed", m_Player.m_fAttackSpeed);
                break;
            case cAbility.eAbilityType.SHOT_PASS:
                m_Player.m_isPass = true;
                break;
            case cAbility.eAbilityType.SHOT_DIAGONAL:
                m_Player.m_isDiagonal = true;
                break;
            case cAbility.eAbilityType.SHOT_RIGHT_ANGLE:
                m_Player.m_isRightAngle = true;
                break;
            case cAbility.eAbilityType.SHOT_BACK:
                m_Player.m_isBack = true;
                break;
            case cAbility.eAbilityType.SHOT_WALL_REFLECT:
                m_Player.m_isWallReflect = true;
                break;
            case cAbility.eAbilityType.SHOT_FIRE:
                m_Player.m_isFireShot = true;
                break;
            case cAbility.eAbilityType.HP_UP:
                m_Player.m_nHp += (int)(m_Player.m_nMaxHp * 0.5f);
                if(m_Player.m_nHp >= m_Player.m_nMaxHp)
                {
                    m_Player.m_nHp = m_Player.m_nMaxHp;
                }

                break;
            case cAbility.eAbilityType.MAXHP_UP:
                m_Player.m_nMaxHp = (int)(m_Player.m_nMaxHp * 1.3f);
                m_Player.m_nHp += (int)(m_Player.m_nMaxHp * 0.2f);
                if (m_Player.m_nHp >= m_Player.m_nMaxHp)
                {
                    m_Player.m_nHp = m_Player.m_nMaxHp;
                }
                break;
            case cAbility.eAbilityType.CRITICAL_UP:
                m_Player.m_nCritical += 10;
                break;
        }
        DestroyAbilitySelectPanel();
    }

    public void DestroyAbilitySelectPanel()
    {
        cUIManager.GetInstance.DestroyAbilitySelectPopup();
    }
}
