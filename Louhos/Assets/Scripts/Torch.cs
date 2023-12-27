using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Torch : MonoBehaviour
{
    private Transform player;
    private Light2D torchLight;
    public float maxLightDistanceFromPlayer = 10.0f;
    public float updateInterval = 0.25f;


    void Start()
    {
        torchLight = GetComponent<Light2D>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            this.enabled = false;
            return;
        }

        player = playerObject.transform;

        StartCoroutine(DistanceCheckCoroutine());
    }


    IEnumerator DistanceCheckCoroutine()
    {
        while (true)
        {
            if (player != null)
            {
                torchLight.enabled = Vector3.Distance(transform.position, player.position) <= maxLightDistanceFromPlayer;
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }
}
