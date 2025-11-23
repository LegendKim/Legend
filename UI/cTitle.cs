using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cTitle : MonoBehaviour
{
    public GameObject m_SelectPopupPrefab;
    Stack<GameObject> m_StackSelectPopup;

    private void Awake()
    {
        m_StackSelectPopup = new Stack<GameObject>();
    }

    public void ChangeGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void CreateSelectPopup()
    {
        AudioListener.volume = 0.5f;
        cSoundManager.GetInstance.PlayUISound(cSoundManager.GetInstance.m_TitleClick);
        GameObject obj = m_SelectPopupPrefab;
        obj = Instantiate(obj, this.transform);
        obj.transform.localPosition = Vector3.zero;

        m_StackSelectPopup.Push(obj);
    }


    public void DestroySelectPopup()
    {
        AudioListener.volume = 1.0f;
        if (m_StackSelectPopup.Count > 0)
        {
            Destroy(m_StackSelectPopup.Pop());
        }
    }
}
