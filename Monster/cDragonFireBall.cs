using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cDragonFireBall : cBaseSkill
{
    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.DragonFireExplosion;
        m_fMaxExplosionTime = 1f;
    }

    protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_nDamage = 50;
        m_fSpeed = 6.0f;

        base.OnEnable();
    }
}
