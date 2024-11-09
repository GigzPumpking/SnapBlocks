using UnityEngine;

public class Player : MonoBehaviour {
    public float speed = 10.0f;
    public float pickUpRange = 2.0f;
    public Transform holdPosition; // Not needed for direct camera positioning
    public Camera playerCamera;
    public LineRenderer lineRenderer; // Assign in Editor
    private GameObject heldObject;
    private float zDistance = 2.0f; // Distance in front of the camera

    void Update() {
        InputHandler();

        // Handle pick-up and drop with mouse click
        if (Input.GetMouseButtonDown(0)) { 
            if (heldObject == null) {
                AttemptPickUp();
            } else {
                DropObject();
            }
        }
        
        // Call update position if holding an object
        if (heldObject != null) {
            UpdateHeldObjectPosition();
        }
    }

    void InputHandler() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);
    }

    void AttemptPickUp() {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Visualize the ray
        Debug.DrawRay(ray.origin, ray.direction * pickUpRange, Color.green);
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * pickUpRange);

        if (Physics.Raycast(ray, out hit, pickUpRange)) {
            GameObject objectHit = hit.collider.gameObject;
            if (objectHit.CompareTag("Obstacle")) {
                PickUpObject(objectHit);
            }
        }
    }

    void PickUpObject(GameObject obj) {
        heldObject = obj;
        heldObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics while held
    }

    void DropObject() {
        if (heldObject != null) {
            heldObject.GetComponent<Rigidbody>().isKinematic = false; // Re-enable physics

            MagneticObject magneticScript = heldObject.GetComponent<MagneticObject>();
            if (magneticScript != null) {
                magneticScript.SetCanCombine(true);
            }

            heldObject = null;
        }
    }

    void UpdateHeldObjectPosition() {
    // Determine a screen point in the lower right
    Vector3 screenPosition = new Vector3(Screen.width * 0.8f, Screen.height * 0.2f, zDistance);
    // Convert to world space considering the camera's orientation
    Vector3 worldPosition = playerCamera.ScreenToWorldPoint(screenPosition);

    // Update the object's position
    heldObject.transform.position = worldPosition;
}
}