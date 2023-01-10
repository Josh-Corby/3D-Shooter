using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [SerializeField]
    protected float damage;

    public bool isProjectile;


    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DamageEnemy(collision.gameObject);
        }
    }
    private void DamageEnemy(GameObject enemy)
    {
        enemy.GetComponent<Enemy>().Damage(damage);
        Destroy(gameObject);
    }
}
