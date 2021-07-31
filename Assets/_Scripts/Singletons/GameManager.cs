using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
/// <summary>
/// Class that controls all the core parts of the game: score, time, dimensions. In the future it 
/// will control resolution and volume too.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;

    public int GridSizeX { get { return _gridSizeX; } }
    public int GridSizeY { get { return _gridSizeY; } }

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject quitScreen;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    private int time;
    private int score;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        pauseMenu.SetActive(false);
        gameOverScreen.SetActive(false);
        quitScreen.SetActive(false);
    }

    private void OnEnable()
    {
        TetrisSceneManager.OnSceneLoad += SetScene;
    }

    private void OnDisable()
    {
        TetrisSceneManager.OnSceneLoad -= SetScene;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            PauseGame();
    }

    public void StartNewGame(int gridSizeX, int gridSizeY)
    {
        _gridSizeX = gridSizeX;
        _gridSizeY = gridSizeY;

        TetrisSceneManager.instance.LoadScene("GameScene");
    }

    private void SetScene(string loadedScene)
    {
        switch (loadedScene)
        {
            case "GameScene":
                Time.timeScale = 1;                
                StartCoroutine(Timer());
                break;
        }
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            time++;

        }
    }

    public Vector2 GetGridSize()
    {
        Vector2 size = new Vector2(_gridSizeX, _gridSizeY);
        return size;
    }
    public void IncreaseScore()
    {
        score++;
    }

    public void GameOver()
    {
        Time.timeScale = 0;

        scoreText.text = $"Score: {score}";

        TimeSpan timeLeft = TimeSpan.FromSeconds(time);

        string timeFormatted = string.Format("{0:D2}:{1:D2}:{2:D2}",
                       timeLeft.Hours,
                       timeLeft.Minutes,
                       timeLeft.Seconds);

        timeText.text = $"Past time: {timeFormatted}";

        gameOverScreen.SetActive(true);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        quitScreen.SetActive(false);
    }

    public void Retry()
    {
        gameOverScreen.SetActive(false);
        Time.timeScale = 1;
        TetrisSceneManager.instance.LoadScene("GameScene");
    }

    public void GoBackToMenu()
    {
        gameOverScreen.SetActive(false);
        pauseMenu.SetActive(false);

        TetrisSceneManager.instance.LoadScene("MainMenu");
    }

    public void SetQuitScreen(bool state)
    {
        quitScreen.SetActive(state);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
