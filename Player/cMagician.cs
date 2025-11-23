using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMagician : cPlayer
{
    GameObject m_MagicZone;
    public float m_fSkillTime;
    public float m_fMaxSkillTime;
    public bool m_isLightning;

    protected override void Awake()
    {
        base.Awake();
        m_isLightning = false;
        m_fSkillTime = 0.0f;
        m_fMaxSkillTime = 2.0f;
        m_nSkillCount = 2;
        m_fSkillCoolTime = 0.0f;
        m_MagicZone = this.transform.Find("MagicZone").gameObject;
    }

    void Start()
    {
        m_fMaxSkillCoolTime = 9.0f;
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

        if (m_isLightning)
        {
            m_fSkillTime += Time.deltaTime;

            if (m_fSkillTime > m_fMaxSkillTime)
            {
                m_isLightning = false;
                m_MagicZone.SetActive(false);
                m_fSkillTime = 0.0f;
            }
        }

    }

    public override void PlayerSkill()
    {
        if (m_nSkillCount > 0 && !m_isLightning)
        {
            m_nSkillCount--;
            m_MagicZone.SetActive(true);
            m_isLightning = true;

            cMagicianLightning lightning = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.MagicianLightning]).GetComponent<cMagicianLightning>();

            Vector3 pos = this.transform.position;
            pos.y = 0.55f;

            lightning.transform.position = pos;
            lightning.m_nDamage = this.m_nDamage / 3;
            lightning.gameObject.SetActive(true);

        }

    }





}
