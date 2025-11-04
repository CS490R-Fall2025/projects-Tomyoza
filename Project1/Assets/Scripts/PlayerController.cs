using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // You can change these values in the Inspector to tweak the feel
    public float forwardSpeed = 15f;
    public float sideMoveSpeed = 10f;
    public float boostForce = 10f;
    public float speedIncreaseRate = 0.3f;
    public Transform firePoint;
    public float powerUpDuration = 7f;
    public AudioClip jumpSound;
    public AudioClip shootSound;
    public AudioClip hitSound;
    [Header("Dash Ability")]
    public float dashSpeedMultiplier = 2.5f;
    public float dashDuration = 2f;
    public float dashCooldown = 10f;
    private bool isDashReady = true;
    private bool isDashing = false;


    private Rigidbody rb;
    private bool isGrounded = true;
    private bool canShoot = false; // Player starts UNABLE to shoot
    private bool isShielded = false;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.isGameOver == true)
        {
            // If game is over, stop all movement immediately
            rb.linearVelocity = Vector3.zero;
            return;
        }
        forwardSpeed += speedIncreaseRate * Time.fixedDeltaTime;

        // Move the player left/right on the X-axis
        float horizontalInput = Input.GetAxis("Horizontal");
        float currentForwardSpeed = forwardSpeed;
        if (isDashing)
        {
            currentForwardSpeed *= dashSpeedMultiplier;
        }
        Vector3 newVelocity = new Vector3(horizontalInput * sideMoveSpeed, rb.linearVelocity.y, currentForwardSpeed);

        rb.linearVelocity = newVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        // Use "Jump" input for an upward boost
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rb.AddForce(Vector3.up * boostForce, ForceMode.Impulse);
            audioSource.PlayOneShot(jumpSound);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canShoot)
        {
            Shoot();
        }

        // Check if the player has fallen off the world
        if (transform.position.y < -5f && !GameManager.Instance.isGameOver)
        {
            // Instantly set health to 0 and trigger the game over
            GameManager.Instance.TakeDamage(GameManager.Instance.playerHealth);
        }

        // --- DASH LOGIC ---
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashReady && !GameManager.Instance.isGameOver)
        {
            StartCoroutine(ActivateDash());
        }
    }

    void Shoot()
    {
        // Create a new projectile at the firePoint's position and rotation
        GameObject projectile = ObjectPooler.Instance.GetProjectile();
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;
        projectile.GetComponent<Projectile>().OnSpawn(forwardSpeed);

        audioSource.PlayOneShot(shootSound);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Check if we just left the ground (by jumping or falling)
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Shooting"))
        {
            // Start the "can-shoot" power-up
            StartCoroutine(ShooterPowerUpTimer());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Shield"))
        {
            // Start the "shield" power-up
            StartCoroutine(ShieldPowerUpTimer());
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Obstacle") && !isShielded && !isDashing)  // Check if the object we collided with has the "Obstacle" tag
        {
            AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
            GameManager.Instance.TakeDamage(1);
            // Destroy the obstacle so we don't hit it again
            Destroy(other.gameObject);
        }
    }

    IEnumerator ShooterPowerUpTimer()
    {
        canShoot = true; // Turn on shooting
        yield return new WaitForSeconds(powerUpDuration); // Wait for 10 seconds
        canShoot = false; // Turn off shooting
    }

    IEnumerator ShieldPowerUpTimer()
    {
        isShielded = true; // Turn on shield
        yield return new WaitForSeconds(powerUpDuration); // Wait for 10 seconds
        isShielded = false; // Turn off shield
    }

    IEnumerator ActivateDash()
    {
        isDashReady = false;
        isDashing = true;

        yield return new WaitForSeconds(dashDuration); 

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown); 

        isDashReady = true;
    }
}
