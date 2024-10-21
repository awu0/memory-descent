using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    public Transform target; // Assign the player transform in the Inspector
    public float cameraHeight = 20f; // Vertical offset from the player
    public float orthographicSize = 10f;
    public float smoothingSpeed = 5f; // Adjust for smoothing effect

    private Vector3 fixedPosition; // To store the fixed X and Z positions

    void Start()
    {
        // Set orthographic projection
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = orthographicSize;

        // Rotate the camera to get an isometric view
        transform.rotation = Quaternion.Euler(23, 45, 0);

        // Initialize the fixed X and Z positions
        fixedPosition = transform.position;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Optionally smooth the camera's vertical movement
            float targetY = target.position.y + cameraHeight;
            float smoothedY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothingSpeed);

            // Update only the Y position to follow the player
            Vector3 cameraPosition = new Vector3(
                transform.position.x,
                smoothedY,
                transform.position.z
            );
            transform.position = cameraPosition;
        }
    }
}