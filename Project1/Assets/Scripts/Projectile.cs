using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 1f;

    private Rigidbody rb;

    void Awake()
    {
        // Get the Rigidbody component when the object is first created
        rb = GetComponent<Rigidbody>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnSpawn(float playerForwardSpeed)
    {
        // Calculate the total speed
        float totalSpeed = playerForwardSpeed + speed;

        // Apply velocity
        rb.linearVelocity = transform.forward * totalSpeed;
        StartCoroutine(ReturnToPoolAfterTime());
    }

    // This coroutine is the timer to return the object to the pool
    IEnumerator ReturnToPoolAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        // Return this projectile to the pool
        ObjectPooler.Instance.ReturnProjectile(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // Give the player 100 points
            GameManager.Instance.AddScore(100);

            Destroy(other.gameObject);

            // Stop the timer coroutine in case it's still running
            StopAllCoroutines();

            ObjectPooler.Instance.ReturnProjectile(gameObject);
        }
    }
}
