using UnityEngine;

public class MagneticObject : MonoBehaviour {
    private bool canCombine = false; // Flag to allow/disallow combination

    // Call this method when the object is dropped by the player
    public void SetCanCombine(bool value) {
        canCombine = value;
    }

    void OnCollisionEnter(Collision collision) {
        if (canCombine && collision.gameObject.CompareTag("Obstacle")) {
            CombineObjects(collision.gameObject);
        }
    }

    void CombineObjects(GameObject otherObject) {
        // Set for one-time combination handling
        canCombine = false;

        Rigidbody thisRigidbody = GetComponent<Rigidbody>();
        Rigidbody otherRigidbody = otherObject.GetComponent<Rigidbody>();

        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = otherRigidbody;

        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;

        otherObject.transform.parent = this.transform;
    }
}