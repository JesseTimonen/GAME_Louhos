using UnityEngine;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private GameObject goldTooltip;
    [SerializeField] private GameObject amethystTooltip;
    [SerializeField] private GameObject diamondTooltip;
    [SerializeField] private GameObject dirtTooltip;
    [SerializeField] private GameObject stoneTooltip;
    [SerializeField] private GameObject bedrockTooltip;
    [SerializeField] private GameObject dragonStoneTooltip;
    [SerializeField] private GameObject torchTooltip;
    [SerializeField] private GameObject staminaPotionTooltip;
    [SerializeField] private GameObject staminaTooltip;

    public void HoverGoldTooltip() => ActivateTooltip(goldTooltip);
    public void HoverAmethystTooltip() => ActivateTooltip(amethystTooltip);
    public void HoverDiamondTooltip() => ActivateTooltip(diamondTooltip);
    public void HoverDirtTooltip() => ActivateTooltip(dirtTooltip);
    public void HoverStoneTooltip() => ActivateTooltip(stoneTooltip);
    public void HoverBedrockTooltip() => ActivateTooltip(bedrockTooltip);
    public void HoverDragonStoneTooltip() => ActivateTooltip(dragonStoneTooltip);
    public void HoverTorchTooltip() => ActivateTooltip(torchTooltip);
    public void HoverStaminaPotionTooltip() => ActivateTooltip(staminaPotionTooltip);
    public void HoverStaminaTooltip() => ActivateTooltip(staminaTooltip);

    public void ExitGoldTooltip() => DisableTooltip(goldTooltip);
    public void ExitAmethystTooltip() => DisableTooltip(amethystTooltip);
    public void ExitDiamondTooltip() => DisableTooltip(diamondTooltip);
    public void ExitDirtTooltip() => DisableTooltip(dirtTooltip);
    public void ExitStoneTooltip() => DisableTooltip(stoneTooltip);
    public void ExitBedrockTooltip() => DisableTooltip(bedrockTooltip);
    public void ExitDragonStoneTooltip() => DisableTooltip(dragonStoneTooltip);
    public void ExitTorchTooltip() => DisableTooltip(torchTooltip);
    public void ExitStaminaPotionTooltip() => DisableTooltip(staminaPotionTooltip);
    public void ExitStaminaTooltip() => DisableTooltip(staminaTooltip);


    private void ActivateTooltip(GameObject tooltip)
    {
        tooltip.SetActive(true);
    }


    private void DisableTooltip(GameObject tooltip)
    {
        tooltip.SetActive(false);
    }
}