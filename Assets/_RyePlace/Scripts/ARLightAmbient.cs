using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(Light))]
public class ARLightAmbient : MonoBehaviour
{
    private Light light;

    void Start()
    {
        light = GetComponent<Light>();
        ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        light.intensity = eventArgs.lightEstimation.averageBrightness.Value;
        light.colorTemperature = eventArgs.lightEstimation.averageColorTemperature.Value;
    }

    void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
    }
}