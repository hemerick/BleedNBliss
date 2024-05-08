using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    //UI REFERENCES
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject playerUI;

    //BTN REFERENCES
    [SerializeField] private SimpleButton restartButton;
    [SerializeField] private SimpleButton quitGameButton;
    [SerializeField] private SimpleButton menuButton;

    //BAR REFERENCES
    [SerializeField] private SliderBar expBar;
    [SerializeField] private SliderBar hpBar;

    //TXT REFERENCES
    [SerializeField] private GameObject xpDisplay;
    [SerializeField] private GameObject hpDisplay;
    [SerializeField] private GameObject currentLvlDisplay;
    [SerializeField] private GameObject nextLvlDisplay;
    TextMeshProUGUI xpTXT;
    TextMeshProUGUI hpTXT;
    TextMeshProUGUI currentLvlTXT;
    TextMeshProUGUI nextLvlTXT;

    private static GameManager instance;

    public static GameManager GetInstance() => instance;

    private void Awake()
    {
        instance = this;
        ToggleUI(gameOverUI, false);
        ToggleUI(playerUI, true);
        xpTXT = xpDisplay.GetComponent<TextMeshProUGUI>();
        hpTXT = hpDisplay.GetComponent<TextMeshProUGUI>();
        currentLvlTXT = currentLvlDisplay.GetComponent<TextMeshProUGUI>();
        nextLvlTXT = nextLvlDisplay.GetComponent<TextMeshProUGUI>();
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

    public void SetPlayerXPDisplay(int value, int maxValue)
    {
        expBar.SetValue(value, maxValue);
        xpTXT.text = value.ToString() + " / " + maxValue.ToString();
    }

    public void SetCurrentLvLDisplay(int lvl)
    {
        currentLvlTXT.text = lvl.ToString();
        lvl++;
        nextLvlTXT.text = lvl.ToString();
    }

    public void SetPlayerHPDisplay(int value, int maxValue) 
    { 
        hpBar.SetValue(value, maxValue);
        hpTXT.text = value.ToString() + " / " + maxValue.ToString();
    }

    private void RestartGame()
    {
        ObjectPool.GetInstance().DisableAllPoolObject();
        Player.GetInstance().gameObject.transform.SetPositionAndRotation(SpawnPoint.GetInstance().transform.position, Quaternion.identity);
        Player.GetInstance().Respawn();
        ToggleUI(gameOverUI, false);
        ToggleUI(playerUI, true);
        Spawner.GetInstance().newSpawnAmount = 3;
        Spawner.GetInstance().SpawnEnemy();
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
