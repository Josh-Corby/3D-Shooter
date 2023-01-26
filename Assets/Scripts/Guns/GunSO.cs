using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun")]
public class GunSO : ScriptableObject
{
    public GameObject bulletToFire;

    [Header("Firing Options")]
    public float shootForce;
    public float timeBetweenShots;
    public bool holdToFire;
    [Header("Spread Options")]
    public bool useSpread;
    public float spread;

    [Header("Shotgun Options")]
    public bool shotgunFire;
    public int shotsInShotgunFire;

    [Header("BurstFire Options")]
    public bool burstFire;
    public int bulletsInBurst;
    public float timeBetweenBurstShots;
}
