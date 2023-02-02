using System.Collections;
using UnityEngine;
public class GunBase : GameBehaviour
{
    public bool playerWeapon;
    [SerializeField] protected GunSO gun;
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
    private float damage;
    private GameObject bulletToFire;

    [HideInInspector] public int maxAmmo;
    [HideInInspector] public int ammoLeft;
    [HideInInspector] public int clipSize;
    [HideInInspector] public int bulletsRemainingInClip;
    [HideInInspector] public float reloadTime;

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
            UIManager.OnReloadAnimationDone += Reload;
            InputManager.Reload += CheckClipForReload;
        }
    }

    private void OnDisable()
    {
        if (playerWeapon)
        {
            InputManager.Fire -= RecieveFireInput;
            InputManager.StopFiring -= CancelFireInput;
            UIManager.OnReloadAnimationDone -= Reload;
            InputManager.Reload -= CheckClipForReload;
        }
    }

    private void Start()
    {
        if (ammoLeft < clipSize)
        {
            bulletsRemainingInClip = ammoLeft;
        }
        else
        {
            bulletsRemainingInClip = clipSize;

        }
    }

    protected virtual void Update()
    {
        FindTarget();
        CheckForShootCondition();
    }

    protected void AssignValues()
    {
        readyToFire = true;

        bulletToFire = gun.bulletToFire;
        damage = gun.damage;
        maxAmmo = gun.maxAmmo;
        clipSize = gun.clipSize;
        reloadTime = gun.reloadTime;

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

        ammoLeft = maxAmmo;
    }
    protected virtual void CheckForShootCondition()
    {
        if (bulletsRemainingInClip > 0)
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
        else
        {
            return;
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
        ReduceAmmo();

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
            if (bulletsRemainingInClip > 0)
            {
                CheckFireType();
                yield return new WaitForSeconds(timeBetweenBurstShots);
            }
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
                GameObject bulletGO = Instantiate(bulletToFire, firePointTransform.position, Quaternion.LookRotation(firePoint.transform.position, Vector3.up));
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithoutSpread * shootForce, ForceMode.Impulse);
                bulletGO.transform.forward = directionWithoutSpread;
                bulletGO.GetComponent<BulletBase>().damage = damage;
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
                bulletGO.GetComponent<BulletBase>().damage = damage;
            }
        }
        else
        {
            GameObject bulletGO = Instantiate(bulletToFire, firePointTransform.position, Quaternion.LookRotation(firePointTransform.position, Vector3.up));
            bulletGO.GetComponent<BulletBase>().damage = damage;
        }
        firePointTransform.DetachChildren();
    }
    protected IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        readyToFire = true;
    }
    private void ReduceAmmo()
    {
        bulletsRemainingInClip -= 1;

        if (playerWeapon)
        {
            UI.UpdateGunAmmoText(this);
        }

        if (bulletsRemainingInClip == 0)
        {
            Reload();
        }
    }
    private void CheckClipForReload()
    {
        if(bulletsRemainingInClip < clipSize)
        {
            UI.StartReloading();
        }
    }
    private void Reload()
    {
        if (ammoLeft > 0)
        {
            if (bulletsRemainingInClip > 0)
            {
                ammoLeft -= (clipSize - bulletsRemainingInClip);
            }
            else
            {
                ammoLeft -= clipSize;
            }

            if (ammoLeft > clipSize)
            {
                bulletsRemainingInClip = clipSize;
            }
            else
            {
                bulletsRemainingInClip = ammoLeft;
            }

            UI.UpdateGunAmmoText(this);
        }
        //if ammo left is greater than 0
        //take clip size amount of bullets from ammo left
        //if clip size is larget than ammo left load ammo left bullets
        //subtract bullets loaded from ammo left
    }
    public void AddAmmo(int amount)
    {
        ammoLeft += amount;
        if (ammoLeft > maxAmmo)
        {
            ammoLeft = maxAmmo;
        }
        UI.UpdateGunAmmoText(this);
    }
}
