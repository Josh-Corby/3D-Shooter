using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : GameBehaviour<PlayerManager>, IDamagable
{
    public float maxHealth;
    public float currentHealth;

    private CapsuleCollider col;
    [SerializeField]
    private float iFramesTime;
    new private MeshRenderer renderer;
    private Color baseColor;

    public Grid lastGrid;

    private GameObject currentWeapon;
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
    }

    private void InitializeWeapons()
    {
        currentWeaponIndex = 0;
        for (int i = 0; i < playerWeapons.Length; i++)
        {
            playerWeapons[i].SetActive(false);
        }
        currentWeapon = playerWeapons[0];
        currentWeapon.SetActive(true);
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
        currentHealth -= damage;
        StartCoroutine(IFrames());
    }

    private IEnumerator IFrames()
    {
        col.enabled = false;
        renderer.material.color = Color.cyan;
        yield return new WaitForSeconds(iFramesTime);
        col.enabled = true;
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

            //Debug.Log("scroll up");
        }

        if (_input < 0)
        {
            currentWeaponIndex += 1;

            if (currentWeaponIndex > playerWeapons.Length - 1)
            {
                currentWeaponIndex = 0;
            }

            //Debug.Log("scroll down");
        }

        currentWeapon.SetActive(false);
        currentWeapon = playerWeapons[currentWeaponIndex];
        currentWeapon.SetActive(true);
    }
}
