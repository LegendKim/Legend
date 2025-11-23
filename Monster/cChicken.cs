using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cChicken : cMonster
{
    public enum ePattern
    {
        IDLE,
        MOVE,
        TURN_HEAD,
        EAT
    };

    public ePattern m_ePattern;

    public Vector3 m_DestPosition;

    public float m_fActionTime;

    public float m_fMaxActionTime;

    public Rigidbody m_Rigidbody;

    protected override void Awake()
    {
        m_oldQuaternion = this.transform.rotation;
        m_fStrongValue = 1.0f;
        m_isFireState = false;
        m_fFireStateTime = 0.0f;
        m_animator = this.transform.Find("ChickenBody").GetComponent<Animator>();
        m_Rigidbody = this.GetComponent<Rigidbody>();
    }


    public override void Start()
    {
        type = eMonsterType.Chicken;
        base.Start();
        this.transform.Find("ChickenBody").rotation = Quaternion.Euler(0, 0, 0);
        m_ePattern = ePattern.MOVE;
        
        AnimationSetting();
        m_DestPosition = this.transform.position;
        m_DestPosition.x = Random.Range(m_DestPosition.x - 2.0f, m_DestPosition.x + 2.0f);
        m_DestPosition.z = Random.Range(m_DestPosition.z - 2.0f, m_DestPosition.z + 2.0f);
        m_vDirection = (m_DestPosition - this.transform.position).normalized;
        this.transform.LookAt(this.transform.position + m_vDirection);

        m_fActionTime = 0.0f;
        m_fMaxActionTime = 4.0f;
    }

    protected override void FixedUpdate()
    {
        if(m_eState != eMonsterState.DIE)
        {
            if (m_ePattern == ePattern.MOVE)
            {

                if (Vector3.Distance(this.transform.position, m_DestPosition) > 0.1f)
                {
                    this.transform.position += m_vDirection * m_fSpeed * Time.deltaTime;
                }
            }

            m_fActionTime += Time.deltaTime;

            if (m_fActionTime > m_fMaxActionTime)
            {
                m_fActionTime = 0.0f;
                m_fMaxActionTime = Random.Range(2.0f, 4.0f);
                int patternNum = (int)m_ePattern + 1;
                if (patternNum > 3) patternNum = 1;
                m_ePattern = (ePattern)patternNum;
                if (m_ePattern == ePattern.MOVE)
                {
                    m_fMaxActionTime = 3.0f;
                    m_DestPosition = this.transform.position;
                    m_DestPosition.x = Random.Range(m_DestPosition.x - 2.0f, m_DestPosition.x + 2.0f);
                    m_DestPosition.z = Random.Range(m_DestPosition.z - 2.0f, m_DestPosition.z + 2.0f);
                    m_vDirection = (m_DestPosition - this.transform.position).normalized;
                    this.transform.LookAt(this.transform.position + m_vDirection);
                }

                AnimationSetting();
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void AnimationSetting()
    {
        switch(m_ePattern)
        {
            case ePattern.IDLE:
                m_animator.SetTrigger("Idle");
                break;
            case ePattern.MOVE:
                m_animator.SetTrigger("Walk");
                break;
            case ePattern.TURN_HEAD:
                m_animator.SetTrigger("Turn_Head");
                break;
            case ePattern.EAT:
                m_animator.SetTrigger("Eat");
                break;
        }
    }
}
