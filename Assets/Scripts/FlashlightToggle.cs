using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    public Light flashlight;
    public KeyCode toggleKey = KeyCode.F;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (flashlight != null)
                flashlight.enabled = !flashlight.enabled;
        }
    }
}
