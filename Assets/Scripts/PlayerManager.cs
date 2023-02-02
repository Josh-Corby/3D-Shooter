using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : GameBehaviour<PlayerManager>, IDamagable
{
    public float maxHealth;
    public float currentHealth;
    private bool canTakeDamage;
    private CapsuleCollider col;
    [SerializeField]
    private float iFramesTime;
    new private MeshRenderer renderer;
    private Color baseColor;

    public Grid lastGrid;

    [SerializeField] private GameObject currentWeaponObject;
    [HideInInspector] public GunBase currentWeapon;
    public GameObject[] playerWeapons;
    private int currentWeaponIndex;

    new private void Awake()
    {
        col = GetComponent<CapsuleCollider>();
        renderer = GetComponentInParent<MeshRenderer>();
        baseColor = renderer.material.color;
    }

    private void OnEnable()
    {
        InputManager.Scroll += ChangeWeapons;
    }
    private void OnDisable()
    {
        InputManager.Scroll -= ChangeWeapons;
    }
    private void Start()
    {
        InitializeWeapons();

        currentHealth = maxHealth;
        canTakeDamage = true;
    }

    private void InitializeWeapons()
    {
        currentWeaponIndex = 0;
        for (int i = 0; i < playerWeapons.Length; i++)
        {
            playerWeapons[i].SetActive(false);
        }
        currentWeaponObject = playerWeapons[0];
        currentWeapon = currentWeaponObject.GetComponent<GunBase>();
        currentWeaponObject.SetActive(true);
        UI.ChangeGunsText(currentWeaponObject.GetComponent<GunBase>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grid"))
        {
            lastGrid = other.GetComponent<Grid>();
        }
    }

    public void Damage(float damage)
    {
        if (canTakeDamage)
        {
            currentHealth -= damage;
            StartCoroutine(IFrames());

            UI.UpdatePlayerCurrentHealth(currentHealth);
        }       
    }

    private IEnumerator IFrames()
    {
        canTakeDamage = false;
        renderer.material.color = Color.cyan;
        yield return new WaitForSeconds(iFramesTime);
        canTakeDamage = true;
        renderer.material.color = baseColor;
    }

    private void ChangeWeapons(float _input)
    {
        if (_input > 0)
        {
            currentWeaponIndex -= 1;

            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = playerWeapons.Length - 1;
            }
        }

        if (_input < 0)
        {
            currentWeaponIndex += 1;

            if (currentWeaponIndex > playerWeapons.Length - 1)
            {
                currentWeaponIndex = 0;
            }
        }

        currentWeaponObject.SetActive(false);
        currentWeaponObject = playerWeapons[currentWeaponIndex];
        currentWeaponObject.SetActive(true);

        UI.ChangeGunsText(currentWeaponObject.GetComponent<GunBase>());
    }
}
