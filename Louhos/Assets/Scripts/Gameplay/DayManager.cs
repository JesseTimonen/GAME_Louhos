using UnityEngine;
using UnityEngine.UI;


public class DayManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform startingPosition;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject endDayView;

    [Header("UI Elements")]
    [SerializeField] private Text dayOverText;
    [SerializeField] private Text dayText;
    [SerializeField] private Text staminaIncreaseText;
    [SerializeField] private Text endDayViewDayText;
    [SerializeField] private Text endDayViewDirtText;
    [SerializeField] private Text endDayViewStoneText;
    [SerializeField] private Text endDayViewBedrockText;
    [SerializeField] private Text endDayViewDragonStoneText;
    [SerializeField] private GameObject endDayViewDirtParent;
    [SerializeField] private GameObject endDayViewStoneParent;
    [SerializeField] private GameObject endDayViewBedrockParent;
    [SerializeField] private GameObject endDayViewDragonStoneParent;

    [Header("Scripts")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private PurchaseManager purchaseManager;

    public int currentDay = 1;
    private bool dayAlreadyEnded = false;


    private void Start()
    {
        InvokeRepeating("CheckPlayerStamina", 1, 1);
    }


    private void CheckPlayerStamina()
    {
        if (!playerInventory.HasStamina())
        {
            EndDay();
        }
    }


    public void EndDay(string cause = "energy")
    {
        // Make sure we never end the day twice
        if (dayAlreadyEnded) return;
        dayAlreadyEnded = true;


        if (cause == "energy")
        {
            dayOverText.text = "Out of energy";
        }
        else if (cause == "surrender")
        {
            dayOverText.text = "You have decided to end the day early and return home";
        }
        else if (cause == "feared")
        {
            dayOverText.text = "You have collapsed in fear";
        }
        else if (cause == "wasted")
        {
            dayOverText.text = "What, who!, where.. what?.. Torille!!";
        }
        else
        {
            dayOverText.text = "Time to end the day";
        }

        CancelInvoke("CheckPlayerStamina");
        playerController.enabled = false;
        playerController.drunknessLevel = 0;
        endDayViewDayText.text = "End of day " + currentDay;
        string staminaIncrease = purchaseManager.isStaminaUpgradeBought ? "150" : "100";
        staminaIncreaseText.text = "As the sun sets and a new dawn rises, your urgy to dig more grows stronger,\r\nincreasing your maximum Stamina by " + staminaIncrease;

        RewardsFromDirt();
        RewardsFromStone();
        RewardsFromBedrock();
        RewardsFromDragonStone();

        currentDay++;
        endDayView.SetActive(true);
    }


    private void RewardFromGround(Grounds groundType, float chanceOfGold, GameObject endDayViewParent, Text endDayViewText)
    {
        int amount = playerInventory.GetGroundAmount(groundType);
        if (amount <= 0)
        {
            endDayViewParent.SetActive(false);
            return;
        }

        int goldEarned = 0;
        for (int i = 0; i < amount; i++)
        {
            if (Random.value < chanceOfGold)
            {
                goldEarned++;
            }
        }

        endDayViewParent.SetActive(true);
        endDayViewText.text = "You gained " + goldEarned + " gold from processing the " + groundType.ToString().ToLower();
        playerInventory.AddValuable(0, goldEarned);
        playerInventory.RemoveGround(groundType, amount);
    }

    private void RewardsFromDirt()
    {
        RewardFromGround(Grounds.Dirt, 0.1f, endDayViewDirtParent, endDayViewDirtText);
    }

    private void RewardsFromStone()
    {
        RewardFromGround(Grounds.Stone, 0.20f, endDayViewStoneParent, endDayViewStoneText);
    }

    private void RewardsFromBedrock()
    {
        RewardFromGround(Grounds.Bedrock, 0.35f, endDayViewBedrockParent, endDayViewBedrockText);
    }

    private void RewardsFromDragonStone()
    {
        RewardFromGround(Grounds.DragonStone, 0.5f, endDayViewDragonStoneParent, endDayViewDragonStoneText);
    }


    // Unity trigger event from "endDayView"
    public void Sleep()
    {
        endDayView.SetActive(false);
        shop.SetActive(true);
        dayText.text = currentDay.ToString();
        player.position = startingPosition.position;
    }


    // Unity trigger event from "shop"
    public void CloseShop()
    {
        shop.SetActive(false);
        int staminaIncrease = purchaseManager.isStaminaUpgradeBought ? 150 : 100;
        playerInventory.AddMaxStamina(staminaIncrease);
        playerInventory.SetStamina(playerInventory.GetMaxStamina());
        InvokeRepeating("CheckPlayerStamina", 1, 1);
        shopManager.ResetShopUI();
        dayAlreadyEnded = false;

        // Add a little delay so clicking UI, doesn't make player dig
        Invoke("ActivatePlayer", 0.33f);
    }

    private void ActivatePlayer()
    {
        playerController.enabled = true;
    }
}
