using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour
{
    public Image planetHpFill;
    public Text goldText, coalText;

    public void SetPlanetHp(float percent)
    {
        planetHpFill.fillAmount = percent;
    }

    public void SetGold(int amount)
    {
        goldText.text = string.Format("골드 : {0}", amount);
    }

    public void SetCoal(int amount)
    {
        coalText.text = string.Format("석탄 : {0}", amount);
    }
}
