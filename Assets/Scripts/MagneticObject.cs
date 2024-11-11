using UnityEngine;
using System.Collections.Generic;

public class MagneticObject : MonoBehaviour
{
    private bool canCombine = false; // Flag to allow/disallow combination

    // Call this method when the object is dropped by the player
    public void SetCanCombine(bool value)
    {
        canCombine = value;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (canCombine && (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("CombinedObject")))
        {
            CombineObjects(collision.gameObject);
        }
    }

    void CombineObjects(GameObject otherObject)
    {
        // Disable further combination
        canCombine = false;

        // Zero out velocities to avoid sudden movement
        Rigidbody thisRigidbody = GetComponent<Rigidbody>();
        Rigidbody otherRigidbody = otherObject.GetComponent<Rigidbody>();
        thisRigidbody.linearVelocity = Vector3.zero;
        thisRigidbody.angularVelocity = Vector3.zero;
        thisRigidbody.isKinematic = true;
        otherRigidbody.linearVelocity = Vector3.zero;
        otherRigidbody.angularVelocity = Vector3.zero;
        otherRigidbody.isKinematic = true;


        // Determine if we need to create a new parent object
        GameObject parentObject = new GameObject("CombinedObject");
        parentObject.transform.position = (transform.position + otherObject.transform.position) / 2;

        // Add Rigidbody to the parent and set combined mass
        Rigidbody parentRigidbody = parentObject.AddComponent<Rigidbody>();
        parentRigidbody.mass = (thisRigidbody ? thisRigidbody.mass : 0) + (otherRigidbody ? otherRigidbody.mass : 0);
        parentRigidbody.isKinematic = true; // Disable physics until we're ready

        // Set up initial parenting relationships
        SetupParentingAndTransferChildren(this.gameObject, parentObject);
        SetupParentingAndTransferChildren(otherObject, parentObject);

        // Transfer colliders to the parent
        TransferCollidersToParent(this.gameObject, parentObject);
        TransferCollidersToParent(otherObject, parentObject);

        // Optionally, destroy the child rigidbodies if they're no longer needed
        Destroy(thisRigidbody);
        Destroy(otherRigidbody);

        // Tag the parent object as a CombinedObject
        parentObject.tag = "CombinedObject";

        // Give the parent object a MagneticObject script
        MagneticObject magneticScript = parentObject.AddComponent<MagneticObject>();

        // Set the rotations of the children to zero
        foreach (Transform child in parentObject.transform)
        {
            child.localRotation = Quaternion.identity;
        }

        // Re-enable physics on the parent object
        parentRigidbody.isKinematic = false;
    }

    // Helper function to transfer colliders from a child object to the parent
    void TransferCollidersToParent(GameObject child, GameObject parent)
    {
        Collider[] childColliders = child.GetComponents<Collider>();

        foreach (Collider childCollider in childColliders)
        {
            if (childCollider is BoxCollider box)
            {
                BoxCollider newCollider = parent.AddComponent<BoxCollider>();

                if (child.CompareTag("Obstacle")) {
                    newCollider.size = new Vector3(box.size.x * child.transform.localScale.x,
                        box.size.y * child.transform.localScale.y,
                        box.size.z * child.transform.localScale.z);
                    

                    newCollider.center = child.transform.localPosition + box.center;
                } else if (child.CompareTag("CombinedObject")) {
                    newCollider.size = box.size;
                    newCollider.center = box.center;
                    Destroy(child);
                }
            }
            else
            {
                Debug.Log("Unsupported collider type: " + childCollider.GetType());
            }

            // Remove the collider from the child object
            Destroy(childCollider);
        }
    }

    void SetupParentingAndTransferChildren(GameObject originalObject, GameObject newParent)
    {
        if (originalObject.CompareTag("CombinedObject"))
        {
            // Create a list to store the children before changing the hierarchy
            List<Transform> children = new List<Transform>();

            // Populate the list with the children
            foreach (Transform child in originalObject.transform)
            {
                children.Add(child);
            }

            // Now transfer each child to the new parent
            foreach (Transform child in children)
            {
                child.parent = newParent.transform;
                Debug.Log("Adding child to new parent: " + child.name);
            }
        }
        else
        {
            // For an individual object, make it a child of the new parent
            originalObject.transform.parent = newParent.transform;
        }
    }
}
