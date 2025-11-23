using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBomb : cMonster
{
    public bool m_isBomb;

    protected override void Awake()
    {
        base.Awake();
        m_fStrongValue = 1.0f;
    }


    public override void Start()
    {
        type = eMonsterType.Bomb;
        base.Start();
        m_isBomb = false;
        m_animator.SetBool("Move", true);
    }

    protected override void FixedUpdate()
    {
        if (m_eState != eMonsterState.DIE)
        {
            //base.FixedUpdate();
            m_vDirection = (m_tTarget.position - this.transform.position).normalized;
        }
        if(!m_isBomb)
        {
            MonsterAStarMove();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if(!m_isBomb && Vector3.Distance(this.transform.position, m_tTarget.position) < 1.5f)
        {
            m_isBomb = true;
            
            m_animator.SetTrigger("Explosion");
        }

    }

    public void BombExplosion()
    {
        m_nHp = 0;
        m_eState = eMonsterState.DIE;
        m_fDieTime = 1.1f;

        cBombFire bombFire = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.BombFire]).GetComponent<cBombFire>();

        Vector3 pos = this.transform.position;
        pos.y = 1.0f;

        bombFire.gameObject.transform.position = pos;
        bombFire.gameObject.SetActive(true);
    }
}
