using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cStageNumAni : MonoBehaviour
{
    private Text m_StageNumText;

    private float m_fTime;

    private Animator m_animator;

    private AudioSource m_AudioSource;


    public AudioClip m_NormalAudioClip;
    public AudioClip m_BossAudioClip;

    void Start()
    {
        m_fTime = 0.0f;
        m_AudioSource = this.GetComponent<AudioSource>();
        m_animator = this.GetComponent<Animator>();
        m_StageNumText = GameObject.Find("StageNumText").GetComponent<Text>();

        if(cMapManager.GetInstance.m_nMapNum % 10 == 0 && cMapManager.GetInstance.m_nMapNum > 0)
        {
            m_AudioSource.clip = m_BossAudioClip;
        }
        else
        {
            m_AudioSource.clip = m_NormalAudioClip;
        }
    }

    void Update()
    {
        if(m_fTime < 0.00001f)
        {
            m_animator.SetTrigger("Blind");
        }

        string stage = "Stage ";

        stage += cMapManager.GetInstance.m_nMapNum;

        m_StageNumText.text = stage;

        m_fTime += Time.deltaTime;

        if (m_fTime > 1.2f)
        {
            m_fTime = 0.0f;
            this.gameObject.SetActive(false);
        }
        
    }
}
