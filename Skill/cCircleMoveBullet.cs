using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cCircleMoveBullet : cBaseSkill
{
    public Transform m_vecTargetTransform;

    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.GolemBulletExplosion;
        m_fMaxExplosionTime = 0.5f;
    }

    protected override void OnEnable()
    {
        m_nCollisionCount = 0;
        m_nDamage = 70;
        m_fSpeed = 3.0f;
        m_fTime = 0.0f;
        m_fMaxTime = 5.0f;
        m_sphereCollider.isTrigger = false;
        m_oRigidbody3d.velocity = Vector3.zero;
    }

    protected override void Update()
    {
        base.Update();
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        this.transform.Rotate(new Vector3(5, 5, 5));
        transform.RotateAround(m_vecTargetTransform.position, Vector3.up, m_fSpeed);
    }
}



















