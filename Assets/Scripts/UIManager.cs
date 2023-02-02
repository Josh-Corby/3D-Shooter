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

    private void Start()
    {
        InitializeUI();
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

    private void InitializeUI()
    {
        UpdatePlayerCurrentHealth(PM.currentHealth);
        UpdatePlayerMaxHealth(PM.maxHealth);

        playerHealthSlider.maxValue = PM.maxHealth;
        playerHealthSlider.value = PM.currentHealth;

        isReloading = false;
        reloadSliderObject.SetActive(false);
        reloadSlider.value = reloadSlider.maxValue;
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

    public void ChangeGunsText(GunBase gun)
    {
        gunNameText.text = gun.gameObject.name;
        UpdateGunAmmoText(gun);
    }

    public void UpdateGunAmmoText(GunBase gun)
    {
        gunBulletsRemainingText.text = gun.bulletsRemainingInClip.ToString();
        gunAmmoLeftText.text = "/" + gun.ammoLeft.ToString();
        gunMaxAmmoText.text = gun.maxAmmo.ToString();
        reloadSlider.maxValue = gun.reloadTime;
        reloadSlider.value = reloadSlider.maxValue;
    }
    
    public void StartReloading()
    {
        reloadSliderObject.SetActive(true);
        isReloading = true;
    }
}
