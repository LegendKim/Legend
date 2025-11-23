using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cGolemRock : cBaseSkill
{
    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.GolemRockExplosion;
        m_fMaxExplosionTime = 1f;
    }

    protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_nDamage = 60;
        m_fSpeed = 7.5f;

        base.OnEnable();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        this.transform.Rotate(new Vector3(6, 6, 6));
    }
}