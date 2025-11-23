using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cRedCyclopes : cMonster
{
    protected override void Awake()
    {
        base.Awake();
        m_fStrongValue = 1.0f;
    }

    public override void Start()
    {
        type = eMonsterType.RedCyclopes;
        base.Start();
        m_animator.SetBool("Run", true);
    }

    protected override void FixedUpdate()
    {
        if (m_eState != eMonsterState.DIE)
        {
            m_vDirection = (m_tTarget.position - this.transform.position).normalized;
            MonsterAStarMove();
        }
    }
}
