using UnityEngine;

public class cFireBall : cBaseSkill
{
    public GameObject m_FireEffect;

    protected override void Awake()
    {
        base.Awake();
        m_explosionType = cObjectPoolManager.Type.prefabExplosion;
        m_fMaxExplosionTime = 1.5f;
        m_fSpeed = 13;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        m_nDamage = cActorManager.GetInstance.m_Player.m_nDamage;
       
        if(m_isFireShot)
        {
            m_FireEffect.SetActive(true);
        }
        else
        {
            m_FireEffect.SetActive(false);
        }
    }

    protected override void Update()
    {
        m_fTime += Time.deltaTime;
        if (m_fTime > 3f)
        {
            m_fTime = 0;
            m_oRigidbody3d.velocity = Vector3.zero;
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }
    }
}
