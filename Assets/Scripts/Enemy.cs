using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private PlayerDataSO playerDataSO;
    public static Action OnEnemyTriggeredPlayer; 

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
