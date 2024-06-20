using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameBoard gameBoard;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject returnPanel;

    void Start()
    {
        ShowMainMenu();
    }

    void Update()
    {
    }

    public void OnePlayerClick()
    {
        menuCanvas.SetActive(false);
        gameBoard.Reset(false, true);
    }

    public void TwoPlayerClick()
    {
        menuCanvas.SetActive(false);
        gameBoard.Reset(false, false);
    }

    public void AiVsAiClick()
    {
        menuCanvas.SetActive(false);
        gameBoard.Reset(true, true);
    }

    public void MainMenuClick()
    {
        ShowMainMenu();
    }


    public void ShowMainMenu()
    {
        menuCanvas.SetActive(true);
        mainMenuPanel.SetActive(true);
        returnPanel.SetActive(false);
        gameBoard.gameObject.SetActive(false);

    }

    public void ShowReturnPanel()
    {
        menuCanvas.SetActive(true);
        returnPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }
}
