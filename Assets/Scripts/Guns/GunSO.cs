using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun",menuName = "New Gun")]
public class GunSO : ScriptableObject
{
    public GameObject bulletToFire;
    public float damage;

    public int maxAmmo;
    public int clipSize;
    public float reloadTime;

    [Header("Firing Options")]
    public float shootForce;
    public float timeBetweenShots;
    public bool holdToFire;
    [Header("Spread Options")]
    public bool useSpread;
    public float spreadAmount;

    [Header("Shotgun Options")]
    public bool shotgunFire;
    public int shotsInShotgunFire;

    [Header("BurstFire Options")]
    public bool burstFire;
    public int bulletsInBurst;
    public float timeBetweenBurstShots;
}
