using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cLichBullet : cBaseSkill
{
    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.LichExplosion;
    }

	protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_nDamage = 50;
        m_fSpeed = 8.0f;

        base.OnEnable();
    }

    protected override void Update()
    {
        base.Update();
    }
}
