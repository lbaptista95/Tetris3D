using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject gameSettings;

    [SerializeField] private TMP_InputField gridSizeXInput;
    [SerializeField] private TMP_InputField gridSizeYInput;
    [SerializeField] private TMP_Text gridSizeErrorFeedback;

    [SerializeField] private GameObject instructionsWindow;

    private void Start()
    {
        gameSettings.SetActive(false);
        instructionsWindow.SetActive(false);

        gridSizeXInput.onValueChanged.AddListener(SetGridSizeFeedback);
        gridSizeYInput.onValueChanged.AddListener(SetGridSizeFeedback);
    }

    private void SetGridSizeFeedback(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            gridSizeErrorFeedback.text = string.Empty;
    }

    public void OpenGameSettings()
    {
        gameSettings.SetActive(true);

        gridSizeXInput.text = GameManager.instance.GridSizeX > 6 ? GameManager.instance.GridSizeX.ToString() : 6.ToString();
        gridSizeYInput.text = GameManager.instance.GridSizeY > 6 ? GameManager.instance.GridSizeY.ToString() : 6.ToString();
    }

    public void OpenQuitScreen()
    {
        GameManager.instance.SetQuitScreen(true);
    }
    public void StartGame()
    {
        int sizeX = int.Parse(gridSizeXInput.text);
        int sizeY = int.Parse(gridSizeYInput.text);

        if (sizeX < 6 || sizeY < 6)
        {
            gridSizeErrorFeedback.text = "Invalid size. Please, insert values greater than 6.";
            return;
        }

        gameSettings.SetActive(false);

        GameManager.instance.StartNewGame(sizeX, sizeY);
    }
}
