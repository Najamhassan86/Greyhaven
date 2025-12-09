
namespace EJETAGame
{
    using UnityEngine;

    /// <summary>
    /// Rotates a key (or any object) continuously for visual effect.
    /// Can be used on key pickups to make them stand out and look more appealing.
    /// </summary>
    public class KeyRotation : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 180f, 0f); //Rotation speed per axis (degrees per second)
        [SerializeField] private bool rotateOnX = false; //Enable rotation on X axis
        [SerializeField] private bool rotateOnY = true; //Enable rotation on Y axis (default: spinning like a coin)
        [SerializeField] private bool rotateOnZ = false; //Enable rotation on Z axis
        [SerializeField] private Space rotationSpace = Space.Self; //Rotate in local or world space

        [Header("Bobbing (Optional)")]
        [SerializeField] private bool enableBobbing = true; //Enable up/down bobbing motion (default: enabled for keys)
        [SerializeField] private float bobbingSpeed = 1.5f; //Speed of bobbing motion
        [SerializeField] private float bobbingAmount = 0.15f; //How far up/down to bob (in meters)
        [SerializeField] private bool useSineWave = true; //Use smooth sine wave for bobbing

        [Header("Pulsing Glow (Optional)")]
        [SerializeField] private bool enablePulsing = false; //Enable pulsing scale effect
        [SerializeField] private float pulseSpeed = 2f; //Speed of pulsing
        [SerializeField] private float pulseAmount = 0.1f; //How much to scale (0.1 = 10% larger/smaller)
        [SerializeField] private Vector3 baseScale = Vector3.one; //Base scale of the object

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = false;

        private Vector3 startPosition; //Original position for bobbing
        private float bobbingTime = 0f;
        private float pulseTime = 0f;

        private void Start()
        {
            startPosition = transform.localPosition;
            baseScale = transform.localScale;

            if (enableDebugLogs)
            {
                Debug.Log($"[KeyRotation] Initialized on {gameObject.name}");
                Debug.Log($"[KeyRotation] Rotation Speed: {rotationSpeed}");
                Debug.Log($"[KeyRotation] Bobbing: {(enableBobbing ? "Enabled" : "Disabled")}");
                Debug.Log($"[KeyRotation] Pulsing: {(enablePulsing ? "Enabled" : "Disabled")}");
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

            //Apply pulsing scale
            if (enablePulsing)
            {
                pulseTime += Time.deltaTime * pulseSpeed;
                float scaleMultiplier = 1f + Mathf.Sin(pulseTime) * pulseAmount;
                transform.localScale = baseScale * scaleMultiplier;
            }
        }

        /// <summary>
        /// Reset rotation, position, and scale to initial state
        /// </summary>
        public void ResetTransform()
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = startPosition;
            transform.localScale = baseScale;
            bobbingTime = 0f;
            pulseTime = 0f;
        }

        /// <summary>
        /// Stop all rotation and effects (useful when key is picked up)
        /// </summary>
        public void StopRotation()
        {
            rotationSpeed = Vector3.zero;
            enableBobbing = false;
            enablePulsing = false;
        }

        /// <summary>
        /// Set rotation speed (useful for dynamic changes)
        /// </summary>
        public void SetRotationSpeed(Vector3 speed)
        {
            rotationSpeed = speed;
        }

        /// <summary>
        /// Enable/disable bobbing motion
        /// </summary>
        public void SetBobbing(bool enabled)
        {
            enableBobbing = enabled;
            if (!enabled)
            {
                transform.localPosition = startPosition;
            }
        }

        /// <summary>
        /// Enable/disable pulsing effect
        /// </summary>
        public void SetPulsing(bool enabled)
        {
            enablePulsing = enabled;
            if (!enabled)
            {
                transform.localScale = baseScale;
            }
        }
    }
}
