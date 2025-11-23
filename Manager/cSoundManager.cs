using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class cSoundManager : cSingleTon<cSoundManager>
{
    public AudioListener m_AudioListener;

    public AudioSource m_BGMAudio;

    public AudioSource m_UIAudio;
    public AudioSource m_PlayerAudio;

    public AudioClip m_ForestBGM;
    public AudioClip m_ForestBossBGM;
    public AudioClip m_DesertBGM;
    public AudioClip m_DesertBossBGM;
    public AudioClip m_DungeonBGM;
    public AudioClip m_DungeonBossBGM;

    public AudioClip m_ButtonClick;

    public AudioClip m_PlayerMove;
    public AudioClip m_PlayerAttack;
    public AudioClip m_PlayerGetHit;
    public AudioClip m_PlayerDie;

	public AudioClip m_CharacterClick;
	public AudioClip m_TitleClick;
	public AudioClip m_CharacterSelect;

	protected override void Awake()
    {
        base.Awake();

        cCharacterSelectInfo characterInfo = GameObject.Find("CharacterSelectInfo").GetComponent<cCharacterSelectInfo>();

        switch (characterInfo.m_eCharacterInfo)
        {
            case cCharacterSelectInfo.eCharacterInfo.ASSASSIN:
                m_PlayerAttack = Resources.Load<AudioClip>("MyGame/Sound/AssassinAttack");

                break;
            case cCharacterSelectInfo.eCharacterInfo.WARRIOR:
                m_PlayerAttack = Resources.Load<AudioClip>("MyGame/Sound/WarriorAttack");
                break;
            case cCharacterSelectInfo.eCharacterInfo.MAGICIAN:
                m_PlayerAttack = Resources.Load<AudioClip>("MyGame/Sound/MagicianAttack");
                break;

        }

    }

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if(null != player)
            m_PlayerAudio = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
    }

    public void PlayBGM(AudioClip clip)
    {
        m_BGMAudio.Stop();
        m_BGMAudio.clip = clip;

        m_BGMAudio.Play();
    }

    public void MuteOnBGM()
    {
        m_BGMAudio.mute = true;
    }

    public void StopBGM()
    {
        m_BGMAudio.Stop();
    }

    public void MuteOffBGM()
    {
        m_BGMAudio.mute = false;
    }

    public void SetBGMVolume(float value)
    {
        m_BGMAudio.volume = value;
    }

    public void SoundOn()
    {
        m_BGMAudio.Play();
        AudioListener.volume = 1f;
    }

    public void SoundOff()
    {
        m_BGMAudio.Stop();
        AudioListener.volume = 0f;
    }

    public void PlayButtonSound()
    {
        PlayUISound(m_ButtonClick);
    }
    
    public void PlayPlayerSound(AudioClip clip)
    {
        m_PlayerAudio.clip = clip;
        m_PlayerAudio.PlayOneShot(m_PlayerAudio.clip);   
    }
	public void PlayUISound(AudioClip clip)
	{
        m_UIAudio.clip = clip;
        m_UIAudio.PlayOneShot(m_UIAudio.clip);
	}

	public void PlayPlayerGetHitSound()
    {
        m_PlayerAudio.clip = m_PlayerGetHit;
        m_PlayerAudio.Play();
    }
}
