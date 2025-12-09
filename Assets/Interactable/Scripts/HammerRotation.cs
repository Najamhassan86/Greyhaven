
namespace EJETAGame
{
    using UnityEngine;

    /// <summary>
    /// Rotates a hammer (or any object) continuously for visual effect.
    /// Can be used on hammer pickups to make them stand out.
    /// </summary>
    public class HammerRotation : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 90f, 0f); //Rotation speed per axis (degrees per second)
        [SerializeField] private bool rotateOnX = false; //Enable rotation on X axis
        [SerializeField] private bool rotateOnY = true; //Enable rotation on Y axis (default: spinning like a coin)
        [SerializeField] private bool rotateOnZ = false; //Enable rotation on Z axis
        [SerializeField] private Space rotationSpace = Space.Self; //Rotate in local or world space

        [Header("Bobbing (Optional)")]
        [SerializeField] private bool enableBobbing = false; //Enable up/down bobbing motion
        [SerializeField] private float bobbingSpeed = 2f; //Speed of bobbing motion
        [SerializeField] private float bobbingAmount = 0.2f; //How far up/down to bob (in meters)
        [SerializeField] private bool useSineWave = true; //Use smooth sine wave for bobbing

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = false;

        private Vector3 startPosition; //Original position for bobbing
        private float bobbingTime = 0f;

        private void Start()
        {
            startPosition = transform.localPosition;

            if (enableDebugLogs)
            {
                Debug.Log($"[HammerRotation] Initialized on {gameObject.name}");
                Debug.Log($"[HammerRotation] Rotation Speed: {rotationSpeed}");
                Debug.Log($"[HammerRotation] Bobbing: {(enableBobbing ? "Enabled" : "Disabled")}");
            }
        }

        private void Update()
        {
            //Apply rotation
            Vector3 rotationDelta = Vector3.zero;
            
            if (rotateOnX)
            {
                rotationDelta.x = rotationSpeed.x * Time.deltaTime;
            }
            if (rotateOnY)
            {
                rotationDelta.y = rotationSpeed.y * Time.deltaTime;
            }
            if (rotateOnZ)
            {
                rotationDelta.z = rotationSpeed.z * Time.deltaTime;
            }

            if (rotationDelta != Vector3.zero)
            {
                transform.Rotate(rotationDelta, rotationSpace);
            }

            //Apply bobbing
            if (enableBobbing)
            {
                bobbingTime += Time.deltaTime * bobbingSpeed;
                
                float offset = 0f;
                if (useSineWave)
                {
                    //Smooth sine wave bobbing
                    offset = Mathf.Sin(bobbingTime) * bobbingAmount;
                }
                else
                {
                    //Linear bobbing (triangle wave)
                    offset = (Mathf.PingPong(bobbingTime, 2f) - 1f) * bobbingAmount;
                }

                Vector3 newPosition = startPosition;
                newPosition.y += offset;
                transform.localPosition = newPosition;
            }
        }

        /// <summary>
        /// Reset rotation and position to initial state
        /// </summary>
        public void ResetTransform()
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = startPosition;
            bobbingTime = 0f;
        }

        /// <summary>
        /// Stop rotation (useful when hammer is picked up)
        /// </summary>
        public void StopRotation()
        {
            rotationSpeed = Vector3.zero;
            enableBobbing = false;
        }
    }
}
