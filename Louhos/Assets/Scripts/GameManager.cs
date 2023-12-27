using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject GroundTileMap;
    [SerializeField] private GameObject Player;
    [SerializeField] private ShopManager ShopManager;

    private PlayerController playerCtrlr;
    private MapManager mapManager;  
    public PlayerInventory playerInventory;
    [HideInInspector] public AudioManager audioManager;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        mapManager = GroundTileMap.GetComponent<MapManager>();
        playerCtrlr = Player.GetComponent<PlayerController>();
        playerInventory = gameObject.GetComponent<PlayerInventory>();
        audioManager = Player.GetComponentInChildren<AudioManager>();
        ConnectEvents();
    }

    public void AddGround(int ground, int amount)
    {
        playerInventory.AddGround(ground, amount);
    }

    public void AddValuable(int valuable, int amount)
    {
        playerInventory.AddValuable(valuable, amount);
    }

    void ConnectEvents()
    {
        playerCtrlr.OnDig += mapManager.Dig;
        playerCtrlr.OnDig += audioManager.Dig;
        playerCtrlr.OnClimb += audioManager.Climb;
        playerCtrlr.OnWalk += audioManager.Walk;
        playerCtrlr.OnJump += audioManager.Jump;
        playerCtrlr.OnLand += audioManager.Land;
        playerCtrlr.OnPlaceTorch += audioManager.PlaceItem;
        mapManager.OnEndDig += playerCtrlr.EndDig;
        ShopManager.OnPageChange += audioManager.PageTurn;
    }
}