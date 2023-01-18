using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : GameBehaviour
{
    public float damage;

    public bool isProjectile;

    public bool isSpawnedProjectile;

    private void Start()
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

        if(isProjectile == false)
        {
            isProjectile = true;
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
        enemy.GetComponent<Enemy>().Damage(damage);
        Destroy(gameObject);
    }

    private IEnumerator ColliderActivateTimer()
    {
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        collider.enabled = true;
    }

}
