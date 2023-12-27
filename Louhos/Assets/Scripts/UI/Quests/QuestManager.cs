using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    [Header("Quest parents")]
    [SerializeField] private GameObject quest1;
    [SerializeField] private GameObject quest2;
    [SerializeField] private GameObject quest3;
    [SerializeField] private GameObject quest4;
    [SerializeField] private GameObject quest5;
    [SerializeField] private GameObject quest6;
    [SerializeField] private GameObject quest7;

    [Header("Quest progression labels")]
    [SerializeField] private Text quest1Progression;
    [SerializeField] private Text quest2Progression;
    [SerializeField] private Text quest3Progression;
    [SerializeField] private Text quest4Progression;
    [SerializeField] private Text quest5Progression;
    [SerializeField] private Text quest6Progression;
    [SerializeField] private Text quest7Progression;

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject submitScore;
    [SerializeField] private Text gameCompletedText;
    [SerializeField] private DayManager dayManager;

    private int questIndex = 1;

    private void Start()
    {
        quest1.SetActive(true);
        InvokeRepeating("CheckQuestProgress", 1, 1);
    }

    private void CheckQuestProgress()
    {
        switch (questIndex)
        {
            case 1:
                Quest1Handler();
                break;
            case 2:
                Quest2Handler();
                break;
            case 3:
                Quest3Handler();
                break;
            case 4:
                Quest4Handler();
                break;
            case 5:
                Quest5Handler();
                break;
            case 6:
                Quest6Handler();
                break;
            case 7:
                Quest7Handler();
                break;
            case 8:
                CancelInvoke("CheckQuestProgress");
                break;
        }
    }

    private void Quest1Handler()
    {
        int progress = playerInventory.GetValuableAmount(Valuables.Gold);
        quest1Progression.text = "Gold: " + progress + "/5";

        if (progress >= 5)
        {
            quest1.SetActive(false);
            quest2.SetActive(true);
            questIndex++;
            HandQuestRewards(5);
        }
    }

    private void Quest2Handler()
    {
        float progress = Mathf.Floor(-player.transform.position.y);
        quest2Progression.text = "Depth: " + progress + "/40";

        if (progress >= 40f)
        {
            quest2.SetActive(false);
            quest3.SetActive(true);
            questIndex++;
            HandQuestRewards(10, 5);
        }
    }

    private void Quest3Handler()
    {
        int progress = playerInventory.GetGroundAmount(Grounds.Stone);
        quest3Progression.text = "Stone: " + progress + "/1";

        if (progress >= 1)
        {
            quest3.SetActive(false);
            quest4.SetActive(true);
            questIndex++;
            HandQuestRewards(15, 5, 3);
        }
    }

    private void Quest4Handler()
    {
        int progress = playerInventory.GetValuableAmount(Valuables.Diamond);
        quest4Progression.text = "Diamonds: " + progress + "/10";

        if (progress >= 10)
        {
            quest4.SetActive(false);
            quest5.SetActive(true);
            questIndex++;
            HandQuestRewards(15);
        }
    }

    private void Quest5Handler()
    {
        float progress = Mathf.Floor(-player.transform.position.y);
        quest5Progression.text = "Depth: " + progress + "/140";

        if (progress >= 140f)
        {
            quest5.SetActive(false);
            quest6.SetActive(true);
            questIndex++;
            HandQuestRewards(10, 10, 10);
        }
    }

    private void Quest6Handler()
    {
        if (playerController.CurrentTool.Tier == 5)
        {
            quest6Progression.text = "Tool obtained: 1/1";
            quest6.SetActive(false);
            quest7.SetActive(true);
            questIndex++;
            HandQuestRewards(25, 25, 25);
        }
        else
        {
            quest6Progression.text = "Tool obtained: 0/1";
        }
    }

    private void Quest7Handler()
    {
        float progress = Mathf.Floor(-player.transform.position.y);
        quest7Progression.text = "Depth: " + progress + "/200";

        if (progress >= 200f)
        {
            quest7.SetActive(false);
            questIndex++;
            HandQuestRewards(1337);

            // Play end credits
            credits.SetActive(true);
            mainCamera.transform.parent = null;
            Invoke("ShowLeaderboard", 21f);
        }
    }

    public void ShowLeaderboard()
    {
        gameCompletedText.text = "Congratulations, you cleared the game in " + dayManager.currentDay + " days.";
        submitScore.SetActive(true);
    }


    public void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void HandQuestRewards(int? gold = null, int? amethyst = null, int? diamonds = null, int? maxStamina = null)
    {
        if (gold.HasValue)
        {
            playerInventory.AddValuable(0, gold.Value);
        }

        if (amethyst.HasValue)
        {
            playerInventory.AddValuable(1, amethyst.Value);
        }

        if (diamonds.HasValue)
        {
            playerInventory.AddValuable(2, diamonds.Value);
        }

        if (maxStamina.HasValue)
        {
            playerInventory.AddMaxStamina(maxStamina.Value);
        }
    }
}