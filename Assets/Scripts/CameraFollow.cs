using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // The Camera object will follow the player object in the third person view

    public Transform player; // The player object
    public Vector3 offset; // The offset of the camera from the player object
    public Vector3 rotationOffset; // The rotation offset of the camera from the player object
    private Camera camera; // The camera object
    [SerializeField] private bool cursorState = false;
    // Variables for tracking vertical rotation
    private float verticalRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera = GetComponent<Camera>();

        // Lock the cursor
        lockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        // Set the position of the camera to the position of the player object plus the offset
        transform.position = player.position + offset;

        // Get the mouse inputs
        float horizontal = Input.GetAxis("Mouse X"); // Horizontal rotation for the player
        float vertical = Input.GetAxis("Mouse Y");   // Vertical rotation for the camera

        // Rotate the player object based on horizontal mouse movement
        player.Rotate(Vector3.up * horizontal);

        // Update the vertical rotation based on vertical mouse movement
        verticalRotation -= vertical;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // Optional clamping to avoid extreme angles

        // Set the camera rotation to match the player's horizontal rotation and the vertical mouse movement
        camera.transform.rotation = Quaternion.Euler(verticalRotation, player.eulerAngles.y, 0f) * Quaternion.Euler(rotationOffset);
    }


    public void setCursorState(bool state)
    {
        if (state)
        {
            unlockCursor();
        }
        else
        {
            lockCursor();
        }
    }

    public void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorState = false;
    }

    public void unlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorState = true;
    }
}
