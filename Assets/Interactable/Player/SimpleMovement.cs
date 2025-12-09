
namespace EJETAGame
{
    using UnityEngine;
    public class SimpleMovement : MonoBehaviour
    {
        public float moveSpeed = 5f; // Movement speed
        public float mouseSensitivity = 100f; // Mouse sensitivity

        private float xRotation = 0f;

        void Start()
        {
            // Lock the cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            // Call movement and camera methods
            MovePlayer();
            RotateCamera();
        }

        void MovePlayer()
        {
            // Get WASD input
            float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
            float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

            // Calculate movement direction
            Vector3 direction = transform.right * horizontal + transform.forward * vertical;

            // Move the player
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }

        void RotateCamera()
        {
            // Get mouse input
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Rotate the camera up/down (clamping to avoid flipping)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Apply rotation to the camera
            Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Rotate the player left/right
            transform.Rotate(Vector3.up * mouseX);
        }
    }

}