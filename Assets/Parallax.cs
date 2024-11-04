using UnityEngine;

public class Parllax : MonoBehaviour
{
    public Transform player; // Reference to the player (or the camera following the player)
    public float parallaxSpeedX = 0.5f; // Speed of parallax on the X-axis
    public float parallaxSpeedY = 0.3f; // Speed of parallax on the Y-axis

    private Vector3 previousPlayerPosition;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player transform not set on ParallaxBackground script.");
            return;
        }

        // Store the initial position of the player
        previousPlayerPosition = player.position;
    }

    void Update()
    {
        // Calculate the amount of movement on the X and Y axes since the last frame
        Vector3 deltaMovement = player.position - previousPlayerPosition;

        // Apply the parallax effect based on the specified speeds
        transform.position += new Vector3(deltaMovement.x * parallaxSpeedX, deltaMovement.y * parallaxSpeedY, 0);

        // Update the previous position of the player for the next frame
        previousPlayerPosition = player.position;
    }
}
