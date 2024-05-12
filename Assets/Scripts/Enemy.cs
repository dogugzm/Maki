using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static Action OnEnemyTriggeredPlayer;
    [SerializeField] private PlayerDataSO playerDataSO;

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnEnemyTriggeredPlayer?.Invoke();
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(playerDataSO.slowDownFactor);
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
    }
}