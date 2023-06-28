using UnityEngine;

public class Camera_controller : MonoBehaviour
{
    public float zoomOutMin = 2f;
    public float zoomOutMax = 5f;
    public float panSpeed = 0.1f;

    private Vector2 initialTouchPosition;
    public static bool isCameraMoving = false;
    [Header("not passing area")]
    public float minBoundaryX;
    public float maxBoundaryX;
    public float maxBoundaryY;
    public float minBoundaryY;

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (!isCameraMoving)
            {
                isCameraMoving = true;
                initialTouchPosition = (touchZero.position + touchOne.position) * 0.5f;
            }
            else
            {
                Vector2 currentTouchPosition = (touchZero.position + touchOne.position) * 0.5f;
                Vector2 panDirection = currentTouchPosition - initialTouchPosition;
                PanCamera(panDirection);
                initialTouchPosition = currentTouchPosition;
            }

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * 0.01f);
        }
        else
        {
            isCameraMoving = false;
        }
    }
    private void PanCamera(Vector2 panDirection)
    {
        Vector3 desiredPan = new Vector3(-panDirection.x, -panDirection.y, 0f) * panSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + desiredPan;

        // Apply boundary checks to restrict camera movement within an area
        newPosition.x = Mathf.Clamp(newPosition.x, minBoundaryX, maxBoundaryX);
        newPosition.y = Mathf.Clamp(newPosition.y, minBoundaryY, maxBoundaryY);

        transform.position = newPosition;
    }

    private void Zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}
