using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : GameBehaviour
{
    public float damage;
    public bool hasRigidbody;
    public bool isSpawnedProjectile;

    public virtual void Start()
    {
        ValidateValues();
        if (isSpawnedProjectile)
        {
            StartCoroutine(ColliderActivateTimer());
        }
    }

    public virtual void ValidateValues()
    {
        if(damage == 0)
        {
            damage = 2;
        }
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
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

}
