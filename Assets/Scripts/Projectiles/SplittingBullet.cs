using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplittingBullet : BulletBase
{
    private Vector3[] CardinalDirections = new Vector3[]
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right,
        Vector3.up,
        Vector3.down,
    };

    [SerializeField]
    private GameObject bulletToSplitInto;

    [SerializeField]
    private float splitForce;

    [SerializeField]
    private float spread;

    [SerializeField]
    private float waitTime;

    public bool isHoming;

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Split();
            DamageEnemy(collision.gameObject);
        }
        return;
    }

    private void Start()
    {
        ValidateValues();
    }

    public override void ValidateValues()
    {
        base.ValidateValues();
        if(splitForce == 0)
        {
            splitForce = 20;
        }

        if(spread == 0)
        {
            spread = 5;
        }

        if(waitTime == 0)
        {
            waitTime = 1;
        }
    }
    private void Split()
    {
        for (int i = 0; i < CardinalDirections.Length-1; i++)
        {
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            Vector3 spreadDirection = new Vector3(x, y, 0);
            Vector3 directionWithSpread = CardinalDirections[i] + spreadDirection;
            GameObject splitBullet = Instantiate(bulletToSplitInto, transform.position, Quaternion.identity);
            splitBullet.GetComponent<BulletBase>().isSpawnedProjectile = true;
            splitBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread * splitForce, ForceMode.Impulse);
            splitBullet.GetComponent<BulletBase>().damage = (damage / 2);
            if (isHoming)
            {
                splitBullet.AddComponent<HomingBullet>();
            }
        }     
    }
   
}
