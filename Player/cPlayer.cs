using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ePlayerState
{
    IDLE,
    ATTACK,
    WALK,
    RUN,
    GET_HIT,
    DIE

}


public abstract class cPlayerSkill
{
    public Transform m_TransformFires;
    public abstract void Start();
    public abstract void Update();
    public abstract void Clear();

    public abstract void Shooting(Vector3 dir);
}
//기본형
public class cPlayerFireBall : cPlayerSkill
{
    public override void Clear()
    {
    }

    public override void Shooting(Vector3 dir)
    {
        dir = dir.normalized;

        cPlayer player = GameObject.FindWithTag("Player")
           .GetComponent<cPlayer>();


        GameObject bullet = cObjectPoolManager.GetInstance.GetObject(cObjectPoolManager.GetInstance.m_ObjectDic[cObjectPoolManager.Type.prefabFireBall]);
        if (bullet != null)
        {
            bullet.GetComponent<cFireBall>().
                Initialization(player.m_fireSkillTransform.position, dir, player.m_isWallReflect, player.m_isPass, player.m_isFireShot);
            bullet.gameObject.SetActive(true);
        }

    }

    public override void Start()
    {
    }

    public override void Update()
    {
    }
}



public class cPlayer : MonoBehaviour
{
    public Vector3 m_vecDir = Vector3.zero;
    public Vector3 m_vecDirOld = Vector3.zero;
    public Vector3 finalMousePosition = Vector3.zero;
    public Vector3 lookDirection;
    public float m_fSpeed = 0;
    const float m_fWalkSpeed = 1;
    const float m_fRunSpeed = 3;
    public Animator m_animator;
    protected Rigidbody m_rigidbody;
    public Transform m_fireSkillTransform;
    public Vector3 m_vTargetPosition;
    public cPlayerSkill m_playerSkill;
    public Camera m_camera;
    public float m_fSkillCoolTime;
    public int m_nSkillCount;
    public float m_fMaxSkillCoolTime;

    protected int m_nEndingCount;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

    private float m_fHorizontal;
    private float m_fVertical;
#endif

    private float m_fInvincibleTime;
    public float m_fShotTime;
    public float m_fMaxShotTime;
    public float m_fAttackSpeed;
    public float m_fDieTime;
    public int m_nCritical;
    public int m_nLevel;
    public int m_nHp;
    public int m_nMaxHp;
    public int m_nMp = 500;
    public int m_nDamage = 50;
    public int m_nExp;
    public int m_nMaxExp;

    public bool m_isBlocked = false;
    public bool m_isInvincible = false;

    public bool m_isWallReflect;
    public bool m_isPass;
    public bool m_isDiagonal;
    public bool m_isRightAngle;
    public bool m_isBack;
    public bool m_isFireShot;

    public bool m_isDie;

    public GameObject m_Light;

    public ePlayerState m_ePlayerState = ePlayerState.IDLE;
    public bool m_isKeyDown;

    public virtual void PlayerSkill()
    {

    }


    protected virtual void Awake()
    {
        m_nLevel = 0;

        m_nEndingCount = 0;
        m_nHp = 500;
        m_nMaxHp = 500;

        m_nExp = 250;
        m_nMaxExp = 250;

        m_fShotTime = 0.0f;
        m_fMaxShotTime = 1.0f;

        m_nCritical = 20;

        m_isWallReflect = false;
        m_isPass = false;
        m_isDiagonal = false;
        m_isRightAngle = false;
        m_isBack = false;
        m_isFireShot = false;

        m_isKeyDown = false;

        m_fDieTime = 0.0f;
        m_isDie = false;
        m_fAttackSpeed = 1.5f;

        m_Light = this.transform.Find("Light").gameObject;
        m_animator = this.GetComponent<Animator>();
        m_rigidbody = this.GetComponent<Rigidbody>();
        m_playerSkill = new cPlayerFireBall();
        m_animator.SetFloat("m_fAttackSpeed", m_fAttackSpeed);
    }

    private void Start()
    {
       
    }

    protected virtual void FixedUpdate()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

        if (m_isKeyDown && m_ePlayerState == ePlayerState.RUN && !m_isDie)
        {
            m_rigidbody.MovePosition(transform.position + transform.forward * m_fSpeed * Time.fixedDeltaTime);
            turn();
        }
#endif

        switch (m_ePlayerState)
        {
            case ePlayerState.IDLE:
                m_fSpeed = 0.0f;
                if (MonsterFind())
                {
                    m_ePlayerState = ePlayerState.ATTACK;
                }
                else
                {
                    m_fShotTime = 0.0f;
                }
                break;
            case ePlayerState.WALK:
                break;
            case ePlayerState.RUN:
                if (m_isKeyDown)
                {
                    m_fSpeed = m_fRunSpeed;
                }
                break;
            case ePlayerState.ATTACK:
                if (MonsterFind())
                {
                    NearMonsterFind();
                    m_fShotTime += Time.deltaTime;
                    if (m_fShotTime >= m_fMaxShotTime)
                    {
                        m_fShotTime = 0.0f;
                        cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerAttack);
                        m_animator.SetTrigger("m_tAttack");
                    }
                }
                else
                {
                    m_fShotTime = 0.0f;
                    m_ePlayerState = ePlayerState.IDLE;
                }
                break;
        }



        if (m_ePlayerState != ePlayerState.DIE)
        {
            if (m_nHp <= 0)
            {
                m_nHp = 0;
                if (!m_isDie)
                {
                    m_animator.SetTrigger("Die");
                    cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerDie);
                    m_isDie = true;
                    m_ePlayerState = ePlayerState.DIE;
                }
            }
        }
            if (m_isDie)
            {
                m_fDieTime += Time.deltaTime;
                if (m_fDieTime > 1.0f)
                {
                    m_isDie = false;
                    m_fDieTime = 0.0f;
                    cUIManager.GetInstance.CreatePlayerDiePopup();
                }


            }
        
        if (m_isInvincible)
        {

            m_fInvincibleTime += Time.deltaTime;
            float colorValue = m_fInvincibleTime;

            if (colorValue > 1.0f)
            {
                colorValue -= 1.0f;
            }

            Light playerLight = m_Light.GetComponent<Light>();

            playerLight.color = new Color(1.0f - colorValue, 0.0f, 0.0f, 1.0f);
            if (m_fInvincibleTime >= 2.0f)
            {
                m_fInvincibleTime = 0.0f;

                playerLight.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                m_isInvincible = false;
            }

        }


        if (cMapManager.GetInstance.m_eMapState == cMapManager.eMapState.NORMAL)
        {
            if (m_nSkillCount < 2)
            {
                m_fSkillCoolTime += Time.deltaTime;

                if (m_fSkillCoolTime > m_fMaxSkillCoolTime)
                {
                    m_fSkillCoolTime = 0.0f;
                    m_nSkillCount++;
                }

            }
        }

        m_animator.SetFloat("m_fSpeed", m_fSpeed);


        if ((this.transform.position.x > 0.0f &&
    this.transform.position.x < cMapManager.GetInstance.m_MaxWidth - 0.5f) &&
     (this.transform.position.z > 0.5f &&
    this.transform.position.z < cMapManager.GetInstance.m_MaxHeight - 1.5f))
        {
            m_isBlocked = false;
        }
        else
        {
            m_isBlocked = true;
            MapOutPosition();
        }
    }

    protected virtual void Update()
    {
// 유니티 에디터나 PC환경 일 시
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER

        m_fHorizontal = Input.GetAxisRaw("Horizontal");
        m_fVertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.UpArrow) ||
            Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            m_ePlayerState = ePlayerState.RUN;
            lookDirection = m_fVertical * Vector3.forward + m_fHorizontal * Vector3.right;
            m_isKeyDown = true;
        }

        if (Input.GetKeyUp(KeyCode.UpArrow) ||
            Input.GetKeyUp(KeyCode.DownArrow) ||
            Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow))
        {
            m_isKeyDown = false;
            m_ePlayerState = ePlayerState.IDLE;

        }
        m_vecDir.Set(m_fHorizontal, 0, m_fVertical);
#endif

    }

    protected virtual void OnCollisionStay(Collision collision)
    {

        if (cMapManager.GetInstance.m_eMapState == cMapManager.eMapState.CLEAR &&
          collision.gameObject.layer == LayerMask.NameToLayer("MapCastle"))
        {
            if (cMapManager.GetInstance.m_nMapNum > 29)
            {
                if(m_nEndingCount < 1)
                {
                    cUIManager.GetInstance.CreateEndingPopup();
                    m_nEndingCount++;
                }
            }
            else
            {
                cMapManager.GetInstance.NextStageSetting();
                cActorManager.GetInstance.ActorPositionSetting();
            }
           
        }

        if (!m_isInvincible && collision.gameObject.layer == LayerMask.NameToLayer("MapTrap"))
        {
            cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerGetHit);
            GetTrapHit(collision);
            m_isInvincible = true;
        }

        if (!m_isInvincible && collision.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerGetHit);
            GetMonsterHit(collision);
            m_isInvincible = true;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("MonsterBullet"))
        {
            
            collision.gameObject.GetComponent<cBaseSkill>().CollisionMonster(collision);

            if (!m_isInvincible)
            {
                cSoundManager.GetInstance.PlayPlayerSound(cSoundManager.GetInstance.m_PlayerGetHit);
                GetMonsterBulletHit(collision);
            }


            m_isInvincible = true;
        }
    }

    public bool MonsterFind()
    {
        List<cMonster> monsterList = cActorManager.GetInstance.m_cMonsterList;

        if(monsterList != null)
        {
            for (int i = 0; i < monsterList.Count; ++i)
            {
                if (monsterList[i].gameObject.activeInHierarchy &&
                    monsterList[i].m_eState != cMonster.eMonsterState.DIE)
                {
                    return true;
                }
            }
        }
       
        return false;
    }


    public void NearMonsterFind()
    {
        List<cMonster> monsterList = cActorManager.GetInstance.m_cMonsterList;

        float fMinDistance = 50000.0f;

        Vector3 targetPos = new Vector3(0,0,0);

        cMonster target = null;

        for (int i = 0; i < monsterList.Count; ++i)
        {
            if(monsterList[i].gameObject.activeInHierarchy && monsterList[i].m_eState != cMonster.eMonsterState.DIE)
            {
                float distance = (this.transform.position - monsterList[i].transform.position).sqrMagnitude;

                if (fMinDistance > distance)
                {
                    fMinDistance = distance;
                    target = monsterList[i];
                    targetPos = monsterList[i].transform.position;
                }
                else
                {
                    monsterList[i].m_TargetZone.SetActive(false);
                }
            }
            
        }

        target.m_TargetZone.SetActive(true);

        targetPos.y = 0.5f;

        m_vTargetPosition = targetPos;
        transform.LookAt(m_vTargetPosition);

    }

    private void MapOutPosition()
    {
        List<cBlock> blockList = cMapManager.GetInstance.m_blockList;

        float minDistance = 99999999.9f;
        int blockNum = 0;
        for(int i=0; i< blockList.Count; ++i)
        {
            if(blockList[i].m_eBlockType == cBlock.eBlockType.GROUND)
            {
                Vector3 vec = blockList[i].transform.position - this.transform.position;

                if(minDistance > vec.magnitude)
                {
                    minDistance = vec.magnitude;
                    blockNum = i;
                }

            }
        }

        Vector3 pos = blockList[blockNum].transform.position;
        pos.y = 0.5f;

        this.transform.position = pos;

    }


    public void GetMonsterHit(Collision collision)
    {
#if UNITY_ANDROID || UNITY_IOS

        Handheld.Vibrate();
#endif
        m_nHp -= collision.gameObject.GetComponent<cMonster>().m_nDamage;
    }

    public void GetMonsterHit(Collider other)
    {
#if UNITY_ANDROID || UNITY_IOS

        Handheld.Vibrate();
#endif
        m_nHp -= other.gameObject.GetComponent<cMonster>().m_nDamage;
    }

    public void GetMonsterBulletHit(Collision collision)
    {
#if UNITY_ANDROID || UNITY_IOS

        Handheld.Vibrate();
#endif
        m_nHp -= collision.gameObject.GetComponent<cBaseSkill>().m_nDamage;
    }

    public void GetTrapHit(Collision collision)
    {
#if UNITY_ANDROID || UNITY_IOS

        Handheld.Vibrate();
#endif
        m_nHp -= collision.gameObject.GetComponent<cBlock>().m_nDamage;
    }

    private void Shooting()
    {
        Vector3 dir = (m_vTargetPosition - transform.position).normalized;
        Vector3 dirLeft = dir;
        Vector3 dirRight = dir;
        Vector3 dirBack = dir;

        m_playerSkill.Shooting(dir);

        if (m_isDiagonal)
        {
            dirLeft = Quaternion.Euler(0, -45, 0) * dirLeft;
            dirRight = Quaternion.Euler(0, 45, 0) * dirRight;

            m_playerSkill.Shooting(dirLeft);
            m_playerSkill.Shooting(dirRight);
        }

        if(m_isRightAngle)
        {
            dirLeft = dir;
            dirRight = dir;
            dirLeft = Quaternion.Euler(0, -90, 0) * dirLeft;
            dirRight = Quaternion.Euler(0, 90, 0) * dirRight;
            m_playerSkill.Shooting(dirLeft);
            m_playerSkill.Shooting(dirRight);
        }

        if (m_isBack)
        {
            dirBack = Quaternion.Euler(0, 180, 0) * dirBack;
            m_playerSkill.Shooting(dirBack);
        }
        
    }

    private void turn()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER


        if (m_fHorizontal == 0 && m_fVertical == 0)
        {
            return;
        }

        Quaternion newRotation = Quaternion.LookRotation(m_vecDir);
        m_rigidbody.rotation = Quaternion.Slerp(m_rigidbody.rotation, newRotation, Time.deltaTime * 20.0f);
#endif
    }

}
