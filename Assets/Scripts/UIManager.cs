using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : GameBehaviour<UIManager>
{
    public static event Action OnReloadAnimationDone;

    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject crosshairPanel;
    [SerializeField] private GameObject gunInfoPanel;

    [SerializeField] private TMP_Text gunNameText;
    [SerializeField] private TMP_Text gunBulletsRemainingText;
    [SerializeField] private TMP_Text gunAmmoLeftText;
    [SerializeField] private TMP_Text gunMaxAmmo;

    [SerializeField] private GameObject reloadSliderObject;
    private Slider reloadSlider;
    public bool isReloading;

    private void Awake()
    {
        reloadSlider = reloadSliderObject.GetComponent<Slider>();
    }

    private void Start()
    {
        isReloading = false;
        reloadSliderObject.SetActive(false);
        reloadSlider.value = reloadSlider.maxValue;
    }

    private void Update()
    {
        if (isReloading)
        {
            reloadSlider.value -= Time.deltaTime;
        }

        if(reloadSlider.value <= reloadSlider.minValue && isReloading)
        {
            isReloading = false;
            reloadSliderObject.SetActive(false);
            OnReloadAnimationDone?.Invoke();
        }
    }

    public void ChangeGunsText(GunBase gun)
    {
        gunNameText.text = gun.gameObject.name;
        UpdateGunAmmoText(gun);
    }

    public void UpdateGunAmmoText(GunBase gun)
    {
        gunBulletsRemainingText.text = gun.bulletsRemainingInClip.ToString();
        gunAmmoLeftText.text = "/" + gun.ammoLeft.ToString();
        gunMaxAmmo.text = gun.maxAmmo.ToString();
        reloadSlider.maxValue = gun.reloadTime;
        reloadSlider.value = reloadSlider.maxValue;
    }
    
    public void StartReloading()
    {
        reloadSliderObject.SetActive(true);
        isReloading = true;
    }
}
