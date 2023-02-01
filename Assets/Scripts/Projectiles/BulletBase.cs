using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : GameBehaviour
{
    public BulletSO bulletInfo;

    private Rigidbody rb;
    [HideInInspector]
    public float damage;
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
    private float maxHomingDistance;
    private float homingSpeed = 10;
    private float maxSpeed = 50;
    private float findTargetWaitTime = 1;
    private GameObject homingtarget;
    private bool searchForTarget;

    [Header("Explosive Options")]
    private bool explodingProjectile;
    private float explosionRadius;
    private float explosionDamage;

    private Vector3 lastPosition;

    [SerializeField] private LayerMask collisionMask;

    private void Awake()
    {
        AssignValues();
    }
    public virtual void Start()
    {
        lastPosition = transform.position;
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
            ClampSpeed();
        }
        RaycastToLastPosition();

        lastPosition = transform.position;
    }
    private void AssignValues()
    {
        hasRigidbody = bulletInfo.hasRigidBody;

        splittingProjectile = bulletInfo.splittingProjectile;
        splitBullet = bulletInfo.splitBullet;
        splitForce = bulletInfo.splitForce;
        splitSpread = bulletInfo.splitSpread;

        homingProjectile = bulletInfo.homingProjectile;
        maxHomingDistance = bulletInfo.maxHomingDistance;
        homingSpeed = bulletInfo.homingSpeed;
        maxSpeed = bulletInfo.maxSpeed;
        findTargetWaitTime = bulletInfo.findTargetWaitTime;

        explodingProjectile = bulletInfo.explodingProjectile;
        explosionRadius = bulletInfo.explosionRadius;
        explosionDamage = bulletInfo.explosionDamage;

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
    private void RaycastToLastPosition()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Ray(transform.position, (transform.position - lastPosition).normalized), (transform.position - lastPosition).magnitude, collisionMask);

        if (hits.Length > 1)
        {
            GameObject closestUnit = null;
            float closestHit = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].collider.gameObject.name);
                float distanceToLastPosition = (hits[i].transform.position - lastPosition).magnitude;
                if (distanceToLastPosition < closestHit)
                {
                    closestHit = distanceToLastPosition;
                    closestUnit = hits[i].collider.gameObject;
                }
            }

            ProcessCollision(closestUnit);
        }
        if (hits.Length == 1)
        {
            //Debug.Log(hits[0].collider.gameObject.name);
            ProcessCollision(hits[0].collider.gameObject);
        }

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

            if (Vector3.Distance(transform.position, homingtarget.transform.position) <= 15f)
            {
                rb.angularVelocity = Vector3.zero;
                rb.AddForce(directionToTarget * homingSpeed);
                return;
            }
            // Apply the force to the missile in the direction it is facing
            rb.AddForce(directionToTarget * homingSpeed);
        }
    }
    private void GetClosestEnemy()
    {
        homingtarget = null;
        float minDist = maxHomingDistance;

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

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, collisionMask);
        foreach (Collider collider in colliders)
        {
            Debug.Log(collider.gameObject.name);
            if (!collider.TryGetComponent<IDamagable>(out var interactable))
            {
                continue;
            }
            interactable.Damage(explosionDamage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other.gameObject);
    }

    private void ProcessCollision(GameObject collider)
    {
        if (!explodingProjectile)
        {
            if (!collider.TryGetComponent<IDamagable>(out var interactable))
            {
                Destroy(gameObject);
                return;
            }
            interactable.Damage(damage);
        }

        BulletEffects();
        Destroy(gameObject);
    }

    private void BulletEffects()
    {
        if (splittingProjectile)
        {
            Split();
        }

        if (explodingProjectile)
        {
            Explode();
        }
    }

    public IEnumerator ColliderActivateTimer()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        collider.enabled = true;
    }
}
