using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    public float MoveSpeed => moveSpeed;
    [SerializeField] private float rotateSpeed = 720f; // Degrees/sec for facing rotation

    [Header("References")]
    [SerializeField] private Animator animator; // Optional: set if you have a running animation

    [Header("Combat")]
    public SwordAttack swordScript;
    public float comboResetTime = 1.0f;

    private Rigidbody rb;
    private Camera mainCamera;
    // COMBO VARIABLES
    private int comboCounter = 0;
    private float lastAttackTime = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        if (animator == null)
        {
            // It's fine if no animator; we just skip animation toggles
            Debug.LogWarning("PlayerController: Animator not assigned, running animation will be skipped.");
        }
    }

    private void Update()
    {
        Attack();
    }

    private void FixedUpdate()
    {
        RotateToMouse();

        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Transform cam = mainCamera != null ? mainCamera.transform : null;
        Vector3 camForward = cam ? cam.forward : Vector3.forward;
        Vector3 camRight   = cam ? cam.right   : Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * inputV + camRight * inputH;
        bool hasInput = moveDir.sqrMagnitude > 0.0001f;

        Vector3 currentVel = rb.linearVelocity;
        Vector3 desiredVel = hasInput ? moveDir.normalized * moveSpeed : Vector3.zero;
        desiredVel.y = currentVel.y;
        rb.linearVelocity = desiredVel;

        // If we are moving (hasInput), send 1.0. If stopped, send 0.0.
        if (animator != null)
        {
            // This matches the "Speed" parameter we created in the Animator window
            float animationSpeed = hasInput ? 1.0f : 0.0f;
            animator.SetFloat("Speed", animationSpeed, 0.1f, Time.fixedDeltaTime);
        }

        if (hasInput)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.fixedDeltaTime);
        }
    }

    private void RotateToMouse()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Vector3 lookPos = new Vector3(point.x, transform.position.y, point.z);
            transform.LookAt(lookPos);
        }
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // If too much time passed, reset combo to 0
            if (Time.time - lastAttackTime > comboResetTime)
            {
                comboCounter = 0;
            }

            // Go to next step
            comboCounter++;

            // If we go past 3, loop back to 1
            if (comboCounter > 3)
            {
                comboCounter = 1;
            }

            // Send data to Unity
            if (animator != null)
            {
                animator.SetInteger("Combo", comboCounter);
                animator.SetTrigger("Attack");
            }

            // Enable Sword Hitbox
            if (swordScript != null)
            {
                swordScript.isAttacking = true;
                
                // Cancel any old resets and schedule a new one
                CancelInvoke("ResetAttack"); 
                Invoke("ResetAttack", 0.5f); // Adjust 0.5f to match your animation length
            }

            // Update timer
            lastAttackTime = Time.time;
        }
    }

    void ResetAttack()
    {
        if (swordScript != null) swordScript.isAttacking = false;
    }
}
