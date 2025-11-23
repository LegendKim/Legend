using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cWarrior : cPlayer
{
    GameObject m_Shield;
    public float m_fSkillTime;
    public float m_fMaxSkillTime;
    public bool m_isShield;

    protected override void Awake()
    {
        base.Awake();
        m_isShield = false;
        m_fSkillTime = 0.0f;
        m_fMaxSkillTime = 3.0f;
        m_nSkillCount = 2;
        m_fSkillCoolTime = 0.0f;
        m_fMaxSkillCoolTime = 8.0f;
        m_Shield = this.transform.Find("Shield").gameObject;
    }

    void Start()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerSkill();
        }

        if (m_isShield)
        {
            m_fSkillTime += Time.deltaTime;

            if (m_fSkillTime > m_fMaxSkillTime)
            {
                m_isShield = false;
                m_Shield.SetActive(false);
                m_fSkillTime = 0.0f;
            }
        }

    }

    public override void PlayerSkill()
    {
        if (m_nSkillCount > 0 && !m_isShield)
        {
            m_nSkillCount--;

            m_Shield.SetActive(true);

            m_isShield = true;
        }

    }

    protected override void OnCollisionStay(Collision collision)
    {
        if (cMapManager.GetInstance.m_eMapState == cMapManager.eMapState.CLEAR &&
          collision.gameObject.layer == LayerMask.NameToLayer("MapCastle"))
        {
            if (cMapManager.GetInstance.m_nMapNum > 29)
            {
                if (m_nEndingCount < 1)
                {
                    cUIManager.GetInstance.CreateEndingPopup();
                    m_nEndingCount++;
                }
            }
            else
            {
                cMapManager.GetInstance.NextStageSetting();
                cActorManager.GetInstance.ActorPositionSetting();
            }

        }

        if (!m_isInvincible && collision.gameObject.layer == LayerMask.NameToLayer("MapTrap"))
        {
            if(!m_isShield)
            {
#if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
#endif
                cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerGetHit);
                m_nHp -= collision.gameObject.GetComponent<cBlock>().m_nDamage;
            }
            else
            {
                m_nHp -= (int)(collision.gameObject.GetComponent<cBlock>().m_nDamage * 0.3f);
            }
            m_isInvincible = true;
        }

        if (!m_isInvincible && collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            if (!m_isShield)
            {
#if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
#endif
                cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerGetHit);
                m_nHp -= collision.gameObject.GetComponent<cMonster>().m_nDamage;
            }
            else
            {
                m_nHp -= (int)(collision.gameObject.GetComponent<cMonster>().m_nDamage * 0.3f);
            }
          
            m_isInvincible = true;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("MonsterBullet"))
        {

            collision.gameObject.GetComponent<cBaseSkill>().CollisionMonster(collision);

            if (!m_isInvincible)
            {
                if (!m_isShield)
                {
#if UNITY_ANDROID || UNITY_IOS
                    Handheld.Vibrate();
#endif
                    cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerGetHit);
                    m_nHp -= collision.gameObject.GetComponent<cBaseSkill>().m_nDamage;
                }
                else
                {
                    m_nHp -= (int)(collision.gameObject.GetComponent<cBaseSkill>().m_nDamage * 0.3f);
                }
                
            }
            m_isInvincible = true;
        }


    }

}
