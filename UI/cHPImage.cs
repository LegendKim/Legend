using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cHPImage : MonoBehaviour
{
    public Image m_HpImage;
    public Image m_HpWhiteImage;

    void Update()
    {
        m_HpWhiteImage.fillAmount = Mathf.Lerp(m_HpWhiteImage.fillAmount, m_HpImage.fillAmount, Time.deltaTime * 2.0f);
    }

    public void SettingFillImage(float currentHp, float maxHp)
    {
        m_HpImage.fillAmount = currentHp / maxHp;
    }

}