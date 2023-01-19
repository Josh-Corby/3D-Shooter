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
            Debug.Log(other.name);
            StartCoroutine(IFrames());
        }
    }

    private IEnumerator IFrames()
    {
        Debug.Log("iframes start");
        col.enabled = false;
        yield return new WaitForSeconds(iFramesTime);
        Debug.Log("iframes end");
        col.enabled = true;
    }
}
