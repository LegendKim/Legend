using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cLightningBullet : cBaseSkill
{
    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.LightningExplosion;
        m_fMaxExplosionTime = 1f;
    }

	protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_fSpeed = 10.0f;
        base.OnEnable();
    }
}
