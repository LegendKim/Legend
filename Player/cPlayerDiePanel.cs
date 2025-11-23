using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class cPlayerDiePanel : MonoBehaviour
{
    public cPlayer m_Player;

    private void Awake()
    {
        m_Player = GameObject.FindWithTag("Player").GetComponent<cPlayer>();
    }

    public void AdsPopUp()
    {
#if UNITY_ANDROID || UNITY_IOS

        ShowRewardedAd();

#elif  UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER
        AdsReward();
#endif


        // 광고 이후
    }

    public void AdsReward()
    {
        m_Player.m_nHp = m_Player.m_nMaxHp / 2;
        m_Player.m_isDie = false;
        m_Player.m_ePlayerState = ePlayerState.IDLE;
        DestroyPlayerDiePanel();
        cSoundManager.GetInstance.SetBGMVolume(1.0f);
    }


    public void ReturnTitle()
    {
        GameObject characterInfo = GameObject.Find("CharacterSelectInfo");
        Destroy(characterInfo);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("TitleScene");
    }

    public void DestroyPlayerDiePanel()
    {
        cUIManager.GetInstance.DestroyPlayerDiePopup();
    }

#if UNITY_ANDROID || UNITY_IOS

    void ShowAds()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = ResultAds };

            Advertisement.Show("rewardedVideo", options);
        }
    }
    
    private void ResultAds(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("The ad was successfully shown.");

                    AdsReward();

                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");
                    AdsReward();
                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");
                    AdsReward();
                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }
#endif
}
