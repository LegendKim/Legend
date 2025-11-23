using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class cReturnTitlePanel : MonoBehaviour
{
    public void ReturnTitle()
    {
        cSoundManager.GetInstance.PlayButtonSound();
        GameObject characterInfo = GameObject.Find("CharacterSelectInfo");
        Destroy(characterInfo);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("TitleScene");
    }

    public void DestroyReturnTitlePanel()
    {
        cSoundManager.GetInstance.PlayButtonSound();
        cUIManager.GetInstance.DestroyReturnTitlePopup();
    }
}
