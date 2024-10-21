using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IsometricCamera : MonoBehaviour
{
    public Transform target; // Assign the player transform in the Inspector
    public float cameraHeight = 20f; // Vertical offset from the player
    public float orthographicSize = 8f;
    public float smoothingSpeed = 5f; // Adjust for smoothing effect

    public float transitionDuration;        // Set via level controller
    private float oldOrthographicSize;       // Old ortho size. Changed in level controller only.
    private float targetOrthographicSize;    // Target ortho size. Changed in level controller only
    private float timeElapsed;               // controls alpha for lerp

    void Start()
    {
        // Set orthographic projection
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = orthographicSize;
        targetOrthographicSize = orthographicSize;

        // Rotate the camera to get an isometric view
        transform.rotation = Quaternion.Euler(23, 45, 0);
    }

    public void ChangeOrthographicSize(float change)
    {
        oldOrthographicSize = Camera.main.orthographicSize;
        targetOrthographicSize += change;
        timeElapsed = 0f;
    }

    void LateUpdate()
    {
        //if (target != null)
        //{
        //    // Optionally smooth the camera's vertical movement
        //    float targetY = target.position.y + cameraHeight;
        //    float smoothedY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * smoothingSpeed);

        //    // Update only the Y position to follow the player
        //    Vector3 cameraPosition = new Vector3(
        //        transform.position.x,
        //        smoothedY,
        //        transform.position.z
        //    );
        //    transform.position = cameraPosition;
        //}
        if (timeElapsed < transitionDuration)
        {
            float smoothedSize = Mathf.Lerp(oldOrthographicSize, targetOrthographicSize, timeElapsed/transitionDuration);
            Camera.main.orthographicSize = smoothedSize;

            timeElapsed += Time.deltaTime;
        }
        else
        {
            Camera.main.orthographicSize = targetOrthographicSize;
        }

    }
}