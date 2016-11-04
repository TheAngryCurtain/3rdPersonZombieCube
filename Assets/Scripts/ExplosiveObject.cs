using UnityEngine;
using System.Collections;

public class ExplosiveObject : MonoBehaviour
{
    public GameObject explosionParticle;

    private float force = 25f;
    private float radius = 8f;
    //private Rigidbody rb;

    void Awake()
    {
        //rb = GetComponent<Rigidbody>();
    }

    public void Explode()
    {
        Instantiate(explosionParticle, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        float distanceFromBoom;
        int numOfHits;
        for (int i = 0; i < colliders.Length; ++i)
        {
            Collider c = colliders[i];
            if (c == GetComponent<Collider>())
            {
                continue;
            }

            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, 0f, 0f, ForceMode.Impulse);
            }

            distanceFromBoom = (c.gameObject.transform.position - transform.position).magnitude;
            numOfHits = (int)(25 * (1 / distanceFromBoom));

            if (c.gameObject.tag == "Player")
            {
                c.gameObject.GetComponent<Character>().MultipleHits(numOfHits);
            }
            else if (c.gameObject.tag == "Zombie")
            {
                c.gameObject.GetComponent<Zombie>().MultipleHits(numOfHits);
            }
        }

        Destroy(this.gameObject);
    }
}
