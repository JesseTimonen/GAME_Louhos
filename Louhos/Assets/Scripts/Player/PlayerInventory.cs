using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<ValuablesInventory> valuables;
    [SerializeField] private List<GroundsInventory> grounds;
    [SerializeField] private int torches;
    [SerializeField] private int staminaPotions;
    [SerializeField] private float maxStamina = 1000f;
    [SerializeField] private float currentStamina;

    [Header("UI Texts")]
    [SerializeField] private Text goldText;
    [SerializeField] private Text amethystText;
    [SerializeField] private Text diamondText;
    [SerializeField] private Text dirtText;
    [SerializeField] private Text stoneText;
    [SerializeField] private Text bedrockText;
    [SerializeField] private Text dragonStoneText;
    [SerializeField] private Text torchesText;
    [SerializeField] private Text staminaPotionsText;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Text staminaText;


    private void Start()
    {
        currentStamina = maxStamina;
        UpdateGroundsUI();
        UpdateValuablesUI();
        UpdateTorchesUI();
        UpdateStaminaPotionsUI();
        UpdateStaminaUI();
    }


    public void AddGround(int ground, int amount)
    {
        grounds.FirstOrDefault(g => (int)g.Ground == ground)!.Amount += amount;
        UpdateGroundsUI();
    }


    public void RemoveGround(Grounds ground, int amount)
    {
        var inventoryItem = grounds.FirstOrDefault(v => v.Ground == ground);
        if (inventoryItem != null && inventoryItem.Amount >= amount)
        {
            inventoryItem.Amount -= amount;
            UpdateGroundsUI();
        }
    }


    public int GetGroundAmount(Grounds ground)
    {
        var inventoryItem = grounds.FirstOrDefault(g => g.Ground == ground);
        if (inventoryItem != null)
        {
            return inventoryItem.Amount;
        }
        return 0;
    }


    public void AddValuable(int valuable, int amount)
    {
        valuables.FirstOrDefault(v => (int)v.Valuable == valuable)!.Amount += amount;
        UpdateValuablesUI();
    }


    public void UseValuable(Valuables valuable, int amount)
    {
        // Complete a purchase and spend the valuables
        var inventoryItem = valuables.FirstOrDefault(v => v.Valuable == valuable);
        if (inventoryItem != null && inventoryItem.Amount >= amount)
        {
            inventoryItem.Amount -= amount;
            UpdateValuablesUI();
        }
    }


    public int GetValuableAmount(Valuables valuable)
    {
        var inventoryItem = valuables.FirstOrDefault(v => v.Valuable == valuable);
        if (inventoryItem != null)
        {
            return inventoryItem.Amount;
        }
        return 0;
    }


    public bool CheckValuableAmount(Valuables valuable, int requiredAmount)
    {
        // Make sure player has required valuables before the purchase is done
        ValuablesInventory inventoryItem = valuables.FirstOrDefault(v => v.Valuable == valuable);
        return inventoryItem != null && inventoryItem.Amount >= requiredAmount;
    }


    public void AddTorch(int amount)
    {
        torches += amount;
        UpdateTorchesUI();
    }


    public void RemoveTorch(int amount)
    {
        torches -= amount;
        UpdateTorchesUI();
    }


    public bool HasTorches()
    {
        return torches > 0;
    }


    public void AddStaminaPotion(int amount)
    {
        staminaPotions += amount;
        UpdateStaminaPotionsUI();
    }


    public void RemoveStaminaPotion(int amount)
    {
        staminaPotions -= amount;
        UpdateStaminaPotionsUI();
    }


    public bool HasStaminaPotions()
    {
        return staminaPotions > 0;
    }


    public void AddStamina(float amount)
    {
        currentStamina += amount;

        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }

        UpdateStaminaUI();
    }


    public void SetStamina(float amount)
    {
        currentStamina = amount;

        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }

        UpdateStaminaUI();
    }

    public void AddMaxStamina(float amount)
    {
        maxStamina += amount;
        currentStamina += amount;
        UpdateStaminaUI();
    }

    public float GetMaxStamina()
    {
        return maxStamina;
    }


    public void RemoveStamina(float amount)
    {
        currentStamina -= amount;

        if (currentStamina < 0)
        {
            currentStamina = 0;
        }

        UpdateStaminaUI();
    }


    public bool HasStamina()
    {
        return currentStamina > 0;
    }


    private void UpdateStaminaUI()
    {
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = currentStamina;
        staminaText.text = currentStamina + " / " + maxStamina;
    }


    private void UpdateTorchesUI()
    {
        torchesText.text = torches.ToString();
        torchesText.gameObject.SetActive(torches > 0);
        torchesText.transform.parent.gameObject.SetActive(torches > 0);
    }


    private void UpdateStaminaPotionsUI()
    {
        staminaPotionsText.text = staminaPotions.ToString();
        staminaPotionsText.gameObject.SetActive(staminaPotions > 0);
        staminaPotionsText.transform.parent.gameObject.SetActive(staminaPotions > 0);
    }


    private void UpdateGroundsUI()
    {
        foreach (var item in grounds)
        {
            switch (item.Ground)
            {
                case Grounds.Dirt:
                    dirtText.gameObject.SetActive(item.Amount > 0);
                    dirtText.transform.parent.gameObject.SetActive(item.Amount > 0);
                    dirtText.text = item.Amount.ToString();
                    break;
                case Grounds.Stone:
                    stoneText.gameObject.SetActive(item.Amount > 0);
                    stoneText.transform.parent.gameObject.SetActive(item.Amount > 0);
                    stoneText.text = item.Amount.ToString();
                    break;
                case Grounds.Bedrock:
                    bedrockText.gameObject.SetActive(item.Amount > 0);
                    bedrockText.transform.parent.gameObject.SetActive(item.Amount > 0);
                    bedrockText.text = item.Amount.ToString();
                    break;
                case Grounds.DragonStone:
                    dragonStoneText.gameObject.SetActive(item.Amount > 0);
                    dragonStoneText.transform.parent.gameObject.SetActive(item.Amount > 0);
                    dragonStoneText.text = item.Amount.ToString();
                    break;
            }
        }
    }


    private void UpdateValuablesUI()
    {
        foreach (var item in valuables)
        {
            switch (item.Valuable)
            {
                case Valuables.Gold:
                    goldText.gameObject.SetActive(item.Amount > 0);
                    goldText.transform.parent.gameObject.SetActive(item.Amount > 0);
                    goldText.text = item.Amount.ToString();
                    break;
                case Valuables.Amethyst:
                    amethystText.text = item.Amount.ToString();
                    amethystText.gameObject.SetActive(item.Amount > 0);
                    amethystText.transform.parent.gameObject.SetActive(item.Amount > 0);
                    break;
                case Valuables.Diamond:
                    diamondText.text = item.Amount.ToString();
                    diamondText.gameObject.SetActive(item.Amount > 0);
                    diamondText.transform.parent.gameObject.SetActive(item.Amount > 0);
                    break;
            }
        }
    }
}


[Serializable]
public class ValuablesInventory
{
    public Valuables Valuable;
    public int Amount;
}


[Serializable]
public class GroundsInventory
{
    public Grounds Ground;
    public int Amount;
}