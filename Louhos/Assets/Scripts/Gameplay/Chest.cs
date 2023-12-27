using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour
{
    [SerializeField] private Text lootText;
    [SerializeField] private Text rewardText;
    [SerializeField] private Animator rewardAnimator;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Reward[] rewards;
    
    private Animator animator;
    private PlayerInventory playerInventory;
    private bool isOpened = false;
    
    // Rewards:
    private int fullWeight;
    


    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        playerInventory = gameManager.GetComponent<PlayerInventory>();
        animator = GetComponent<Animator>();
        lootText.text = "";
        InitRewards();
        Invoke("PositionChestOnGround", 2);
    }

    private void InitRewards()
    {
        fullWeight = rewards.Sum(r => r.chanceWeight);
        
    }

    private (Reward, int) GetReward()
    {
        var randNum = Random.Range(0, fullWeight);
        var sum = 0;
        for(var i = 0; i < rewards.Length; i++)
        {
            sum += rewards[i].chanceWeight;
            if (sum > randNum)
            {
                var reward = rewards[i];
                var amount = Random.Range(reward.minAmount, reward.maxAmount + 1);
                return (reward, amount);
                
            }
        }
        return (new Reward(), 0);
    }


    private void PositionChestOnGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 50f, groundLayer);
        if (hit.collider != null)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.6f, transform.position.z);
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (isOpened) return;

        if (other.gameObject.CompareTag("Player") )
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenChest();
            }

            lootText.text = "E";
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (isOpened) return;

        if (other.gameObject.CompareTag("Player"))
        {
            lootText.text = "";
        }
    }


    private void OpenChest()
    {
        isOpened = true;
        (Reward reward, int amount) rewardWithAmount = GetReward();
        
        switch (rewardWithAmount.reward.type)
        {
            case Valuables.Torch:
                playerInventory.AddTorch(rewardWithAmount.amount);
                break;
            case Valuables.StaminaPotion:
                playerInventory.AddStaminaPotion(rewardWithAmount.amount);
                break;
            default:
                Debug.Log("Adding " + rewardWithAmount.reward.type + " " + rewardWithAmount.amount);
                playerInventory.AddValuable((int)rewardWithAmount.reward.type, rewardWithAmount.amount);
                break;
        }
        rewardText.text = rewardWithAmount.amount.ToString();
        rewardText.color = rewardWithAmount.reward.textColor;
        rewardAnimator.runtimeAnimatorController = rewardWithAmount.reward.animation;
        animator.SetTrigger("Open Chest");
    }
    [Serializable]
    public struct Reward
    {
        public Valuables type;
        public RuntimeAnimatorController animation;
        public int minAmount;
        public int maxAmount;
        public Color textColor;
        public int chanceWeight;
    }
}

