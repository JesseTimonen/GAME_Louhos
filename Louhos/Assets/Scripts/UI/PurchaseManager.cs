using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseManager : MonoBehaviour
{
    // Struct to hold currency amount
    [System.Serializable]
    public struct CurrencyAmount
    {
        public Text text;
        public Valuables currency;
        public int amount;
    }


    // Struct for item prices
    [System.Serializable]
    public struct ItemPrice
    {
        public string identifier;
        public List<CurrencyAmount> prices;
    }


    [SerializeField] private List<ItemPrice> itemPrices;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Text errorMessageText;
    private Dictionary<string, bool> purchasedItems = new Dictionary<string, bool>();
    public bool isStaminaUpgradeBought = false;


    [Header("Purchase Buttons")]
    [SerializeField] private Text purchaseButtonShovel;
    [SerializeField] private Text purchaseButtonIronPick;
    [SerializeField] private Text purchaseButtonGoldPick;
    [SerializeField] private Text purchaseButtonDiamondPick;
    [SerializeField] private Text purchaseButtonSuperDrill;
    [SerializeField] private Text purchaseButtonUpgradeStamina;
    [SerializeField] private Text purchaseButtonUpgradeMovement;
    [SerializeField] private Text purchaseButtonUpgradeMining;
    [SerializeField] private Text purchaseButtonBuyTorch;
    [SerializeField] private Text purchaseButtonBuyStaminaPotion;
    [SerializeField] private Color purchaseButtonNormalColor = Color.black;
    [SerializeField] private Color purchaseButtonHoverColor = Color.white;
    [SerializeField] private Color purchaseButtonBoughtColor = Color.green;

    [SerializeField] private List<ToolBase> availableTools; 


    private void Start()
    {
        SetInitialPrices();
    }


    private void SetInitialPrices()
    {
        // Display initial prices in UI
        foreach (ItemPrice itemPrice in itemPrices)
        {
            foreach (CurrencyAmount currencyAmount in itemPrice.prices)
            {
                // Check if text component exists and if the amount is more than 0
                if (currencyAmount.amount > 0)
                {
                    // Set price
                    currencyAmount.text.text = FormatCurrencyText(currencyAmount.amount, currencyAmount.currency);
                }
                else
                {
                    // Hide prices that are 0
                    currencyAmount.text.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }


    private string FormatCurrencyText(int amount, Valuables currency)
    {
        string currencyName = currency.ToString();

        if (currency != Valuables.Gold)
        {
            if (amount != 1)
            {
                currencyName += "s";
            }
        }

        return amount + " " + currencyName;
    }


    private ItemPrice? GetItemPriceByIdentifier(string itemIdentifier)
    {
        foreach (ItemPrice itemPrice in itemPrices)
        {
            if (itemPrice.identifier == itemIdentifier)
            {
                return itemPrice;
            }
        }
        return null;
    }


    public bool CanAffordPurchase(ItemPrice itemPrice)
    {
        foreach (CurrencyAmount currencyAmount in itemPrice.prices)
        {
            if (!playerInventory.CheckValuableAmount(currencyAmount.currency, currencyAmount.amount))
            {
                return false;
            }
        }
        return true;
    }


    public void MakePurchase(string itemIdentifier)
    {
        // Check if the item has already been purchased
        if (purchasedItems.TryGetValue(itemIdentifier, out bool isPurchased) && isPurchased)
        {
            return;
        }

        ItemPrice? itemPrice = GetItemPriceByIdentifier(itemIdentifier);
        if (itemPrice != null && CanAffordPurchase(itemPrice.Value))
        {
            // The player can afford the item, deduct the cost from the player's inventory
            foreach (CurrencyAmount cost in itemPrice.Value.prices)
            {
                playerInventory.UseValuable(cost.currency, cost.amount);
            }

            GrantItemToPlayer(itemIdentifier);
        }
        else
        {
            errorMessageText.gameObject.SetActive(true);
            CancelInvoke("clearErrorMessage");
            Invoke("clearErrorMessage", 3.0f);
        }
    }


    private void clearErrorMessage()
    {
        errorMessageText.gameObject.SetActive(false);
    }


    private void GrantItemToPlayer(string itemIdentifier)
    {
        switch (itemIdentifier)
        {
            case "tool_shovel":
                Debug.Log("Shovel granted to the player.");
                GiveToolToPlayer(itemIdentifier);
                SetAsBought(purchaseButtonShovel, itemIdentifier);
                break;
            case "tool_iron_pick":
                Debug.Log("Iron pick granted to the player.");
                GiveToolToPlayer(itemIdentifier);
                SetAsBought(purchaseButtonIronPick, itemIdentifier);
                break;
            case "tool_gold_pick":
                Debug.Log("Gold pick granted to the player.");
                GiveToolToPlayer(itemIdentifier);
                SetAsBought(purchaseButtonGoldPick, itemIdentifier);
                break;
            case "tool_diamond_pick":
                Debug.Log("Diamond pick granted to the player.");
                GiveToolToPlayer(itemIdentifier);
                SetAsBought(purchaseButtonDiamondPick, itemIdentifier);
                break;
            case "tool_super_drill":
                Debug.Log("Super drill granted to the player.");
                GiveToolToPlayer(itemIdentifier);
                SetAsBought(purchaseButtonSuperDrill, itemIdentifier);
                break;
            case "upgrade_stamina":
                playerInventory.AddMaxStamina(playerInventory.GetMaxStamina() * 0.5f);
                isStaminaUpgradeBought = true;
                SetAsBought(purchaseButtonUpgradeStamina, itemIdentifier);
                break;
            case "upgrade_movement":
                playerController.ActivateRunBoost();
                SetAsBought(purchaseButtonUpgradeMovement, itemIdentifier);
                break;
            case "upgrade_mining":
                playerAnimator.ActivateMiningBoost();
                SetAsBought(purchaseButtonUpgradeMining, itemIdentifier);
                break;
            case "buy_torch":
                playerInventory.AddTorch(1);
                break;
            case "buy_stamina_potion":
                playerInventory.AddStaminaPotion(1);
                break;
            default:
                Debug.LogWarning("Unrecognized item identifier: " + itemIdentifier);
                break;
        }
    }


    public void SetAsBought(Text button, string itemIdentifier)
    {
        button.color = purchaseButtonBoughtColor;
        button.text = "Bought";
        purchasedItems[itemIdentifier] = true;

        // Hide the currency costs for the purchased item
        ItemPrice? itemPrice = GetItemPriceByIdentifier(itemIdentifier);
        if (itemPrice != null)
        {
            foreach (CurrencyAmount currencyAmount in itemPrice.Value.prices)
            {
                if (currencyAmount.text != null)
                {
                    currencyAmount.text.gameObject.SetActive(false);
                    currencyAmount.text.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }


    // Unity event triggers when UI purchase buttons have been pressed
    public void BuyShovel() => MakePurchase("tool_shovel");
    public void BuyIronPick() => MakePurchase("tool_iron_pick");
    public void BuyGoldPick() => MakePurchase("tool_gold_pick");
    public void BuyDiamondPick() => MakePurchase("tool_diamond_pick");
    public void BuySuperDrill() => MakePurchase("tool_super_drill");
    public void BuyStaminaUpgrade() => MakePurchase("upgrade_stamina");
    public void BuyMovementUpgrade() => MakePurchase("upgrade_movement");
    public void BuyMiningUpgrade() => MakePurchase("upgrade_mining");
    public void BuyTorch() => MakePurchase("buy_torch");
    public void BuyStaminaPotion() => MakePurchase("buy_stamina_potion");


    // Unity trigger events for Shop Purchases
    public void HoverPurchaseShovel() => HoverPurchaseButton(purchaseButtonShovel, true, "tool_shovel");
    public void ExitHoverPurchaseShovel() => HoverPurchaseButton(purchaseButtonShovel, false, "tool_shovel");
    public void HoverPurchaseIronPick() => HoverPurchaseButton(purchaseButtonIronPick, true, "tool_iron_pick");
    public void ExitHoverPurchaseIronPick() => HoverPurchaseButton(purchaseButtonIronPick, false, "tool_iron_pick");
    public void HoverPurchaseGoldPick() => HoverPurchaseButton(purchaseButtonGoldPick, true, "tool_gold_pick");
    public void ExitHoverPurchaseGoldPick() => HoverPurchaseButton(purchaseButtonGoldPick, false, "tool_gold_pick");
    public void HoverPurchaseDiamondPick() => HoverPurchaseButton(purchaseButtonDiamondPick, true, "tool_diamond_pick");
    public void ExitHoverPurchaseDiamondPick() => HoverPurchaseButton(purchaseButtonDiamondPick, false, "tool_diamond_pick");
    public void HoverPurchaseSuperDrill() => HoverPurchaseButton(purchaseButtonSuperDrill, true, "tool_super_drill");
    public void ExitHoverPurchaseSuperDrill() => HoverPurchaseButton(purchaseButtonSuperDrill, false, "tool_super_drill");
    public void HoverPurchaseStaminaUpgrade() => HoverPurchaseButton(purchaseButtonUpgradeStamina, true, "upgrade_stamina");
    public void ExitHoverPurchaseStaminaUpgrade() => HoverPurchaseButton(purchaseButtonUpgradeStamina, false, "upgrade_stamina");
    public void HoverPurchaseMovementUpgrade() => HoverPurchaseButton(purchaseButtonUpgradeMovement, true, "upgrade_movement");
    public void ExitHoverPurchaseMovementUpgrade() => HoverPurchaseButton(purchaseButtonUpgradeMovement, false, "upgrade_movement");
    public void HoverPurchaseMiningUpgrade() => HoverPurchaseButton(purchaseButtonUpgradeMining, true, "upgrade_mining");
    public void ExitHoverPurchaseMiningUpgrade() => HoverPurchaseButton(purchaseButtonUpgradeMining, false, "upgrade_mining");
    public void HoverPurchaseBuyTorch() => HoverPurchaseButton(purchaseButtonBuyTorch, true, "buy_torch");
    public void ExitHoverPurchaseBuyTorch() => HoverPurchaseButton(purchaseButtonBuyTorch, false, "buy_torch");
    public void HoverPurchaseBuyStaminaPotion() => HoverPurchaseButton(purchaseButtonBuyStaminaPotion, true, "buy_stamina_potion");
    public void ExitHoverPurchaseBuyStaminaPotion() => HoverPurchaseButton(purchaseButtonBuyStaminaPotion, false, "buy_stamina_potion");


    private void HoverPurchaseButton(Text button, bool isHovering, string itemIdentifier)
    {
        // Do not visualize already bought items
        if (purchasedItems.TryGetValue(itemIdentifier, out bool alreadyPurchased) && alreadyPurchased)
        {
            return;
        }

        button.color = isHovering ? purchaseButtonHoverColor : purchaseButtonNormalColor;
    }

    private void GiveToolToPlayer(string itemIdentifier)
    {
         var tool = availableTools.FirstOrDefault(tool => tool.Identifier == itemIdentifier);

        if (tool != null)
        {
            playerController.EquipTool(tool);
        }
    }
}