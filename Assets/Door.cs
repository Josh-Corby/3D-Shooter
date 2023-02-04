using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closePosition;

    private DoorTrigger trigger;
    [SerializeField] private float moveDuration;

    private void Awake()
    {
        openPosition = transform.position;
        closePosition = new Vector3(openPosition.x, transform.position.y - transform.localScale.y, openPosition.z);
        trigger = GetComponentInChildren<DoorTrigger>();
    }

    public void Open()
    {
        StartCoroutine(LerpPosition(openPosition, moveDuration));
    }

    public void Close()
    {
        StartCoroutine(LerpPosition(closePosition, moveDuration));
        trigger.enabled = false;
    }

    IEnumerator LerpPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }
}
