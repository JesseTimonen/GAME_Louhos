using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightLevel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Light2D playerLight;
    [SerializeField] private Light2D PlayerHeadLight;

    [Header("Settings")]
    [SerializeField] private float playerLightIntensity = 0.7f;
    [SerializeField] private float playerHeadLightIntensity = 1.0f;
    [SerializeField] private float intensityAboveGround = 1.0f;
    [SerializeField] private float intensityBelowGround = 0.05f;
    [SerializeField] private float transitionSpeed = 1.0f;
    [SerializeField] private float transitionPoint = -1.0f;
    [SerializeField] private float intensityThreshold = 0.01f;


    void Update()
    {
        UpdateLightIntensity();
    }


    private void UpdateLightIntensity()
    {
        float targetIntensity = player.position.y > transitionPoint ? intensityAboveGround : intensityBelowGround;

        if (Mathf.Abs(globalLight.intensity - targetIntensity) > intensityThreshold)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, transitionSpeed * Time.deltaTime);
        }

        playerLight.intensity = player.position.y > transitionPoint ? 0 : playerLightIntensity;
        PlayerHeadLight.intensity = player.position.y > transitionPoint ? 0 : playerHeadLightIntensity;
    }
}
