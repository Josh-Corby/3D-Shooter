using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : GameBehaviour<UIManager>
{
    public static event Action OnReloadAnimationDone;

    #region panels
    [Header("Panels")]
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject crosshairPanel;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject playerHealthPanel;
    [SerializeField] private GameObject gunInfoPanel;
    #endregion

    #region playerInfoUI
    [Header("Player Info UI")]  
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text playerCurrentHealthText;
    [SerializeField] private TMP_Text playerMaxHealthText;
    #endregion

    #region gunInfoUI
    [Header("Gun Info UI")]  
    [SerializeField] private TMP_Text gunNameText;
    [SerializeField] private TMP_Text gunBulletsRemainingText;
    [SerializeField] private TMP_Text gunAmmoLeftText;
    [SerializeField] private TMP_Text gunMaxAmmoText;
    [SerializeField] private GameObject reloadSliderObject;
    private Slider reloadSlider;
    private bool isReloading;
    #endregion


    new private void Awake()
    {
        reloadSlider = reloadSliderObject.GetComponent<Slider>();
    }

    private void OnEnable()
    {
        GunBase.OnReloadDone += ReloadUI;
        GunBase.OnBulletFired += UpdateGunAmmoText;
        GunBase.OnReloadStart += StartReloading;

        PlayerManager.OnWeaponChange += ChangeGunUI;
        PlayerManager.OnCurrentHealthChange += UpdatePlayerCurrentHealth;
        PlayerManager.OnMaxHealthChange += UpdatePlayerMaxHealth;
    }
    private void OnDisable()
    {
        GunBase.OnReloadDone -= ReloadUI;
        GunBase.OnBulletFired -= UpdateGunAmmoText;
        GunBase.OnReloadStart -= StartReloading;

        PlayerManager.OnWeaponChange -= ChangeGunUI;
        PlayerManager.OnCurrentHealthChange -= UpdatePlayerCurrentHealth;
        PlayerManager.OnMaxHealthChange -= UpdatePlayerMaxHealth;
    }
    private void Start()
    {
        InitializeUI();
    }
    private void InitializeUI()
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

    public void UpdatePlayerCurrentHealth(float value)
    {
        playerCurrentHealthText.text = value.ToString();
        playerHealthSlider.value = value;
    }

    public void UpdatePlayerMaxHealth(float value)
    {
        playerMaxHealthText.text = value.ToString();
        playerHealthSlider.maxValue = value;
    }


    private void ChangeGunUI(GunBase gun)
    {
        UpdateAmmoUI(gun);
        gunBulletsRemainingText.text = gun.bulletsRemainingInClip.ToString();
        gunNameText.text = gun.gameObject.name;
        gunMaxAmmoText.text = gun.maxAmmo.ToString();
        reloadSlider.maxValue = gun.reloadTime;
        reloadSlider.value = reloadSlider.maxValue;
    }

    private void ReloadUI(GunBase gun)
    {
        UpdateAmmoUI(gun);
        UpdateGunAmmoText(gun.bulletsRemainingInClip);
    }

    public void UpdateGunAmmoText(int value)
    {
        gunBulletsRemainingText.text = value.ToString();
    }

  
    private void UpdateAmmoUI(GunBase gun)
    {
        gunAmmoLeftText.text = "/" + gun.ammoLeft.ToString();
    }


    private void UpdateWeaponMaxAmmo(GunBase gun)
    {
        gunMaxAmmoText.text = gun.maxAmmo.ToString();
    }

    public void StartReloading()
    {
        reloadSlider.value = reloadSlider.maxValue;
        reloadSliderObject.SetActive(true);
        isReloading = true;
    }
}
