using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GameBehaviour
{
    private float maxHealth = 10;
    [SerializeField]
    private float currentHealth;

    [SerializeField]
    private GameObject gunObject;
    private GunBase gun;
    [SerializeField]
    private float fireRange;
    [SerializeField]
    private float detectionRange;

    private GameObject player;

    public bool playerDetected;
    public bool playerInRange;

    private void Awake()
    {
        gun = gunObject.GetComponent<GunBase>();
        player = PM.gameObject;
    }
    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {

        Vector3 distanceToPlayer = (gun.targetPoint - player.transform.position);
        float sqrLen = distanceToPlayer.sqrMagnitude;


        if(sqrLen < detectionRange * detectionRange)
        {
            playerDetected = true;
            transform.LookAt(player.transform.position);
        }
        if (sqrLen < fireRange * fireRange && gun.readyToFire)
        {
            playerInRange = true;
            gun.targetPoint = player.transform.position;
        }
    }
    public void Damage(float damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SM.enemiesAlive.Remove(gameObject);
        Destroy(gameObject);
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, fireRange);
    //}


    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(transform.position, player.transform.position);
    //}
}
