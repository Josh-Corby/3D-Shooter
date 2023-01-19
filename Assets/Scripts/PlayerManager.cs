using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : GameBehaviour<PlayerManager>
{
    private CapsuleCollider col;
    [SerializeField]
    private float iFramesTime;

    private void Awake()
    {
        col = GetComponent<CapsuleCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            StartCoroutine(IFrames());
        }
    }

    private IEnumerator IFrames()
    {
        col.enabled = false;
        yield return new WaitForSeconds(iFramesTime);
        col.enabled = true;
    }
}
