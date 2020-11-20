using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        enemy.SetActive(true);
    }
}
