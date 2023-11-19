using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Camera camera1;
    public Camera camera2;

    private Camera activeCamera;

    private void Start()
    {
        // Set the initial active camera
        activeCamera = camera1;
        activeCamera.gameObject.SetActive(true);
    }

    public void switchCam()
    {
        // Disable the currently active camera
        activeCamera.gameObject.SetActive(false);

        // Switch the active camera
        if (activeCamera == camera1)
        {
            activeCamera = camera2;
        }
        else
        {
            activeCamera = camera1;
        }

        // Enable the newly active camera
        activeCamera.gameObject.SetActive(true);
    }
}
