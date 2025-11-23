using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cCoolTimeImage : MonoBehaviour
{
    public Image m_CoolTimeImage;
    public cPlayer m_Player;

    void Start()
    {
        m_Player = GameObject.FindWithTag("Player").GetComponent<cPlayer>();
    }

    void Update()
    {
        if (m_Player.m_nSkillCount == 0)
        {
            m_CoolTimeImage.fillAmount = (m_Player.m_fSkillCoolTime / m_Player.m_fMaxSkillCoolTime / 2.0f);


        }
        else if (m_Player.m_nSkillCount == 1)
        {
            m_CoolTimeImage.fillAmount = 0.5f + ((m_Player.m_fSkillCoolTime / m_Player.m_fMaxSkillCoolTime /2.0f));
            
        }
        else
        {
            m_CoolTimeImage.fillAmount = 1.0f;
        }
    }
}
