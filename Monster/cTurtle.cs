using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cTurtle : cMonster
{
    public float m_fShieldReadyTime;
    public float m_fMaxShieldReadyTime;
    public float m_fShieldTime;
    public bool m_isShield;


    protected override void Awake()
    {
        base.Awake();
        m_fStrongValue = 1.0f;
    }
    public override void Start()
    {
        type = eMonsterType.Turtle;
        base.Start();
        m_fShieldReadyTime = 0.0f;
        m_fMaxShieldReadyTime = Random.Range(2.0f, 4.0f);
        m_fShieldTime = 0.0f;
        m_isShield = false;
    }

    protected override void FixedUpdate()
    {
        if (!m_isShield)
        {
            if (m_eState != eMonsterState.DIE)
            {
                m_vDirection = (m_tTarget.position - this.transform.position).normalized;
                MonsterAStarMove();
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        if(!m_isShield)
        {
            m_fShieldReadyTime += Time.deltaTime;
            if (m_fShieldReadyTime > m_fMaxShieldReadyTime)
            {
                m_fMaxShieldReadyTime = Random.Range(2.0f, 4.0f);
                m_isShield = true;
                m_fShieldReadyTime = 0.0f;
                m_animator.SetBool("Move", false);
                m_animator.SetBool("Shield", true);

            }
        }
        else
        {
            m_fShieldTime += Time.deltaTime;
            if (m_fShieldTime > 3.0f)
            {
                m_isShield = false;
                m_fShieldTime = 0.0f;
                m_animator.SetBool("Shield", false);
                m_animator.SetBool("Move", true);
            }
        }

    }

    protected override void CollisionPlayerBullet(Collision collision)
    {
        if(!m_isShield)
        {
            int damage = collision.gameObject.GetComponent<cBaseSkill>().m_nDamage;

            int randDamage = Random.Range((int)(damage - (damage / 8.0f)), (int)(damage + (damage / 8.0f)));

            int cri = Random.Range(1, 100);

            bool isCritical = false;

            if (cri < cActorManager.GetInstance.m_Player.m_nCritical)
            {
                isCritical = true;
            }

            if (isCritical)
            {
                randDamage *= 2;
            }

            m_nHp -= randDamage;
            DamageTextSetting(randDamage, isCritical, false);
        }
    }

}
