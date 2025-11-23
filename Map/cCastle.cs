using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cCastle : cBlock
{
    public Animator m_animator;

    void Start()
    {
        m_animator = this.GetComponent<Animator>();
    }
    public void OpenDoor()
    {
        m_animator.SetTrigger("OpenDoor");
    }

}
