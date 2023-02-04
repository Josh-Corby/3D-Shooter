using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : GameBehaviour<UIManager>
{
    public static event Action OnReloadAnimationDone;

    #region Panels
    [Header("Panels")]
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject crosshairPanel;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject playerHealthPanel;
    [SerializeField] private GameObject gunInfoPanel;
    #endregion

    #region PlayerInfoUI
    [Header("Player Info UI")]  
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TMP_Text playerCurrentHealthText;
    [SerializeField] private TMP_Text playerMaxHealthText;
    #endregion

    #region GunInfoUI
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
        GunBase.OnBulletFired += UpdateBulletsRemainingUI;
        GunBase.OnReloadStart += StartReloading;
        GunBase.OnAmmoAdded += UpdateAmmoLeftUI;

        PlayerManager.OnWeaponChange += ChangeGunUI;
        PlayerManager.OnCurrentHealthChange += UpdatePlayerCurrentHealth;
        PlayerManager.OnMaxHealthChange += UpdatePlayerMaxHealth;
    }
    private void OnDisable()
    {
        GunBase.OnReloadDone -= ReloadUI;
        GunBase.OnBulletFired -= UpdateBulletsRemainingUI;
        GunBase.OnReloadStart -= StartReloading;
        GunBase.OnAmmoAdded -= UpdateAmmoLeftUI;

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
        UpdateAmmoLeftUI(gun.ammoLeft);
        UpdateBulletsRemainingUI(gun.bulletsRemainingInClip);
        UpdateWeaponMaxAmmoText(gun.maxAmmo);
        gunNameText.text = gun.gameObject.name;
        reloadSlider.maxValue = gun.reloadTime;
        reloadSlider.value = reloadSlider.maxValue;
    }

    private void ReloadUI(GunBase gun)
    {
        UpdateAmmoLeftUI(gun.ammoLeft);
        UpdateBulletsRemainingUI(gun.bulletsRemainingInClip);
    }

    public void UpdateBulletsRemainingUI(int value)
    {
        gunBulletsRemainingText.text = value.ToString();
    }

  
    private void UpdateAmmoLeftUI(int value)
    {
        gunAmmoLeftText.text = "/" + value.ToString();
    }

    private void UpdateWeaponMaxAmmoText(int value)
    {
        gunMaxAmmoText.text = value.ToString();
    }

    public void StartReloading()
    {
        reloadSlider.value = reloadSlider.maxValue;
        reloadSliderObject.SetActive(true);
        isReloading = true;
    }
}
