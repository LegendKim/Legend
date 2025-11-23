using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMeteorAttackZone : cBaseSkill
{
    public BoxCollider m_boxCollider;

    protected override void Awake()
    {
    }


    protected override void OnEnable()
    {
        m_fTime = 0.0f;
        m_fMaxTime = 0.5f;
        m_nCollisionCount = 0;
        m_nDamage = 500;
        m_fSpeed = 10.0f;

    }
    protected override void Update()
    {
        m_fTime += Time.deltaTime;
        if (m_fTime > m_fMaxTime)
        {
            m_fTime = 0;
            m_isPass = false;
            m_isWallReflect = false;
            this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }
    }


    public override void CollisionMonster(Collision collision)
    {
        if(collision.gameObject.GetComponent<cPlayer>())
        {
            if(!collision.gameObject.GetComponent<cPlayer>().m_isInvincible)
            {
                cExplosionEffect effect = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.DragonBreathExplosion]).GetComponent<cExplosionEffect>();
                effect.m_fMaxTime = 1.0f;
                effect.InitPosition(collision.gameObject.transform.position);
                effect.gameObject.SetActive(true);
            }
        }
    }

    public override void CollisionWall()
    {
    }
}
