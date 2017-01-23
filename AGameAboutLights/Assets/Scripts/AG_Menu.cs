using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Menu : MonoBehaviour {

    public GameObject levelsPanel, creditsPanel;

    public void displayLevels()
    {
        levelsPanel.SetActive(!levelsPanel.activeSelf);
    }

    public void displayCredits()
    {
        creditsPanel.SetActive(!creditsPanel.activeSelf);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
