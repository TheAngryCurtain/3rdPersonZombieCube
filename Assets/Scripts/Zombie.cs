using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    public GameObject ammoDrop;
    public GameObject healthDrop;

    private NavMeshAgent agent;
    private Health health;
    private int maxHealth = 10;
    private int dropChance = 2;
    private int ammoDropChance = 7;
    public bool IsDead;

    void Awake()
    {
        agent = transform.GetComponent<NavMeshAgent>();
        health = new Health(maxHealth);
    }

    public void SetGoal(Vector3 g)
    {
        if (agent.enabled)
        {
            agent.destination = g;
        }
    }

    public void Hit()
    {
        IsDead = health.TakeHit();
        if (IsDead)
        {
            DetermineDrop();
            GameManager.Instance.Data.zombiesKilled += 1;
            GetComponent<Rigidbody>().freezeRotation = false;
            agent.enabled = false;
        }
    }

    public void MultipleHits(int numHits)
    {
        for (int i = 0; i < numHits; ++i)
        {
            Hit();
        }
    }

    private void DetermineDrop()
    {
        int r = UnityEngine.Random.Range(0, 10);
        
        if (r < dropChance)
        {
            r = UnityEngine.Random.Range(0, 10);
            if (r < ammoDropChance)
            {
                Instantiate(ammoDrop, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(healthDrop, transform.position, Quaternion.identity);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ShootableObject")
        {
            other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(5f, other.contacts[0].point, 0f, 0f, ForceMode.Impulse);
        }
    }
}
