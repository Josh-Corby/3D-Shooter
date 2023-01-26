using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : GameBehaviour
{
    public BulletSO bulletInfo;

    private Rigidbody rb;
    private float damage;
    [HideInInspector]
    public bool hasRigidbody;
    private bool isSpawnedProjectile;

    [Header("Split Options")]
    private bool splittingProjectile;
    private GameObject splitBullet;
    private float splitForce = 10;
    private float splitSpread = 5;
    private Vector3[] CardinalDirections = new Vector3[]
   {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down,
   };
    [Header("Homing Options")]
    private bool homingProjectile;
    private float homingSpeed = 10;
    private float maxSpeed = 50;
    private float findTargetWaitTime = 1;
    private GameObject homingtarget;
    private bool searchForTarget;

    private void Awake()
    {
        AssignValues();
    }
    public virtual void Start()
    {
        if (isSpawnedProjectile)
        {
            StartCoroutine(ColliderActivateTimer());
        }
    }

    private void Update()
    {
        if (homingProjectile)
        {
            HomingCountDown();
        }
    }
    private void AssignValues()
    {
        damage = bulletInfo.damage;
        hasRigidbody = bulletInfo.hasRigidBody;
        splittingProjectile = bulletInfo.splittingProjectile;
        splitBullet = bulletInfo.splitBullet;
        splitForce = bulletInfo.splitForce;
        splitSpread = bulletInfo.splitSpread;
        homingProjectile = bulletInfo.homingProjectile;
        homingSpeed = bulletInfo.homingSpeed;
        maxSpeed = bulletInfo.maxSpeed;
        findTargetWaitTime = bulletInfo.findTargetWaitTime;
        rb = GetComponent<Rigidbody>();


        if (homingProjectile)
        {
            findTargetWaitTime = Mathf.Clamp(findTargetWaitTime, 0, findTargetWaitTime);
        }
    }

    private void ClampSpeed()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (splittingProjectile)
            {
                Split();
            }
            DamageEnemy(collision.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    protected void DamageEnemy(GameObject enemy)
    {
        enemy.GetComponent<EnemyBase>().Damage(damage);
        Destroy(gameObject);
    }

    public IEnumerator ColliderActivateTimer()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        collider.enabled = true;
    }


    private void MoveTowardsTarget()
    {
        if (homingtarget == null)
        {
            GetClosestEnemy();
        }
        if (homingtarget != null)
        {

            // Rotate the missile towards the target
            transform.LookAt(homingtarget.transform);
            Vector3 directionToTarget = homingtarget.transform.position - transform.position;

            if (Vector3.Distance(transform.position, homingtarget.transform.position) <= 30f)
            {
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(directionToTarget * maxSpeed);
                return;
            }
            // Apply the force to the missile in the direction it is facing
            rb.AddForce(directionToTarget * homingSpeed);
        }
    }
    private void GetClosestEnemy()
    {
        homingtarget = null;
        float minDist = Mathf.Infinity;

        if (SM.enemiesAlive.Count > 0)
        {
            for (int i = 0; i < SM.enemiesAlive.Count; i++)
            {
                GameObject enemy = SM.enemiesAlive[i];
                float dist = Vector3.Distance(enemy.transform.position, transform.position);
                if (dist < minDist)
                {
                    homingtarget = enemy;
                    minDist = dist;
                }
            }
        }
    }
    private void HomingCountDown()
    {
        findTargetWaitTime -= Time.deltaTime;

        if (findTargetWaitTime <= 0)
        {
            searchForTarget = true;
        }
        if (searchForTarget)
        {
            MoveTowardsTarget();
        }
        ClampSpeed();
    }
    private void Split()
    {
        for (int i = 0; i < CardinalDirections.Length - 1; i++)
        {
            Debug.Log("Split");
            float x = Random.Range(-splitSpread, splitSpread);
            float y = Random.Range(-splitSpread, splitSpread);
            Vector3 spreadDirection = new Vector3(x, y, 0);
            Vector3 directionWithSpread = (CardinalDirections[i] + spreadDirection);
            GameObject spawnedBullet = Instantiate(splitBullet, transform.position, Quaternion.identity);
            spawnedBullet.GetComponent<BulletBase>().isSpawnedProjectile = true;
            spawnedBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread * splitForce, ForceMode.Impulse);
            spawnedBullet.GetComponent<BulletBase>().damage = (damage / 2);
        }
    }
}
