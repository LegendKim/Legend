using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cGolem : cMonster
{
    public enum eBossPattern
    {
        IDLE,
        WALK,
        ATTACK,
        SPELL,
    }

    private float m_fActionTime;
    private float m_fPatternTime;
    private float m_fMaxPatternTime;
    public eBossPattern m_eBossPattern;
    public GameObject m_MagicZone;
    public Transform m_attackTransform;


    // Start is called before the first frame update
    public override void Start()
    {
        type = eMonsterType.Golem;
        base.Start();

        m_fActionTime = 0.0f;
        m_fPatternTime = 0.0f;
        m_fMaxPatternTime = 2.5f;
    }

    protected override void FixedUpdate()
    {
    }

    protected override void Update()
    {
        base.Update();
        m_fPatternTime += Time.deltaTime;

        if (m_fPatternTime >= m_fMaxPatternTime)
        {
            m_fPatternTime = 0.0f;

            int randNum = Random.Range(1, 4);
            m_eBossPattern = (eBossPattern)randNum;
            BossAnimation();
        }

        BossPatternUpdate();
    }

    void BossPatternUpdate()
    {
        switch (m_eBossPattern)
        {
            case eBossPattern.IDLE:
                Idle();
                break;
            case eBossPattern.WALK:
                Walk();
                break;
            case eBossPattern.ATTACK:
                Attack();
                break; 
            case eBossPattern.SPELL:
                Spell();
                break;
        }
    }

    
    void BossAnimation()
    {
        switch (m_eBossPattern)
        {
            case eBossPattern.IDLE:
               
                m_MagicZone.SetActive(false);
                m_animator.SetBool("Walk", false);
                break;
            case eBossPattern.WALK:
                m_MagicZone.SetActive(false);
                m_animator.SetBool("Walk", true);
                break;
            case eBossPattern.ATTACK:
                transform.LookAt(m_tTarget.position);
                m_vDirection = (m_tTarget.position - this.transform.position).normalized;

                m_animator.SetBool("Walk", false);
                m_animator.SetTrigger("Attack");
                m_MagicZone.SetActive(true);
                break;
            case eBossPattern.SPELL:
                transform.LookAt(m_tTarget.position);
                m_vDirection = (m_tTarget.position - this.transform.position).normalized;

                m_animator.SetBool("Walk", false);
                m_animator.SetTrigger("Spell");
                m_MagicZone.SetActive(true);
                break;
        }
    }

    void Idle()
    {
        transform.LookAt(m_tTarget.position);
    }

    void Walk()
    {
        transform.LookAt(m_tTarget.position);
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 3.0f)
        {
            m_fActionTime = 0.0f;
            m_eBossPattern = eBossPattern.IDLE;
            m_animator.SetBool("Walk", false);
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, m_tTarget.position, Time.deltaTime * 0.4f);
        }
       
    }
    void Attack()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 3.0f)
        {
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }

    }

    void Spell()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 3.0f)
        {
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }

    }

    void BulletCreate()
    {
        cCircleMoveBullet moveBullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.GolemBullet]).GetComponent<cCircleMoveBullet>();
        Vector3 pos = this.transform.position + (m_vDirection.normalized) * 5.0f;
        pos.y = 1.0f;
        moveBullet.Initialization(pos, m_vDirection, false, false, false);
        moveBullet.m_vecTargetTransform = this.transform;
        moveBullet.gameObject.SetActive(true);

        moveBullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.GolemBullet]).GetComponent<cCircleMoveBullet>();
        pos = this.transform.position - (m_vDirection.normalized) * 5.0f;
        pos.y = 1.0f;
        moveBullet.Initialization(pos, m_vDirection, false, false, false);
        moveBullet.m_vecTargetTransform = this.transform;
        moveBullet.gameObject.SetActive(true);
    }

    void AttackExplosion()
    {
        Vector3 dir1 = m_vDirection;
        dir1 = Quaternion.Euler(0, -45, 0) * dir1;
        ShotRock(dir1);

        Vector3 dir2 = m_vDirection;
        dir2 = Quaternion.Euler(0, 45, 0) * dir2;
        ShotRock(dir2);

        ShotRock(m_vDirection);
        
        cExplosionEffect effect = 
        cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.GolemExplosion]).
        GetComponent<cExplosionEffect>();

        effect.InitPosition(m_attackTransform.position);
        effect.gameObject.SetActive(true);
    }

    void ShotRock(Vector3 dir)
    {
        cGolemRock rock = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.GolemRock]).GetComponent<cGolemRock>();
        Vector3 pos = m_attackTransform.position;
        pos.y = 1.0f;
        rock.Initialization(pos, dir, true, false, false);
        rock.gameObject.SetActive(true);
    }

}
