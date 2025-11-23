using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cDamage : MonoBehaviour
{
    private float m_fTime;
    private Text m_text;

    private Vector3 m_InitPosition;

    public Animator m_animator;

    private bool m_isCritical;
    private bool m_isFireShot;

    private void Awake()
    {
        m_text = this.GetComponent<Text>();
        m_animator = this.GetComponent<Animator>();
    }

    void Start()
    {
        m_fTime = 0.0f;
        this.transform.position = m_InitPosition;
    }

    void Update()
    {
        if (m_isCritical)
        {
            m_text.color = new Color(1.0f, 0.0f, 0.0f, 1.0f - m_fTime);
        }
        else if(m_isFireShot)
        {
            m_text.color = new Color(1.0f, 0.5f, 0.5f, 1.0f - m_fTime);
        }
        else
        {
            m_text.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - m_fTime);
        }

        m_fTime += Time.deltaTime;


        Vector3 monPos = Camera.main.WorldToScreenPoint(m_InitPosition);
        monPos.z = 100.0f;

        Vector3 pos = cActorManager.GetInstance.m_HpCamera.ScreenToWorldPoint(monPos);
        pos.y += 8.0f;
        this.transform.position = pos;

        if (m_fTime > 1.0f)
        {
            m_fTime = 0.0f;
            m_text.text = "";
            m_text.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            m_isFireShot = false;
            m_isCritical = false;
            cObjectPoolManager.GetInstance.SetActiveFalse(this.gameObject);
        }
    }

    public void Init(Vector3 pos, int damage, bool isCritical, bool isFireShot)
    {
        m_isFireShot = isFireShot;
        m_isCritical = isCritical;

        m_text.text = "" + damage;

        if (isCritical)
        {
            m_text.text = "" + damage + "!";
        }
        if(isFireShot)
        {
            m_text.text = "-" + damage;
        }
        
        m_InitPosition = pos;
    }
}
