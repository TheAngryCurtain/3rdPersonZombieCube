using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Spawner : MonoBehaviour
{
    public Transform spawnpoint;
    public GameObject zombiePrefab;
    public GameObject explosionPrefab;

    private Health health;
    private int maxZombies = 10;
    private int maxHealth = 20;
    private float spawnDelay = 5f;
    private List<GameObject> activeZombies;
    private Zombie current;

    void Start()
    {
        health = new Health(maxHealth);
        activeZombies = new List<GameObject>(maxZombies);

        InvokeRepeating("Spawn", 0f, spawnDelay);
    }

    private void Spawn()
    {
        if (activeZombies.Count == maxZombies)
        {
            CancelInvoke("Spawn");
            return;
        }

        GameObject zombie = Instantiate(zombiePrefab, spawnpoint.position, spawnpoint.rotation) as GameObject;
        activeZombies.Add(zombie);
    }

    public void UpdateZombiesGoal(Vector3 newPos)
    {
        for (int i = 0; i < activeZombies.Count; ++i)
        {
            if (activeZombies[i].transform != null)
            {
                current = activeZombies[i].transform.GetChild(0).GetComponent<Zombie>();

                // if they are dead, remove them
                if (current.IsDead)
                {
                    activeZombies.RemoveAt(i);
                }

                current.SetGoal(newPos);
            }
        }
    }

    public void Hit()
    {
        bool isDead = health.TakeHit();
        if (isDead)
        {
            GameManager.Instance.NotifySpawnerDead();
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.transform.parent.gameObject);
        }
    }
}
