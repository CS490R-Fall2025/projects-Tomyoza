using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    
    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float distanceFromPlayer = 30f;
    [SerializeField] private float screenXOffset = 2.0f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-10f, 60f); // Limiting how far up or down you can look.
    [SerializeField] private Vector3 lookOffset = new Vector3(0, 2f, 0);

    private float yaw = 0f;
    private float pitch = 0f;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void Start()
    {
        InitializeCamera();
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null)
        {
            LateUpdate(); 
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Get Mouse Input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Calculate Rotation
        yaw += mouseX;
        pitch -= mouseY; // Subtracting Y flips the control (Standard FPS feel)

        // Clamp the pitch so you don't do backflips
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

        // Apply Rotation & Position
        Vector3 targetPosition = player.position + lookOffset;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 cameraPosition = targetPosition 
                                 - (rotation * Vector3.forward * distanceFromPlayer) 
                                 + (rotation * Vector3.right * screenXOffset);

        // Calculate position: Start at target, rotate, then move backwards
        transform.position = cameraPosition;
        transform.rotation = rotation;
    }
}