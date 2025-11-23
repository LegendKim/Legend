using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cDragon : cMonster
{
    public enum eBossPattern
    {
        IDLE,
        SIDE_FIRE,
        BREATH,
        SPELL_FRONT,
        SPELL_UP,
        SPELL_PORTAL,
        FLY_METEOR,
        
    }

    private float m_fActionTime;
    private float m_fPatternTime;
    private float m_fMaxPatternTime;
    public eBossPattern m_eBossPattern;
    public GameObject m_MagicZone;

    public GameObject m_BreathZone;
    public GameObject m_SideFireZone;
    public GameObject m_MeteorZone;

    public GameObject m_ChargeEffect;

    public GameObject m_BreathAttackExplosion;

    public Transform m_attackTransform;

    private CapsuleCollider m_capsuleCollider;

    public Vector3 m_IdlePosition;
    public Vector3 m_FlyPosition;
    public Vector3[] m_SpellTargetPosition;

    public float m_fMeteorSound;

    public AudioSource m_AudioSource;

    public AudioClip m_DragonMeteorSound;
    public AudioClip m_DragonDieSound;

    public cObjectPoolManager pool => cObjectPoolManager.GetInstance;

    public override void Start()
    {
        type = eMonsterType.Dragon;
        base.Start();
        m_fMaxDieTime = 3.0f;
        m_AudioSource = this.GetComponent<AudioSource>();
        m_capsuleCollider = this.GetComponent<CapsuleCollider>();
        m_fMeteorSound = 0.0f;
        m_SpellTargetPosition = new Vector3[5];
        m_IdlePosition = this.transform.position;

        m_fActionTime = 0.0f;
        m_fPatternTime = 0.0f;
        m_fMaxPatternTime = Random.Range(1.5f, 2.0f);
    }

    protected override void FixedUpdate()
    {
    }

    protected override void Update()
    {
        base.Update();

        if (m_eState != eMonsterState.DIE)
        {
            
            if (m_eBossPattern == eBossPattern.IDLE)
            {
                m_fPatternTime += Time.deltaTime;
            }

            if (m_fPatternTime >= m_fMaxPatternTime)
            {
                m_fPatternTime = 0.0f;
                m_fMaxPatternTime = Random.Range(2.0f, 3.0f);

                int randNum = 0;
                if (m_nHp < m_nMaxHp / 2)
                {
                    randNum = Random.Range(1, 7);
                }
                else
                {
                    randNum = Random.Range(1, 6);
                }

                m_eBossPattern = (eBossPattern)randNum;


                BossAnimation();

            }
            BossPatternUpdate();
        }
        
    }

    void BossPatternUpdate()
    {
        switch (m_eBossPattern)
        {
            case eBossPattern.IDLE:
                Idle();
                break;
            case eBossPattern.SIDE_FIRE:
                SideFire();
                break;
            case eBossPattern.BREATH:
                Breath();
                break;
            case eBossPattern.SPELL_FRONT:
                SpellFront();
                break;
            case eBossPattern.SPELL_UP:
                SpellUp();
                break;
            case eBossPattern.SPELL_PORTAL:
                SpellPortal();
                break;
            case eBossPattern.FLY_METEOR:
                FlyMeteor(); 
                break;
  
        }
    }


    void BossAnimation()
    {
        switch (m_eBossPattern)
        {
            case eBossPattern.IDLE:
                m_MagicZone.SetActive(false);
                m_animator.SetBool("Idle", true);
                m_IdlePosition.y = 0.5f;
                break;

            case eBossPattern.SIDE_FIRE:
                m_SideFireZone.SetActive(true);
                m_animator.SetTrigger("SideFire");
                m_MagicZone.SetActive(true);
                break;
            case eBossPattern.BREATH:
                m_BreathAttackExplosion.SetActive(false);
                m_BreathZone.SetActive(true);
                m_animator.SetTrigger("Breath");
                m_MagicZone.SetActive(true);
                break;
            case eBossPattern.SPELL_FRONT:
                m_BreathAttackExplosion.SetActive(false);
                m_animator.SetTrigger("SpellFront");
                m_MagicZone.SetActive(true);
                break;
            case eBossPattern.SPELL_UP:
                CreateShadowZone();
                m_animator.SetTrigger("SpellUp");
                m_MagicZone.SetActive(true);
                break;
            case eBossPattern.SPELL_PORTAL:
                m_animator.SetTrigger("SpellPortal");
                m_MagicZone.SetActive(true);
                break;
            case eBossPattern.FLY_METEOR:
                m_MeteorZone.SetActive(true);
                m_ChargeEffect.SetActive(true);
                m_MagicZone.SetActive(false);
                m_FlyPosition = this.transform.position;
                m_FlyPosition.y = 3.0f;
                m_fMeteorSound = 0.0f;
                m_animator.SetBool("Fly", true);
                break;
        }
    }

    void Idle()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 5.0f)
        {
            m_fActionTime = 0.0f;
            m_eBossPattern = eBossPattern.IDLE;
        }
        else
        {
            m_capsuleCollider.gameObject.transform.position = Vector3.Lerp(this.transform.position, m_IdlePosition, Time.deltaTime * 5.0f); 
            Vector3.Lerp(this.transform.position, m_IdlePosition, Time.deltaTime * 5.0f);
        }

    }

    void FlyMeteor()
    {
        m_fActionTime += Time.deltaTime;
        
        if(m_fMeteorSound >= 3.9f)
        {
            m_fMeteorSound = 3.9f;
        }
        else
        {
            m_fMeteorSound += Time.deltaTime;
        }

        if (m_fActionTime > 5.0f)
        {
            m_fActionTime = 0.0f;
            m_fMeteorSound = 3.0f;
            m_animator.SetTrigger("Meteor");
        }
        else
        {
            // 메테오 소리!!
            cSoundManager.GetInstance.SetBGMVolume(1.0f - m_fMeteorSound * 0.2f);
            m_capsuleCollider.gameObject.transform.position = Vector3.Lerp(this.transform.position, m_FlyPosition, Time.deltaTime * 3.0f);
            Vector3.Lerp(this.transform.position, m_FlyPosition, Time.deltaTime * 3.0f);
        }
    }



    void SideFire()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 5.0f)
        {
            m_MagicZone.SetActive(false);
            m_fActionTime = 0.0f;
            m_eBossPattern = eBossPattern.IDLE;
            
        }
    }

    void Breath()
    {
        m_fActionTime += Time.deltaTime;


        if (m_fActionTime > 3.0f)
        {
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
            
        }
    }

    void SpellFront()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 3.0f)
        {

            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }
    }

    void SpellPortal()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 3.0f)
        {
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }
    }

    void SpellUp()
    {
        m_fActionTime += Time.deltaTime;

        if (m_fActionTime > 3.0f)
        {
            m_fActionTime = 0.0f;
            m_MagicZone.SetActive(false);
            m_eBossPattern = eBossPattern.IDLE;
        }

    }

    void ShotSpellUp()
    {
        for (int i = 0; i < m_SpellTargetPosition.Length; ++i)
        {
            Vector3 pos = m_SpellTargetPosition[i];
            pos.y += 10.0f;
            Vector3 dir = new Vector3(0, -1, 0);
            CreateBullet(pos, dir);
        }
    }

    void ShotSpellPortal()
    {
        for (int i = 0; i < 5; ++i)
        {
            Vector3 targetPos = new Vector3(0, 0, 0);
            targetPos.x = Random.Range(2, 13);
            targetPos.z = Random.Range(8, 15);
            targetPos.y = 2.0f;
            PortalZone(targetPos, i);
        }
       
    }

    void CreateShadowZone()
    {
        Vector3 pos = m_tTarget.position;
        pos.y = 1.2f;
        SpellZone(pos, 0);

        for(int i=1; i < 5; ++i)
        {
            Vector3 targetPos = new Vector3(0,0,0);
            targetPos.x = Random.Range(3, 12);
            targetPos.z = Random.Range(3, 15);
            targetPos.y = 1.5f;
            SpellZone(targetPos, i);
        }


    }

    void PortalZone(Vector3 pos, int index)
    {
        Vector3 dir = m_tTarget.position - pos;


        cDragonPortal portalZone = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonPortal]).GetComponent<cDragonPortal>();
        portalZone.m_fMaxTime = 1.0f;
        portalZone.InitPosition(pos);
        portalZone.gameObject.transform.LookAt(m_tTarget.position);
        portalZone.m_shotDirection = dir;
        portalZone.gameObject.SetActive(true);
        m_SpellTargetPosition[index] = pos;
    }

    void SpellZone(Vector3 pos, int index)
    {

        cExplosionEffect attackZone =
	pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.AttackZone])
    .GetComponent<cExplosionEffect>();
        attackZone.m_fMaxTime = 1.0f;
        attackZone.InitPosition(pos);
        attackZone.gameObject.SetActive(true);
        m_SpellTargetPosition[index] = pos;
    }

    void ShotMeteor()
    {
        m_AudioSource.clip = m_DragonMeteorSound;
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        m_eBossPattern = eBossPattern.IDLE;
        m_animator.SetBool("Fly", false);

        m_ChargeEffect.SetActive(false);
        m_MeteorZone.SetActive(false);

        cMeteor meteor = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonMeteor]).GetComponent<cMeteor>();

        meteor.Initialization(new Vector3(7,10,8), new Vector3(0, -1, 0), false, false, false);
        meteor.gameObject.SetActive(true);
    }

    void ShotFireBall()
    {
        cExplosionEffect effect =pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.GolemExplosion]).GetComponent<cExplosionEffect>();

        effect.InitPosition(m_attackTransform.position);
        effect.transform.rotation = Quaternion.Euler(-180, 0, 0);
        effect.gameObject.SetActive(true);

        Vector3 dir = new Vector3(0, 0, -1);
        CreateFireBall(dir);

        dir = Quaternion.Euler(0, 80, 0) * dir;
        CreateFireBall(dir);

        dir = new Vector3(0, 0, -1);
        dir = Quaternion.Euler(0, -80, 0) * dir;
        CreateFireBall(dir);

        dir = new Vector3(0, 0, -1);
        dir = Quaternion.Euler(0, 30, 0) * dir;
        CreateFireBall(dir);

        dir = new Vector3(0, 0, -1);
        dir = Quaternion.Euler(0, -30, 0) * dir;
        CreateFireBall(dir);

    }

    void ShotBreath()
    {
        m_BreathZone.SetActive(false);

        cExplosionEffect effect =pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.GolemExplosion]).GetComponent<cExplosionEffect>();

        effect.InitPosition(m_attackTransform.position);
        effect.transform.rotation = Quaternion.Euler(-180, 0, 0);
        effect.gameObject.SetActive(true);

        cDragonBreath breath = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonBreath]).GetComponent<cDragonBreath>();
        breath.gameObject.transform.position = m_attackTransform.position;
        breath.gameObject.SetActive(true);

        breath = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonBreath]).GetComponent<cDragonBreath>();
		breath.gameObject.transform.position = new Vector3(2, 1.5f, 16);
        breath.gameObject.SetActive(true);

        breath = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonBreath]).GetComponent<cDragonBreath>();
		breath.gameObject.transform.position = new Vector3(13, 1.5f, 16);
        breath.gameObject.SetActive(true);
    }

    void ShotSideFire()
    {
        m_SideFireZone.SetActive(false);
        cDragonBreath breath = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonBreath]).GetComponent<cDragonBreath>();

        breath.gameObject.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
       breath.gameObject.transform.position = new Vector3(1f, 1.5f, 2.5f);
        breath.gameObject.SetActive(true);

        breath = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonBreath]).GetComponent<cDragonBreath>();

        breath.gameObject.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        breath.gameObject.transform.position = new Vector3(14f, 1.0f, 2.5f);
        breath.gameObject.SetActive(true);
    }

    void CreateBullet(Vector3 pos, Vector3 dir)
    {
        cDragonShadowBullet shadowBullet = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonShadowBullet]).GetComponent<cDragonShadowBullet>();
        shadowBullet.Initialization(pos, dir, false, false, false);
        shadowBullet.gameObject.SetActive(true);
    }

    void CreateFireBall(Vector3 dir)
    {
        cDragonFireBall fireBall = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.DragonFireBall]).GetComponent<cDragonFireBall>();

        Vector3 pos = m_attackTransform.position;
        pos.z += 2.0f;
        pos.y = 1.5f;

        fireBall.Initialization(pos, dir, false, false, false);
        fireBall.gameObject.SetActive(true);
    }

    protected override void MonsterDieStand()
    {
        base.MonsterDieStand();
        m_AudioSource.clip = m_DragonDieSound;
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
    }

    protected override void MonsterDie()
    {
        m_fDieTime = 0.0f;
        m_eState = eMonsterState.IDLE;
        this.transform.rotation = m_oldQuaternion;

        cExplosionEffect effect = pool.GetObject(pool.m_ObjectDic[cObjectPoolManager.Type.MonsterDeath]).GetComponent<cExplosionEffect>();
        effect.m_fMaxTime = 1.0f;

        Vector3 pos = this.transform.position;
        pos.y = 1.5f;

        effect.gameObject.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        effect.InitPosition(pos);
        effect.gameObject.SetActive(true);

        InitExpOrb();
        pool.SetActiveFalse(this.gameObject);
    }
}
