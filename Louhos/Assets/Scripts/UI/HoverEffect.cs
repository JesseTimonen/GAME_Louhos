using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color hoverColor = Color.red;
    [SerializeField] private float hoverSize = 30f;
    private Text text;
    private Color originalColor;
    private float originalSize;


    void Start()
    {
        text = GetComponent<Text>();

        // Store the original color and size
        originalColor = text.color;
        originalSize = text.fontSize;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
        text.fontSize = (int)hoverSize;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = originalColor;
        text.fontSize = (int)originalSize;
    }
}