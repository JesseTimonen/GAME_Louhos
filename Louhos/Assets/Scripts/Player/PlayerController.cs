using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;
    [SerializeField] private GameObject Highlight;
    [SerializeField] private Tilemap digTilemap;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject torch;
    [SerializeField] private BoxCollider2D torchChecker;
    [SerializeField] private LayerMask digLayer;
    [SerializeField] private Transform[] enemies;
    [SerializeField] private Volume volume;
    [SerializeField] private DayManager dayManager;
    [SerializeField] private float DrunknessDistortionMultiplier = 3f;
    [SerializeField] private float DrunknessHueShiftMultiplier = 2f;
    [SerializeField] private float DrunknessCameraShiftMultiplier = 1f;
    [SerializeField] private float CameraShiftMaxDistance = 3f;
    [SerializeField] private float staminaPotionDrunknessGain = 40f;
    public float drunknessLevel;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;
    private float currentHueShift = 0;

    [Header("Movement")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldownSeconds;
    [SerializeField] private Vector2 groundCheckBoxSize;
    [SerializeField] private float groundCheckCastDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 climbCheckBoxSize;
    [SerializeField] private float climbCheckCastDistance;

    [Header("Stamina costs")]
    [SerializeField] private int passiveStaminaCost = 1;
    [SerializeField] private int climbStaminaCost = 3;
    [SerializeField] private int jumpStaminaCost = 5;
    [SerializeField] [Range(0,1)]private float staminaPotionReplenish = 0.2f;
    private bool isPassiveStaminaCostActive = true;
    
    [Header("Tools")]
    [SerializeField] private ToolBase startingTool;
    public ToolBase CurrentTool;
    
    public UnityAction<Vector3, float> OnJump;
    public UnityAction<Vector3, float> OnLand; // TODO: Implement.
    public UnityAction<Vector3, float> OnWalk;
    public UnityAction<Vector3, float> OnClimb;
    public UnityAction<Vector3, float> OnDig;
    public UnityAction OnPlaceTorch;
    [HideInInspector] public bool Digging;
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public bool isClimbing = false;
    [HideInInspector] public bool isClimbingMoving = false;
    private Vector3 lookPosition;
    private Vector3 digPosition;
    public float horizontal;
    public float vertical;
    public bool shouldJump = false;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public bool isJumping = false;
    public bool isGrounded = true;
    private float tickrate = 0.6f;

    // For gizmos
    RaycastHit2D groundHit;

    private void Start()
    {
        drunknessLevel = 0;
        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        volume.profile.TryGet<LensDistortion>(out lensDistortion);
        InvokeRepeating(nameof(PassiveStaminaDrain), 1, 1);
        InvokeRepeating(nameof(Drunkness), 1, 1);
        InvokeRepeating(nameof(TriggerMovementActions), tickrate, tickrate);
        EquipTool(startingTool);
    }

    private void Drunkness()
    {
        drunknessLevel = Mathf.Max(0, drunknessLevel - 1);
    }


    private void OnEnable()
    {
        Digging = false;
        isPassiveStaminaCostActive = true;
    }

    private void OnDisable()
    {
        rb.velocity = Vector2.zero;
        playerAnimator.SetIsMoving(false);
        isPassiveStaminaCostActive = false;
    }


    private void Update()
    {
        MouseLook();
        Dig();
        UseTorch();
        UseStaminaPotion();
        Movement();
    }


    private void drunknessEffect()
    {
        if (drunknessLevel > 200)
        {
            dayManager.EndDay("wasted");
        }
        else if (drunknessLevel > 100)
        {
            float intensity = Mathf.Clamp((drunknessLevel - 100) / 100f, 0, 1);

            // Lens Distortion
            float lensDistortionIntensity = Mathf.Sin(Time.time * DrunknessDistortionMultiplier) * 0.5f * intensity;
            lensDistortion.intensity.value = lensDistortionIntensity;

            // Camera Shift
            float cameraShiftAmount = Mathf.Cos(Time.time * DrunknessCameraShiftMultiplier) * CameraShiftMaxDistance * intensity;
            mainCamera.position = new Vector3(transform.position.x + cameraShiftAmount, mainCamera.position.y, mainCamera.position.z);

            // Hue shifting
            if (drunknessLevel > 150)
            {
                currentHueShift = currentHueShift + (intensity * DrunknessHueShiftMultiplier);
                if (currentHueShift > 180)
                {
                    currentHueShift -= 360;
                }
                colorAdjustments.hueShift.value = currentHueShift;
            }
            else
            {
                colorAdjustments.hueShift.value = 0;
                currentHueShift = 0;
            }
        }
        else
        {
            lensDistortion.intensity.value = 0;
            colorAdjustments.hueShift.value = 0;
            currentHueShift = 0;
            mainCamera.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
    }


    private void ApplyGrayscaleEffectBasedOnClosestEnemy()
    {
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemyTransform in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemyTransform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyTransform.gameObject;
            }
        }

        if (closestEnemy != null && closestDistance <= 25f)
        {
            float effectIntensity = Mathf.Clamp01(1 - (closestDistance / 25f));
            colorAdjustments.saturation.value = Mathf.Lerp(0, -100, effectIntensity);
        }
        else
        {
            colorAdjustments.saturation.value = 0;
        }
    }


    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        Movementphysics();
        ApplyGrayscaleEffectBasedOnClosestEnemy();
        drunknessEffect();
    }


    private void PassiveStaminaDrain()
    {
        if (!isPassiveStaminaCostActive) return;

        int passiveEnergyCost = passiveStaminaCost;

        if (isClimbing)
        {
            passiveEnergyCost = climbStaminaCost;
        }

        playerInventory.RemoveStamina(passiveEnergyCost);
    }


    private void TriggerMovementActions()
    {
        if (isClimbing)
        {
            OnClimb?.Invoke(transform.position, Math.Max(0, MathF.Abs(vertical)));
        }
        else if (IsGrounded() && rb.velocity.magnitude > 0)
        {
            OnWalk?.Invoke(transform.position, rb.velocity.magnitude);
        }
    }
    

    void Movement()
    {
        // Capture movement
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Coyote jumping
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jumping inputs
        if (Input.GetButtonDown("Jump") && coyoteTimeCounter > 0)
        {
            shouldJump = true;
            coyoteTimeCounter = 0;
            playerAnimator.TriggerJumping();
        }

        // Variable jump height
        if (Input.GetButtonUp("Jump") && isJumping)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
            isJumping = false;
        }
    }


    void Movementphysics()
    {
        #region Jumping
        if (shouldJump)
        {
            playerInventory.RemoveStamina(jumpStaminaCost);
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
            shouldJump = false;
            playerAnimator.StopJumpParticles();
            isJumping = true;
            OnJump?.Invoke(transform.position, jumpForce);
        }
        #endregion

        #region Walking
        rb.velocity = new Vector2(horizontal * walkSpeed, rb.velocity.y);

        if (horizontal != 0)
        {
            playerAnimator.SetIsMoving(true);
        }
        else
        {
            playerAnimator.SetIsMoving(false);
        }
        #endregion

        #region Climbing
        if (CanClimb() && (!isGrounded || vertical > 0))
        {
            isClimbing = true;
            isClimbingMoving = false;

            if (vertical != 0)
            {
                isClimbingMoving = true;
                rb.velocity = new Vector2(0, climbSpeed * vertical);
            }
            else
            {
                isClimbingMoving = false;
            }
        }
        else
        {
            isClimbing = false;
            isClimbingMoving = false;
        }

        playerAnimator.SetClimbingMoving(isClimbingMoving);
        playerAnimator.SetIsClimbing(isClimbing);
        #endregion

        #region Facing
        if (horizontal > .01f)
        {
            climbCheckCastDistance = Math.Abs(climbCheckCastDistance);
            climbCheckBoxSize = new Vector2(Math.Abs(climbCheckBoxSize.x), climbCheckBoxSize.y);
            isFacingRight = true;
        }

        if (horizontal < -.01f)
        {
            climbCheckCastDistance = -Math.Abs(climbCheckCastDistance);
            climbCheckBoxSize = new Vector2(Math.Abs(climbCheckBoxSize.x), climbCheckBoxSize.y);
            isFacingRight = false;
        }
        #endregion
    }


    public void ActivateRunBoost()
    {
        walkSpeed *= 1.25f;
    }


    public void EquipTool(ToolBase newTool)
    {
        CurrentTool = newTool;
    }


    void UseStaminaPotion()
    {
        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.F))
        {
            if (playerInventory.HasStaminaPotions())
            {
                playerInventory.RemoveStaminaPotion(1);
                playerInventory.AddStamina(Mathf.Floor(playerInventory.GetMaxStamina() * staminaPotionReplenish));
                drunknessLevel += staminaPotionDrunknessGain;
            }
        }
    }


    void UseTorch()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.T))
        {
            OnPlaceTorch?.Invoke();
            List<GameObject> nearbyTorches = GetNearbyTorches();
            if (nearbyTorches.Count > 0)
            {
                foreach (GameObject torchObj in nearbyTorches)
                {
                    Destroy(torchObj);
                }

                playerInventory.AddTorch(nearbyTorches.Count);
            }
            else if (playerInventory.HasTorches())
            {
                Instantiate(torch, RoundTorchPosition(transform.position), Quaternion.identity);
                playerInventory.RemoveTorch(1);
            }
        }
    }

    private Vector3 RoundTorchPosition(Vector3 ogPos)
    {
        return new Vector3(Mathf.Floor(ogPos.x) + 0.5f, Mathf.Ceil(ogPos.y) - 0.25f, ogPos.z);
    }


    List<GameObject> GetNearbyTorches()
    {
        List<GameObject> foundTorches = new List<GameObject>();

        Vector2 size = torchChecker.size;
        Vector2 center = (Vector2)torchChecker.transform.position + torchChecker.offset;
        float angle = torchChecker.transform.eulerAngles.z;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag("Torch"))
            {
                foundTorches.Add(hit.gameObject);
            }
        }

        return foundTorches;
    }


    void Dig()
    {
        if (Digging || isClimbing || !isGrounded || !Highlight.activeSelf)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            playerAnimator.TriggerDigging(CurrentTool.Tier);
            digPosition = lookPosition;
            Digging = true;
        }
    }


    // Called from animator to time block breaking with animations
    public void BreakBlock()
    {
        if (playerInventory.HasStamina())
        {
            OnDig?.Invoke(digPosition, CurrentTool.Damage);
            playerInventory.RemoveStamina(CurrentTool.EnergyConsumption);
        }
    }


    public void EndDig()
    {
        Digging = false;
    }


    void MouseLook()
    {
        if (isClimbing || !isGrounded)
        {
            Highlight.SetActive(false);
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        Vector3 direction = mousePos - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, CurrentTool.Range, digLayer);

        Highlight.SetActive(false);

        if (hit.collider != null)
        {
            Vector3 hitPoint = hit.point;
            hitPoint.x = hit.point.x - 0.01f * hit.normal.x;
            hitPoint.y = hit.point.y - 0.01f * hit.normal.y;
            lookPosition = FloorVector3(hitPoint);

            if (CheckTileAtPosition(lookPosition))
            {
                Highlight.SetActive(true);
                Highlight.transform.position = lookPosition;
            }
        }
    }


    bool CheckTileAtPosition(Vector3 position)
    {
        return digTilemap.GetTile(digTilemap.WorldToCell(position)) != null;
    }


    Vector3 FloorVector3(Vector3 vector)
    {
        return new Vector3(
            math.floor(vector.x) + 0.5f,
            math.floor(vector.y) + 0.5f,
            math.floor(vector.z) + 0.5f
        );
    }


    bool IsGrounded()
    {
        var hit = Physics2D.BoxCast(transform.position - transform.up * groundCheckCastDistance / 2, groundCheckBoxSize, 0, -transform.up, groundCheckCastDistance, groundLayer);
        groundHit = hit;

        return hit;
    }


    bool CanClimb()
    {
        return Physics2D.BoxCast(
            new Vector2(transform.position.x, transform.position.y - .5f),
            climbCheckBoxSize,
            0,
            transform.right,
            climbCheckCastDistance,
            groundLayer);
    }


    private void OnDrawGizmos()
    {
        // REF: IsGrounded
        Gizmos.DrawWireCube(transform.position - transform.up * groundCheckCastDistance, groundCheckBoxSize);
        Gizmos.DrawWireSphere(groundHit.point, .25f);

        // REF: CanClimb
        Gizmos.DrawWireCube(new Vector2(transform.position.x + climbCheckCastDistance, transform.position.y - .5f), climbCheckBoxSize);
    }
}