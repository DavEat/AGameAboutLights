using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Menu : MonoBehaviour {

    public GameObject levelsPanel, creditsPanel;

    public void displayLevels()
    {
        levelsPanel.SetActive(!levelsPanel.active);
    }

    public void displayCredits()
    {
        creditsPanel.SetActive(!creditsPanel.active);
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
