using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMeteor : cBaseSkill
{
   protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.DragonMeteorExplosion;
        m_fMaxExplosionTime = 2.0f;
    }

    protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_nDamage = 300;
        m_fSpeed = 4.0f;
        m_fMaxTime = 5.0f;
        base.OnEnable();
    }

    protected override void Update()
    {
        if (this.transform.position.y < 0.0f && m_nCollisionCount < 1)
        {
            cSoundManager.GetInstance.SetBGMVolume(1.0f);
            CollisionWall();
            m_nCollisionCount++;
        }

        base.Update();
    }

    public override void CollisionWall()
    {
        cMeteorAttackZone attackZone = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.DragonMeteorAttackZone]).GetComponent<cMeteorAttackZone>();
        attackZone.Initialization(new Vector3(0, 0, 0), new Vector3(0, 0, 0), false, false, false);
        attackZone.gameObject.SetActive(true);
        base.CollisionWall();
    }
}
