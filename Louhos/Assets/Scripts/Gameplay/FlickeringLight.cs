using UnityEngine;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(Light2D))]
public class FlickeringLight : MonoBehaviour
{
    private Light2D flickerLight;
    [SerializeField] private float minIntensity = 0.8f;
    [SerializeField] private float maxIntensity = 1f;
    [SerializeField] private float frequency = 10f;


    void Start()
    {
        flickerLight = GetComponent<Light2D>();

        if (flickerLight == null)
        {
            Debug.LogError("FlickeringLight2D requires a Light2D component");
            this.enabled = false;
        }
    }


    void Update()
    {
        float lightIntensity = (Mathf.Sin(Time.time * frequency) + 1f) / 2f;
        lightIntensity = lightIntensity * (maxIntensity - minIntensity) + minIntensity;
        flickerLight.intensity = lightIntensity;
    }
}