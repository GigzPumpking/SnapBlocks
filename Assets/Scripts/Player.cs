using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f; // The speed of the player object
    void Start() {

    }

    void Update() {
        InputHandler();
    }

    void InputHandler() {
        // Get the input from the keyboard
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move the player object
        transform.Translate(new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime);
    }

    void MoveHandler() {
        
    }
}
