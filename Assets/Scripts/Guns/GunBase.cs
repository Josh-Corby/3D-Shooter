using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class GunBase : GameBehaviour
{
    public static event Action OnReloadStart = null;
    public static event Action<GunBase> OnReloadDone = null;

    public static event Action<int> OnBulletFired = null;
    public static event Action<int> OnAmmoAdded = null;

    [SerializeField] protected GunSO gun;
    private Camera cam;
    protected GameObject firePoint;
    protected Transform firePointTransform;
    protected bool readyToFire;
    private bool fireInput;

    [HideInInspector] public Vector3 targetPoint;
    public LayerMask mask;
    [HideInInspector]
    public float distanceToTarget;

    #region GunStats
    [Header("Gun Stats")]
    private GameObject bulletToFire;
    private float damage;
    private float swapInTime;
    private User user;
    [HideInInspector] public int maxAmmo;
    [HideInInspector] public int ammoLeft;
    [HideInInspector] public int clipSize;
    [HideInInspector] public int bulletsRemainingInClip;
    [HideInInspector] public float reloadTime;

    [Header("Firing Stats")]
    private float shootForce;
    private float timeBetweenShots;
    private bool holdToFire;

    [Header("Spread Stats")]
    private bool useSpread;
    private float spreadAmount;

    [Header("Shotgun Stats")]
    private bool shotgunFire;
    private int shotsInShotgunFire;

    [Header("BurstFire Stats")]
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
    protected void AssignValues()
    {
        bulletToFire = gun.bulletToFire;
        damage = gun.damage;


        user = gun.user;
        if (user == User.Player)
        {
            maxAmmo = gun.maxAmmo;
            clipSize = gun.clipSize;
            reloadTime = gun.reloadTime;
            swapInTime = gun.swapInTime;
            ammoLeft = maxAmmo;
            bulletsRemainingInClip = 0;

            CheckBulletsToReload();            
        }

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
    }
    private void OnEnable()
    {
        if (user == User.Player)
        {
            InputManager.OnFire += RecieveFireInput;
            InputManager.OnStopFiring += CancelFireInput;
            UIManager.OnReloadAnimationDone += CheckBulletsToReload;
            InputManager.OnReload += CheckClipForReload;

            StartCoroutine(SwapInTimer());
        }
        else
        {
            readyToFire = true;
        }
    }
    private void OnDisable()
    {
        if (user == User.Player)
        {
            InputManager.OnFire -= RecieveFireInput;
            InputManager.OnStopFiring -= CancelFireInput;
            UIManager.OnReloadAnimationDone -= CheckBulletsToReload;
            InputManager.OnReload -= CheckClipForReload;
        }
        StopAllCoroutines();
    }

    protected virtual void Update()
    {
        FindTarget();
        CheckForFireInput();
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
    private IEnumerator SwapInTimer()
    {
        readyToFire = false;
        yield return new WaitForSeconds(swapInTime);
        readyToFire = true;
    }
    private void RecieveFireInput()
    {
        fireInput = true;
    }
    private void CancelFireInput()
    {
        fireInput = false;
    }
    protected virtual void CheckForFireInput()
    {
        if (bulletsRemainingInClip > 0)
        {
            if (fireInput && readyToFire && burstFire)
            {
                StartCoroutine(BurstFire());
            }

            else if (fireInput && readyToFire)
            {
                CheckForShotgunFire();
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
    protected IEnumerator BurstFire()
    {
        for (int i = 0; i < bulletsInBurst; i++)
        {
            if (user == User.Player)
            {
                if (bulletsRemainingInClip < 0)
                {
                    break;
                }
            }

            CheckForShotgunFire();
            yield return new WaitForSeconds(timeBetweenBurstShots);

        }
        StartCoroutine(ResetShooting());
    }
    protected void CheckForShotgunFire()
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
    public virtual void Shoot()
    {
        if (bulletToFire == null)
        {
            return;
        }
        GameObject bulletGO = null;
        if (bulletToFire.GetComponent<BulletBase>().hasRigidbody)
        {
           
            if (!useSpread)
            {
                Vector3 directionWithoutSpread = (targetPoint - firePointTransform.position).normalized;
                bulletGO = Instantiate(bulletToFire, firePointTransform.position, Quaternion.LookRotation(firePoint.transform.position, Vector3.up));
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
                bulletGO = Instantiate(bulletToFire, firePointTransform.position, firePointTransform.rotation * spreadRotation);
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithSpread * shootForce, ForceMode.Impulse);
            }
        }
        else
        {
            bulletGO = Instantiate(bulletToFire, firePointTransform.position, Quaternion.LookRotation(firePointTransform.position, Vector3.up));
            
        }
        bulletGO.GetComponent<BulletBase>().damage = damage;
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

        if (user == User.Player)
        {
            OnBulletFired(bulletsRemainingInClip);
        }

        if (bulletsRemainingInClip == 0)
        {
            if (ammoLeft > 0)
            {
                //start UI animation for reloading
                OnReloadStart?.Invoke();
            }
        }
    }
    private void CheckClipForReload()
    {
        if (bulletsRemainingInClip < clipSize)
        {
            OnReloadStart?.Invoke();
        }
    }

    private void CheckBulletsToReload()
    {
        if (ammoLeft == 0) return;

        //check how many bullets are left in current clip
        int bulletsToReload = clipSize - bulletsRemainingInClip;

        //check if there is enough ammo left for reload
        if (ammoLeft < bulletsToReload)
        {
            bulletsToReload = ammoLeft;
        }
        LoadBullets(bulletsToReload);
    }

    private void LoadBullets(int bulletsToReload)
    {
        bulletsRemainingInClip += bulletsToReload;
        ammoLeft -= bulletsToReload;

        if (PWM.CheckIfCurrentWeapon(this))
        {
            OnReloadDone(this);
        }
    }

    public void AddAmmo(int amount)
    {
        ammoLeft += amount;
        if (ammoLeft > maxAmmo)
        {
            ammoLeft = maxAmmo;
        }

        OnAmmoAdded(ammoLeft);
    }
}
