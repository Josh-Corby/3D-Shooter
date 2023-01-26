using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "New Bullet")]
public class BulletSO : ScriptableObject
{
    public float damage;
    public bool hasRigidBody;

    [Header("Split Options")]
    public bool splittingProjectile;
    public GameObject splitBullet;
    public float splitForce;
    public float splitSpread;

    [Header("Homing Options")]
    public bool homingProjectile;
    public float homingSpeed;
    public float maxSpeed;
    public float findTargetWaitTime;
}
