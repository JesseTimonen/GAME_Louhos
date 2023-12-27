using UnityEngine;
using UnityEngine.UI;

public class DepthManager : MonoBehaviour
{
    [SerializeField] private Text depthText;
    [SerializeField] private Text bestDepthText;
    [SerializeField] private Transform player;
    private float personalBest = 0f;
    private float currentDepth = 0f;

    private void Update()
    {
        currentDepth = Mathf.Floor(-player.transform.position.y);

        if (currentDepth < 0)
        {
            return;
        }

        if (currentDepth > personalBest)
        {
            personalBest = currentDepth;
            bestDepthText.text = "(Best: " + personalBest + " meters)";
        }

        depthText.text = currentDepth + " meters";
    }
}