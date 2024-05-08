using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //UI REFERENCES
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject playerUI;

    //BTN REFERENCES
    [SerializeField] private SimpleButton restartButton;
    [SerializeField] private SimpleButton quitGameButton;
    [SerializeField] private SimpleButton menuButton;

    private static GameManager instance;

    public static GameManager GetInstance() => instance;

    private void Awake()
    {
        instance = this;
        ToggleUI(gameOverUI, false);
        ToggleUI(playerUI, true);
    }

    private void Start()
    {
        restartButton.OnClick += RestartGame;
        quitGameButton.OnClick += QuitGame;
    }

    public void PlayerDeath() 
    {
        ToggleUI(playerUI, false);
        ToggleUI(gameOverUI, true);
    }

    private void ToggleUI(GameObject UI,bool toggle)
    {
        UI.SetActive(toggle);
    }


    private void RestartGame()
    {
        Player.GetInstance().Respawn();
        ToggleUI(gameOverUI, false);
        ToggleUI(playerUI, true);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
