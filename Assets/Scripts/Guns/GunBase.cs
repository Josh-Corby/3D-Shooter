using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunBase : GameBehaviour
{
    private Camera cam;
    private GameObject firePoint;
    private Transform firePointTransform;
    

    [Header("Firing Options")]
    [SerializeField]
    private GameObject bulletToFire;
    [SerializeField]
    private float shootForce;
    [SerializeField]
    private bool useSpread;
    [SerializeField]
    private float spread;
    public float timeBetweenShots;
    public bool holdToFire;
    public bool shotgunFire;
    public int shotsInShotgunFire;
    public bool burstFire;
    public int bulletsInBurst;
    public float timeBetweenBurstShots;
    private bool readyToFire;
    private bool fireInput;
   

    Vector3 targetPoint;
    private LayerMask mask;
    float distanceToTarget;
    float distanceModifier = 70;

    private void OnEnable()
    {
        InputManager.Fire += RecieveFireInput;
        InputManager.StopFiring += CancelFireInput;
    }
    private void Awake()
    {
        cam = Camera.main;
        firePoint = FindChildGameObjectByName("FirePoint");
        firePointTransform = firePoint.transform;
    }
    private void Start()
    {
        ValidateValues();
    }
    private void Update()
    {
        LookAtScreenCentre();

        if (fireInput && readyToFire && burstFire)
        {
            StartCoroutine(BurstFire());
        }

        else if (fireInput && readyToFire)
        {
            FindTarget();
            StartCoroutine(ResetShooting());

            if (!holdToFire)
            {
                CancelFireInput();
            }
        }
    }

    private void ValidateValues()
    {
        readyToFire = true;
        CheckAimLayerMask();
        if (shootForce == 0)
        {
            shootForce = 15;
        }
        if (timeBetweenShots == 0)
        {
            timeBetweenShots = 0.2f;
        }

        if (burstFire)
        {
            if (bulletsInBurst == 0)
            {
                bulletsInBurst = 3;
            }

            if (timeBetweenBurstShots == 0)
            {
                timeBetweenBurstShots = 0.1f;
            }
        }
        if (shotgunFire)
        {
            shotsInShotgunFire = 5;

            if (!useSpread)
            {
                useSpread = true;
                spread = 0.1f;
                return;
            }
            
        }
        if (useSpread)
        {
            if (spread == 0)
            {
                spread = 0.1f;
            }
        }
    }

    private void LookAtScreenCentre()
    {
        //Find the exact hit position using a raycast
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view

        //check if ray hits something
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            distanceToTarget = hit.distance;
            //Debug.Log(hit.collider.gameObject);
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(50); //Just a point far away from the player
        }

        transform.LookAt(targetPoint);
    }

   
    private void CheckAimLayerMask()
    {
        //add enemy layer mask
        if (!(mask == (mask | (1 << LayerMask.NameToLayer("Enemy")))))
        {
            mask |= 1 << LayerMask.NameToLayer("Enemy");
        }
        //add ground layer mask
        //add enemy layer mask
        if (!(mask == (mask | (1 << LayerMask.NameToLayer("Ground")))))
        {
            mask |= 1 << LayerMask.NameToLayer("Ground");
        }
    }
    private void RecieveFireInput()
    {
        fireInput = true;
    }

    private void CancelFireInput()
    {
        fireInput = false;
    }

    protected virtual void FindTarget()
    {
        readyToFire = false;


        if (shotgunFire)
        {
            for (int i = 1; i < shotsInShotgunFire; i++)
            {
                Shoot();
            }
        }
        else
        {
            Shoot();
        }
    }
    protected IEnumerator BurstFire()
    {
        for (int i = 0; i < bulletsInBurst; i++)
        {
            FindTarget();
            yield return new WaitForSeconds(timeBetweenBurstShots);
        }
        StartCoroutine(ResetShooting());
    }
    protected virtual void Shoot()
    {
        if (bulletToFire == null)
        {
            return;
        }

        

        if (bulletToFire.GetComponent<BulletBase>().isProjectile)
        {
            if (!useSpread)
            {
                Vector3 directionWithoutSpread = (targetPoint - firePointTransform.position).normalized;
                GameObject bulletGO = Instantiate(bulletToFire, firePointTransform.position, firePointTransform.rotation);
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithoutSpread * shootForce, ForceMode.Impulse);
            }

            if (useSpread)
            {
                //Calculate spread
                float x = Random.Range(-spread, spread) * distanceToTarget;
                float y = Random.Range(-spread, spread) * distanceToTarget;
                float z = Random.Range(-spread, spread) * distanceToTarget;

                //apply random rotation
                Quaternion spreadRotation = Quaternion.Euler(x, y, z);

                //apply random direction
                Vector3 spreadDirection = new Vector3(x, y, z);

                //add random direcion
                Vector3 targetSpreadOffset = targetPoint + spreadDirection;
                Vector3 directionWithSpread = (targetSpreadOffset - firePointTransform.position).normalized;

                //add random rotation
                GameObject bulletGO = Instantiate(bulletToFire, firePointTransform.position, firePointTransform.rotation * spreadRotation);
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithSpread * shootForce, ForceMode.Impulse);
            }
          
        }
        firePointTransform.DetachChildren();
    }
    protected IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        readyToFire = true;
    }
}
