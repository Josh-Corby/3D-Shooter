using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : GameBehaviour<PlayerManager>
{
    private CapsuleCollider col;
    [SerializeField]
    private float iFramesTime;

    public Grid lastGrid;

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

        if (other.CompareTag("Grid"))
        {
            lastGrid = other.GetComponent<Grid>();
        }
    }

    private IEnumerator IFrames()
    {
        col.enabled = false;
        yield return new WaitForSeconds(iFramesTime);
        col.enabled = true;
    }

}
