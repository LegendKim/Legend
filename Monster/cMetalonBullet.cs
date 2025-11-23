using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMetalonBullet : cBaseSkill
{
    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.MetalonExplosion;
    }
    protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_nDamage = 70;
        m_fSpeed = 10.0f;

        base.OnEnable();
    }

    protected override void Update()
    {
        if(this.transform.position.y < 0.3f)
        {
            CollisionWall();
        }
        base.Update();
    }
}
