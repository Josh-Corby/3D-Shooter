using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : GameBehaviour<PlayerManager>, IDamagable
{
    public float maxHealth;
    public float currentHealth;

    private CapsuleCollider col;
    [SerializeField]
    private float iFramesTime;
    private MeshRenderer renderer;
    private Color baseColor;

    public Grid lastGrid;

    new private void Awake()
    {
        col = GetComponent<CapsuleCollider>();
        renderer = GetComponentInParent<MeshRenderer>();
        baseColor = renderer.material.color;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grid"))
        {
            lastGrid = other.GetComponent<Grid>();
        }
    }

    public void Damage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(IFrames());
    }

    private IEnumerator IFrames()
    {
        col.enabled = false;
        renderer.material.color = Color.cyan;
        yield return new WaitForSeconds(iFramesTime);
        col.enabled = true;
        renderer.material.color = baseColor;
    }
}
