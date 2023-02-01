using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : GameBehaviour<UIManager>
{
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject crosshairPanel;
    [SerializeField] private GameObject gunInfoPanel;

    [SerializeField] private TMP_Text gunNameText;
    [SerializeField] private TMP_Text gunBulletsRemainingText;
    [SerializeField] private TMP_Text gunClipSizeText;
    [SerializeField] private TMP_Text gunAmmoLeftText;

    public void ChangeGunsText(GunBase gun)
    {
        gunNameText.text = gun.gameObject.name;
        UpdateGunAmmoText(gun);
    }

    public void UpdateGunAmmoText(GunBase gun)
    {
        gunBulletsRemainingText.text = gun.bulletsRemainingInClip.ToString();
        gunClipSizeText.text = gun.clipSize.ToString();
        gunAmmoLeftText.text = gun.ammoLeft.ToString();
    }
}
