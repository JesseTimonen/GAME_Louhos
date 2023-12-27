using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D[] cursorTextureArray;
    [SerializeField] private float animationFrameTime;
    [SerializeField] private Vector2 cursorOffset = Vector2.zero;

    private int currentCursorFrame;
    private int cursorFrameCount;
    private float cursorFrameTimer;

    private void Awake()
    {
        currentCursorFrame = 0;
        cursorFrameTimer = animationFrameTime;
        cursorFrameCount = cursorTextureArray.Length;
        Cursor.SetCursor(cursorTextureArray[currentCursorFrame], cursorOffset, CursorMode.Auto);
    }

    private void Update()
    {
        if (animationFrameTime != 0)
        {
            cursorFrameTimer -= Time.deltaTime;
            if (cursorFrameTimer <= 0f)
            {
                cursorFrameTimer += animationFrameTime;
                currentCursorFrame = (currentCursorFrame + 1) % cursorFrameCount;
                Cursor.SetCursor(cursorTextureArray[currentCursorFrame], cursorOffset, CursorMode.Auto);
            }
        }
    }


}
