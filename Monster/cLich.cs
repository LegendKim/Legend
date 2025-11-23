using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cLich : cMonster
{
    public enum eBossPattern
    {
        IDLE,
        SPELL,
    }

    private float m_fActionTime;
    private float m_fPatternTime;
    private float m_fMaxPatternTime;
    public GameObject m_MagicZone;
    public Transform m_attackTransform;
    public eBossPattern m_eBossPattern;

    public override void Start()
    {
        type = eMonsterType.Lich;
        base.Start();
        m_eBossPattern = eBossPattern.IDLE;

        m_fActionTime = 0.0f;
        m_fPatternTime = 0.0f;
        m_fMaxPatternTime = Random.Range(2.0f, 3.0f);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Update()
    {
        base.Update();

        if(m_eBossPattern == eBossPattern.IDLE)
        {
            m_fPatternTime += Time.deltaTime;
        }

        if (m_eBossPattern == eBossPattern.SPELL)
        {
            ShotPattern();
        }


        if (m_fPatternTime >= m_fMaxPatternTime)
        {
            m_fPatternTime = 0.0f;
            m_fMaxPatternTime = Random.Range(2.0f, 3.0f);
            m_animator.SetTrigger("ShotBullet");

            m_eBossPattern = eBossPattern.SPELL;
        }


    }

    void ShotPattern()
    {
        m_fActionTime += Time.deltaTime;
        m_MagicZone.SetActive(true);

        if(m_fActionTime >= 0.5f)
        {
            m_fActionTime = 0.0f;
            ShotBullet(m_vDirection);
            m_eBossPattern = eBossPattern.IDLE;
        }
       
    }

    void ShotBullet(Vector3 dir)
    {
        cLichBullet rock = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.LichBullet]).GetComponent<cLichBullet>();
        rock.Initialization(m_attackTransform.position, dir, false, false, false);
        rock.gameObject.SetActive(true);
    }

}
