using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenuController : MonoBehaviour
{
    private enum MenuOption { StartGame, Leaderboards, Credits, QuitGame }
    private MenuOption menuIndex = MenuOption.StartGame;
    [SerializeField] private MainMenuCamera mainMenuCamera;
    [SerializeField] private Text[] menuButtons;
    [SerializeField] private Color activeColor = Color.yellow;
    [SerializeField] private Color unactiveColor = Color.white;
    [SerializeField] private int activeFontSize = 50;
    [SerializeField] private int unactiveFontSize = 40;

    private void Start()
    {
        UpdateLinks();
    }


    private void Update()
    {
        if (mainMenuCamera.isAtMenu)
        {
            CheckActivationKeys();
            CheckNavigationKeys();
        }
    }


    private void CheckActivationKeys()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            ActivateLink();
        }
    }


    private void CheckNavigationKeys()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && menuIndex > MenuOption.StartGame)
        {
            menuIndex--;
            UpdateLinks();
        }
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && menuIndex < MenuOption.QuitGame)
        {
            menuIndex++;
            UpdateLinks();
        }
    }


    private void UpdateLinks()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            menuButtons[i].color = (MenuOption)i == menuIndex ? activeColor : unactiveColor;
            menuButtons[i].fontSize = (MenuOption)i == menuIndex ? activeFontSize : unactiveFontSize;
        }
    }


    private void ActivateLink()
    {
        switch (menuIndex)
        {
            case MenuOption.StartGame: StartGame(); break;
            case MenuOption.Leaderboards: OpenLeaderboards(); break;
            case MenuOption.Credits: OpenCredits(); break;
            case MenuOption.QuitGame: QuitGame(); break;
        }
    }


    // Unity mouse over event
    public void HoverStartGameButtonEvent()
    {
        menuIndex = MenuOption.StartGame;
        UpdateLinks();
    }

    // Unity mouse over event
    public void HoverCreditsButtonEvent()
    {
        menuIndex = MenuOption.Credits;
        UpdateLinks();
    }

    // Unity mouse over event
    public void HoverLeaderboardsButtonEvent()
    {
        menuIndex = MenuOption.Leaderboards;
        UpdateLinks();
    }

    // Unity mouse over event
    public void HoverQuitButtonEvent()
    {
        menuIndex = MenuOption.QuitGame;
        UpdateLinks();
    }

    // Unity mouse click event
    public void StartGame()
    {
        var camera = GameObject.Find("Main Camera");
        camera.SetActive(false);
    
        SceneManager.LoadScene("MainScene");
    }

    // Unity mouse click event
    public void OpenCredits()
    {
        mainMenuCamera.MoveToCreditsPosition();
    }

    // Unity mouse click event
    public void OpenLeaderboards()
    {
        mainMenuCamera.MoveToLeaderboardPosition();
    }

    // Unity mouse click event
    public void QuitGame()
    {
        Application.Quit();
    }

    // Unity mouse click event
    public void CancelCredits()
    {
        mainMenuCamera.MoveToDefaultPosition();
    }

    // Unity mouse click event
    public void ReturnLeaderboard()
    {
        mainMenuCamera.MoveToDefaultPosition();
    }
}