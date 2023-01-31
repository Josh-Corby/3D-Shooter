using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunBase : GameBehaviour
{
    public bool playerWeapon;


    [SerializeField]
    protected GunSO gun;
    private Camera cam;
    protected GameObject firePoint;
    protected Transform firePointTransform;
    protected bool readyToFire;
    private bool fireInput;

    [HideInInspector]
    public Vector3 targetPoint;
    public LayerMask mask;
    [HideInInspector]
    public float distanceToTarget;

    #region GunStats
    private GameObject bulletToFire;
    [Header("Firing Options")]
    private float shootForce;
    private float timeBetweenShots;
    private bool holdToFire;
    [Header("Spread Options")]
    private bool useSpread;
    private float spreadAmount;

    [Header("Shotgun Options")]
    private bool shotgunFire;
    private int shotsInShotgunFire;

    [Header("BurstFire Options")]
    protected bool burstFire;
    private int bulletsInBurst;
    private float timeBetweenBurstShots;
    #endregion

    protected virtual void Awake()
    {
        cam = Camera.main;
        firePoint = FindChildGameObjectByName("FirePoint");
        firePointTransform = firePoint.transform;

        AssignValues();
    }
    private void OnEnable()
    {
        if (playerWeapon)
        {
            InputManager.Fire += RecieveFireInput;
            InputManager.StopFiring += CancelFireInput;
        }    
    }

    private void OnDisable()
    {
        if(playerWeapon)
        {
            InputManager.Fire -= RecieveFireInput;
            InputManager.StopFiring -= CancelFireInput;
        }     
    }
    protected virtual void Update()
    {
        FindTarget();
        CheckForShootCondition();
    }

    protected void AssignValues()
    {
        bulletToFire = gun.bulletToFire;
        shootForce = gun.shootForce;
        timeBetweenShots = gun.timeBetweenShots;
        holdToFire = gun.holdToFire;
        useSpread = gun.useSpread;
        spreadAmount = gun.spreadAmount;
        shotgunFire = gun.shotgunFire;
        shotsInShotgunFire = gun.shotsInShotgunFire;
        burstFire = gun.burstFire;
        bulletsInBurst = gun.bulletsInBurst;
        timeBetweenBurstShots = gun.timeBetweenBurstShots;
        readyToFire = true;
    }
    protected virtual void CheckForShootCondition()
    {
        if (fireInput && readyToFire && burstFire)
        {
            StartCoroutine(BurstFire());
        }

        else if (fireInput && readyToFire)
        {
            CheckFireType();
            StartCoroutine(ResetShooting());

            if (!holdToFire)
            {
                CancelFireInput();
            }
        }
    }


    protected virtual void FindTarget()
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
            targetPoint = ray.GetPoint(1000); //Just a point far away from the player
        }

        transform.LookAt(targetPoint);
    }

    private void RecieveFireInput()
    {
        fireInput = true;
    }

    private void CancelFireInput()
    {
        fireInput = false;
    }

    protected void CheckFireType()
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
    public IEnumerator BurstFire()
    {
        for (int i = 0; i < bulletsInBurst; i++)
        {
            CheckFireType();
            yield return new WaitForSeconds(timeBetweenBurstShots);
        }
        StartCoroutine(ResetShooting());
    }
    public virtual void Shoot()
    {
        if (bulletToFire == null)
        {
            return;
        }

        if (bulletToFire.GetComponent<BulletBase>().hasRigidbody)
        {
            if (!useSpread)
            {
                Vector3 directionWithoutSpread = (targetPoint - firePointTransform.position).normalized;
                GameObject bulletGO = Instantiate(bulletToFire, firePointTransform.position, Quaternion.LookRotation(firePoint.transform.position,Vector3.up));
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithoutSpread * shootForce, ForceMode.Impulse);
                bulletGO.transform.forward = directionWithoutSpread;
            }
            if (useSpread)
            {
                //Calculate spread
                float x = Random.Range(-spreadAmount, spreadAmount) * distanceToTarget;
                float y = Random.Range(-spreadAmount, spreadAmount) * distanceToTarget;
                float z = Random.Range(-spreadAmount, spreadAmount) * distanceToTarget;
                //apply random rotation
                Quaternion spreadRotation = Quaternion.Euler(x, y, z);

                //apply random direction
                Vector3 spreadDirection = new Vector3(x, y, z);

                //Debug.Log(spreadDirection);

                //add random direcion
                Vector3 targetSpreadOffset = targetPoint + spreadDirection;
                Vector3 directionWithSpread = (targetSpreadOffset - firePointTransform.position).normalized;

                //add random rotation
                GameObject bulletGO = Instantiate(bulletToFire, firePointTransform.position, firePointTransform.rotation * spreadRotation);
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithSpread * shootForce, ForceMode.Impulse);
            }
        }
        else
        {
            GameObject bulletGO = Instantiate(bulletToFire, firePointTransform.position, Quaternion.LookRotation(firePointTransform.position,Vector3.up));
        }
        firePointTransform.DetachChildren();
    }
    protected IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        readyToFire = true;
    }
}
