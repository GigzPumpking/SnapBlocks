using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float pickUpRange = 2.0f;
    public Transform holdPosition; // Not needed for direct camera positioning
    public Camera playerCamera;
    public LineRenderer lineRenderer; // Assign in Editor
    private GameObject heldObject;
    private float zDistance = 2.0f; // Distance in front of the camera

    void Update()
    {
        InputHandler();

        // Handle pick-up and drop with mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (heldObject == null)
            {
                AttemptPickUp();
            }
            else
            {
                DropObject();
            }
        }

        // Handle splitting objects with right-click (deleting original object and spawning two of the same objects at half the scale)
        if (Input.GetMouseButtonDown(1))
        {
            if (heldObject == null)
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, pickUpRange))
                {
                    GameObject objectHit = hit.collider.gameObject;
                    if (objectHit.CompareTag("Obstacle") || objectHit.CompareTag("CombinedObject"))
                    {
                        // Calculate the offset for the new objects
                        Vector3 offset = objectHit.transform.right * (objectHit.transform.localScale.x / 4);

                        // Scale factor for the new objects
                        Vector3 newScale = objectHit.transform.localScale / 1.5f;

                        // Create and position new objects
                        GameObject newObject1 = Instantiate(objectHit, objectHit.transform.position + offset, objectHit.transform.rotation);
                        newObject1.transform.localScale = newScale;
                        EnsureRigidbody(newObject1);

                        GameObject newObject2 = Instantiate(objectHit, objectHit.transform.position - offset, objectHit.transform.rotation);
                        newObject2.transform.localScale = newScale;
                        EnsureRigidbody(newObject2);

                        // Optionally, destroy the original object
                        Destroy(objectHit);
                    }
                }
            }
        }


        // Call update position if holding an object
        if (heldObject != null)
        {
            UpdateHeldObjectPosition();
        }
    }
    void InputHandler()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);
    }

    void AttemptPickUp()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpRange))
        {
            GameObject objectHit = hit.collider.gameObject;
            if (objectHit.CompareTag("Obstacle") || objectHit.CompareTag("CombinedObject"))
            {
                PickUpObject(objectHit);
            }
        }
    }

    void PickUpObject(GameObject obj)
    {
        heldObject = obj;
        heldObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics while held

        MagneticObject magneticScript = heldObject.GetComponent<MagneticObject>();

        if (magneticScript != null)
        {
            magneticScript.SetCanCombine(false);
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            // if heldObject still has a Rigidbody component
            if (heldObject.GetComponent<Rigidbody>() != null)
            {
                heldObject.GetComponent<Rigidbody>().isKinematic = false; // Re-enable physics
            }

            MagneticObject magneticScript = heldObject.GetComponent<MagneticObject>();
            if (magneticScript != null)
            {
                magneticScript.SetCanCombine(true);
            }

            heldObject = null;
        }
    }

    void UpdateHeldObjectPosition()
    {
        // Determine a screen point in the lower right
        Vector3 screenPosition = new Vector3(Screen.width * 0.8f, Screen.height * 0.2f, zDistance);
        // Convert to world space considering the camera's orientation
        Vector3 worldPosition = playerCamera.ScreenToWorldPoint(screenPosition);

        // Update the object's position
        heldObject.transform.position = worldPosition;
    }

    // Ensure the new object has a Rigidbody component
    void EnsureRigidbody(GameObject obj)
    {
        if (obj.GetComponent<Rigidbody>() == null)
        {
            obj.AddComponent<Rigidbody>();
        }
    }
}