using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    [Header("AsteroidInfo")]
    public GameObject grpAsteroidInfoTexts;
    public Text asteroidNameText;
    public Text asteroidHpText;
    public Text asteroidRadiusText;
    public Text asteroidSpeedText;
    public Text asteroidRemainTimeToCollisionText;

    [Header("TowerInfo")]
    public GameObject grpTowerInfoTexts;
    public Text towerNameText;
    public Text towerLevelText;
    public Text towerHpText;
    public Text towerDamageText;
    public Text towerCooldownText;

    public GameObject buttonContent;
    public Button upgradeButton;
    public Button sellButton;
    public Button liftOffButton;
    public Button landButton;

    [Space(10)]
    public Image portraitImage;
    public UITween uiTween;

    private bool isShowAsteroidInfo;

    public void ShowAsteroidInfo(Asteroid asteroid)
    {
        if (!isShowAsteroidInfo)
        {
            grpTowerInfoTexts.SetActive(false);
            buttonContent.SetActive(false);
        }

        asteroidNameText.text = "";
        asteroidHpText.text = "";
        asteroidRadiusText.text = "";
        asteroidSpeedText.text = "";
        asteroidRemainTimeToCollisionText.text = "";
        portraitImage.sprite = null;
        grpAsteroidInfoTexts.SetActive(true);

        uiTween.Show(true);

        isShowAsteroidInfo = true;
    }

    public void ShowTowerInfo(Tower tower)
    {
        if (isShowAsteroidInfo)
        {
            grpAsteroidInfoTexts.SetActive(false);
            buttonContent.SetActive(true);
        }

        if (tower.CurState == Tower.State.Landed)
        {
            ButtonsWhenTowerLanded();
        }
        else if (tower.CurState == Tower.State.Floating)
        {
            ButtonsWhenTowerLiftedOff();
        }

        towerNameText.text = "";
        towerLevelText.text = "";
        towerHpText.text = "";
        towerDamageText.text = "";
        towerCooldownText.text = "";
        portraitImage.sprite = null;
        grpTowerInfoTexts.SetActive(true);

        uiTween.Show(true);

        isShowAsteroidInfo = false;
    }

    public void Hide()
    {
        uiTween.Show(false);
    }

    public void ButtonsWhenTowerLiftedOff()
    {
        upgradeButton.interactable = false;
        sellButton.interactable = false;

        liftOffButton.gameObject.SetActive(false);
        landButton.gameObject.SetActive(true);
    }

    public void ButtonsWhenTowerLanded()
    {
        upgradeButton.interactable = true;
        sellButton.interactable = true;

        liftOffButton.gameObject.SetActive(true);
        landButton.gameObject.SetActive(false);
    }
}
