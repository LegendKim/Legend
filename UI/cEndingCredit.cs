using UnityEngine;
using UnityEngine.SceneManagement;

public class cEndingCredit : MonoBehaviour
{
    public void ReturnTitle()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("TitleScene");
    }
}