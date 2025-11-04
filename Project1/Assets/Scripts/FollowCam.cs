using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform player; // A reference to the player's transform
    public Vector3 offset;   // The distance and angle from the player

    void LateUpdate()
    {
        // Make sure we have a player to follow
        if (player != null)
        {
            // Set the camera's position to be the player's position plus the offset
            transform.position = player.position + offset;
        }
    }
}
