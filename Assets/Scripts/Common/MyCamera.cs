using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public float speed = 10f;
    public float sensitivity = 2f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private bool currentlyLockedIn = false;
    private void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
            speed += 5.0f;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus)) {
            speed -= 5.0f;
        }

        
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            currentlyLockedIn = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            currentlyLockedIn = false;
        }

        if (!currentlyLockedIn) return;
        // Mouse look
        rotationX += Input.GetAxis("Mouse X") * sensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Limit vertical rotation

        transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0);

        // Movement controls
        float moveSpeed = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) transform.Translate(Vector3.forward * moveSpeed);
        if (Input.GetKey(KeyCode.S)) transform.Translate(Vector3.back * moveSpeed);
        if (Input.GetKey(KeyCode.A)) transform.Translate(Vector3.left * moveSpeed);
        if (Input.GetKey(KeyCode.D)) transform.Translate(Vector3.right * moveSpeed);
        if (Input.GetKey(KeyCode.Q)) transform.Translate(Vector3.down * moveSpeed);
        if (Input.GetKey(KeyCode.E)) transform.Translate(Vector3.up * moveSpeed);
    }
}
