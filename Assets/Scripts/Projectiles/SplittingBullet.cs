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
    private float splitSpread;

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
            splitForce = 10;
        }

        if(splitSpread == 0)
        {
            splitSpread = 5;
        }

    }
    private void Split()
    {
        for (int i = 0; i < CardinalDirections.Length-1; i++)
        {
            Debug.Log("Split");
            float x = Random.Range(-splitSpread, splitSpread);
            float y = Random.Range(-splitSpread, splitSpread);
            Vector3 spreadDirection = new Vector3(x, y, 0);
            Vector3 directionWithSpread = (CardinalDirections[i] + spreadDirection);
            GameObject splitBullet = Instantiate(bulletToSplitInto, transform.position, Quaternion.identity);
            splitBullet.GetComponent<BulletBase>().isSpawnedProjectile = true;
            splitBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread * splitForce, ForceMode.Impulse);
            splitBullet.GetComponent<BulletBase>().damage = (damage / 2);
        }     
    }
   
}
