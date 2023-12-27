using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform menuPosition;
    [SerializeField] private Transform leaderboardPosition;
    [SerializeField] private Transform creditsPosition;
    [SerializeField] private float speed = 1.0f;

    [Header("Animators")]
    [SerializeField] private Animator MenuAnimator;
    [SerializeField] private Animator CreditsAnimator;
    [SerializeField] private Animator LeaderboardAnimator;

    [HideInInspector] public bool isAtMenu = false;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float journeyLength;
    private float startTime;


    private void Start()
    {
        MoveToDefaultPosition();
    }


    private void Update()
    {
        Inputs();
        AnimatemainCamera();
        UpdateAnimatorParameters();
        isAtMenu = mainCamera.position.y == menuPosition.position.y;
    }


    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            if (mainCamera.position.y == creditsPosition.position.y || mainCamera.position.y == leaderboardPosition.position.y)
            {
                MoveToDefaultPosition();
            }
        }
    }


    private void UpdateAnimatorParameters()
    {
        MenuAnimator.SetBool("isVisible", mainCamera.position.y == menuPosition.position.y);
        CreditsAnimator.SetBool("isVisible", mainCamera.position.y == creditsPosition.position.y);
        LeaderboardAnimator.SetBool("isVisible", mainCamera.position.y == leaderboardPosition.position.y);
    }


    private void AnimatemainCamera()
    {
        // Move mainCamera towards target position
        float newY = Mathf.SmoothStep(startPosition.y, endPosition.y, (Time.time - startTime) * speed / journeyLength);
        mainCamera.position = new Vector3(mainCamera.position.x, newY, mainCamera.position.z);
    }


    public void MoveToDefaultPosition()
    {
        SetDestination(menuPosition.position.y);
    }


    public void MoveToLeaderboardPosition()
    {
        SetDestination(leaderboardPosition.position.y);
    }


    public void MoveToCreditsPosition()
    {
        SetDestination(creditsPosition.position.y);
    }


    private void SetDestination(float posY)
    {
        startPosition = mainCamera.position;
        endPosition = new Vector3(mainCamera.position.x, posY, mainCamera.position.z);
        startTime = Time.time;
        journeyLength = Mathf.Abs(startPosition.y - posY);
    }
}