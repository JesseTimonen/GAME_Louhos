using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerController playerController;
    private Enemy[] enemies;
    private bool isPaused = false;


    private void Start()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new Enemy[enemyObjects.Length];

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            enemies[i] = enemyObjects[i].GetComponent<Enemy>();
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && (playerController.enabled || isPaused))
        {
            isPaused = !isPaused;
            UpdateGameStatus();
        }
    }

    private void UpdateGameStatus()
    {
        pauseMenu.SetActive(isPaused);

        playerController.enabled = !isPaused;

        foreach (Enemy enemy in enemies)
        {
            enemy.enabled = !isPaused;
        }
    }


    // Unity event triggers from UI elements
    public void ClickQuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ClickEndDay()
    {
        isPaused = false;
        UpdateGameStatus();
        dayManager.EndDay("surrender");
    }

    public void ClickReloadGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ClickResumeGame()
    {
        isPaused = false;
        // Small delay to avoid digging when clicking UI
        Invoke("UpdateGameStatus", 0.1f);
    }
}