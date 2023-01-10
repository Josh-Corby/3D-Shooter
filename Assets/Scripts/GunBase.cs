using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GunBase : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private float shootForce;

    [SerializeField]
    private bool useSpread;
    [SerializeField]
    private int spread;
    [SerializeField]
    private LayerMask mask;

    public bool readyToFire;
    public float timeBetweenShots;

    private bool fireInput;
    private const float shootForceDampen = 5;

    public bool holdToFire;
    public bool burstFire;
    public int bulletsInBurst;
    public float timeBetweenBurstShots;
    Vector3 targetPoint;
    private void OnEnable()
    {
        InputManager.Fire += RecieveFireInput;
        InputManager.StopFiring += CancelFireInput;
    }
    private void Start()
    {
        readyToFire = true;
        if (bulletsInBurst == 0)
        {
            bulletsInBurst = 1;
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

    private void Update()
    {
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
    public virtual void FindTarget()
    {
        readyToFire = false;

        //Find the exact hit position using a raycast
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view


        //check if ray hits something
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            Debug.Log(hit.collider.gameObject);
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75); //Just a point far away from the player
        }
        Shoot();
    }
    private IEnumerator BurstFire()
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
        GameObject bulletGO = Instantiate(bullet, firePoint.position, Quaternion.identity);

        if (bullet.GetComponent<BulletBase>().isProjectile)
        {
            //Calculate spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            Vector3 spreadDirection = new Vector3(x, y, 0);

            Vector3 directionWithoutSpread = targetPoint - firePoint.position;
            Vector3 directionWithSpread = directionWithoutSpread + spreadDirection;
            if (useSpread)
            {
                bulletGO.transform.forward = directionWithSpread.normalized;
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithSpread * shootForce / shootForceDampen, ForceMode.Impulse);
            }
            else if (!useSpread)
            {
                bullet.transform.forward = directionWithoutSpread.normalized;
                bulletGO.GetComponent<Rigidbody>().AddForce(directionWithoutSpread * shootForce / shootForceDampen, ForceMode.Impulse);
            }
        }
        firePoint.DetachChildren();
    }

    private IEnumerator ResetShooting()
    {
        yield return new WaitForSeconds(timeBetweenShots);
        readyToFire = true;
    }
}
