using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBat : cMonster
{
    protected override void Awake()
    {
        base.Awake();
    }


    public override void Start()
    {
        type = eMonsterType.Bat;
        base.Start();
        this.transform.Find("bat").rotation = Quaternion.Euler(0, 0, 0);
    }

    protected override void FixedUpdate()
    {
        if(m_eState != eMonsterState.DIE)
        {
            base.FixedUpdate();
            this.transform.position += m_vDirection * m_fSpeed * Time.deltaTime;
        }
       
        
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            this.transform.position += collision.gameObject.GetComponent<cBaseSkill>().m_vecDirection;
        }
        
    }
}