using System.Collections;
using UnityEngine;
using System;

public class PlayerManager : GameBehaviour<PlayerManager>, IDamagable
{

    public static event Action<float> OnCurrentHealthChange = null;
    public static event Action<float> OnMaxHealthChange = null;

    [HideInInspector] public GameObject player;
    [HideInInspector] public Transform playerTransform;

    public float maxHealth;
    public float currentHealth;
    private bool canTakeDamage;
    [SerializeField]
    private float iFramesTime;
    new private MeshRenderer renderer;
    private Color baseColor;

    public Grid lastGrid;

    new private void Awake()
    {
        player = transform.parent.gameObject;
        playerTransform = player.transform;
        renderer = GetComponentInParent<MeshRenderer>();
        baseColor = renderer.material.color;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        canTakeDamage = true;

        OnMaxHealthChange(maxHealth);
        OnCurrentHealthChange(currentHealth);
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
        if (canTakeDamage)
        {
            currentHealth -= damage;
            //StartCoroutine(IFrames());

            OnCurrentHealthChange(currentHealth);
        }
    }

    private IEnumerator IFrames()
    {
        canTakeDamage = false;
        renderer.material.color = Color.cyan;
        yield return new WaitForSeconds(iFramesTime);
        canTakeDamage = true;
        renderer.material.color = baseColor;
    }


}
