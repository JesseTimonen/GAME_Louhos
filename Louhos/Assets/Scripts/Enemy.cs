using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float torchSpeed = 1.5f;
    [SerializeField] private Vector2 searchAreaMin;
    [SerializeField] private Vector2 searchAreaMax;
    [SerializeField] private float aggroDistance = 5f;
    [SerializeField] private float loseInterestDistance = 10f;
    [SerializeField] private float torchDestroyTime = 0.5f;
    [SerializeField] private EnemyMoveState moveState;
    private float torchDestroyTimer;
    
    private float moveSpeed;
    private GameObject player;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    private GameObject torchToDestroy;
    private bool reachedDest => Vector3.Distance(transform.position, targetPosition) < 0.1f;
    private List<GameObject> torchBuffer;


    void Start()
    {
        torchBuffer = new List<GameObject>();
        moveSpeed = baseSpeed;
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        torchDestroyTimer = torchDestroyTime;
        PickNextPatrolPoint();
    }


    void Update()
    {
        Move();
        FlipSprite();
    }


    private void CheckPlayerAggro()
    {
        // Do not chase player incase player is not active (day already ended)
        if (!playerController.isActiveAndEnabled)
        {
            ChangeState(EnemyMoveState.Patrolling);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (moveState != EnemyMoveState.ChasingPlayer && distanceToPlayer <= aggroDistance)
        {
            ChangeState(EnemyMoveState.ChasingPlayer);
        }
        else if (moveState == EnemyMoveState.ChasingPlayer)
        {
            if (distanceToPlayer > loseInterestDistance)
            {
                ChangeState(EnemyMoveState.Patrolling);
            }
            else
            {
                targetPosition = player.transform.position;
            }
        }
    }


    private void Move()
    {
        if (moveState == EnemyMoveState.GoingForTorch)
        {
            if (reachedDest)
            {
                torchDestroyTimer -= Time.deltaTime;
                if (torchDestroyTimer <= 0)
                {
                    Destroy(torchToDestroy);
                    torchBuffer.Remove(torchToDestroy);
                    torchDestroyTimer = torchDestroyTime;
                    if (torchBuffer.Any())
                    {
                        GetTorchFromBuffer();
                    }
                    else
                    {
                        ChangeState(EnemyMoveState.Patrolling);
                    }
                    return;
                }
            }
        }
        else
        {
            CheckPlayerAggro();
            if (moveState != EnemyMoveState.ChasingPlayer && reachedDest)
            {
                PickNextPatrolPoint();
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private void FlipSprite()
    {
        spriteRenderer.flipX = (targetPosition.x > transform.position.x);
    }

    private void ChangeState(EnemyMoveState state)
    {
        switch (state)
        {
            case EnemyMoveState.ChasingPlayer:
                targetPosition = player.transform.position;
                moveSpeed = chaseSpeed;
                moveState = EnemyMoveState.ChasingPlayer;
                break;
            case EnemyMoveState.Patrolling:
                PickNextPatrolPoint();
                moveSpeed = baseSpeed;
                moveState = EnemyMoveState.Patrolling;
                break;
            case EnemyMoveState.GoingForTorch:
                if (!torchBuffer.Any())
                {
                    ChangeState(EnemyMoveState.Patrolling);
                    return;
                }
                moveState = EnemyMoveState.GoingForTorch;
                moveSpeed = torchSpeed;
                GetTorchFromBuffer();
                break;
        }

    }
    private void GetTorchFromBuffer()
    {
        if (torchBuffer.Any())
        {
            var torch = torchBuffer.First();
            if (torch == null)
            {
                torchBuffer.Remove(torch);
                GetTorchFromBuffer();
                return;
            }
            targetPosition = torch.transform.position;
            torchToDestroy = torch;
            torch.tag = "TorchUnpickable";
        }
    }


    private void PickNextPatrolPoint()
    {
        float randomX = Random.Range(searchAreaMin.x, searchAreaMax.x);
        float randomY = Random.Range(searchAreaMin.y, searchAreaMax.y);
        targetPosition = new Vector3(randomX, randomY, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && playerController.isActiveAndEnabled)
        {
            dayManager.EndDay("feared");
        }
        else if (other.gameObject.CompareTag("TorchAggro"))
        {
            torchBuffer.Add(other.transform.parent.gameObject);
            if(moveState != EnemyMoveState.GoingForTorch)
                ChangeState(EnemyMoveState.GoingForTorch);
        }
    }

    private enum EnemyMoveState
    {
        Patrolling,
        GoingForTorch,
        ChasingPlayer
    }
}