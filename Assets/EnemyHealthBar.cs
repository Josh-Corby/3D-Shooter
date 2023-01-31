using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : GameBehaviour
{
    private Slider healthBar;
    private float targetHP;
    private EnemyBase unit;

    private void Awake()
    {
        healthBar = GetComponent<Slider>();
        unit = GetComponentInParent<EnemyBase>();
    }

    private void Start()
    {
        SetHUD();
    }

    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
    public void SetHUD()
    {
        healthBar.maxValue = unit.maxHealth;
        healthBar.value = healthBar.maxValue;
    }

    public void SetHP(float hp)
    {
        targetHP = hp;
        healthBar.value = targetHP;
    }
}
