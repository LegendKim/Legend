using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cAssassin : cPlayer
{
    float m_fTime;
    bool m_isBlink;

    protected override void Awake()
    {
        base.Awake();
        m_nSkillCount = 2;
        m_fSkillCoolTime = 0.0f;
    }

    void Start()
    {
        m_isBlink = false;
        m_fTime = 0.0f;
        m_fMaxSkillCoolTime = 2.5f;
    }

	protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerSkill();
        }

        if (m_isBlink)
        {
            m_fTime += Time.deltaTime * 3.0f;
        }


        if (m_fTime > 1.0f)
        {
            m_fTime = 0.0f;
            m_isBlink = false;
        }
    }

    public override void PlayerSkill()
    {
        if(m_nSkillCount > 0 && m_vecDir != Vector3.zero)
        {
            m_isBlink = true;
            m_nSkillCount--;
            cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.AssassinBlink]).GetComponent<cExplosionEffect>();
            effect.m_fMaxTime = 1.0f;
            Vector3 pos = this.transform.position;
            pos.y = 1.0f;
            effect.InitPosition(pos);
            effect.transform.LookAt(pos - m_vecDir);
            effect.gameObject.SetActive(true);
            //this.transform.position += m_vecDir.normalized * 40.0f * Time.deltaTime;
			m_rigidbody.MovePosition(m_rigidbody.position + m_vecDir.normalized * 40.0f);
		}
    }
}
