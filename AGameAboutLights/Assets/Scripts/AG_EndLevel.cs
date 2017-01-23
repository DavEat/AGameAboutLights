using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AG_EndLevel : MonoBehaviour {

    public GameObject settingScreen;

    public static void SaveProgression()
    {
        string level = SceneManager.GetActiveScene().name;
        int levelDone = int.Parse(level.Remove(0, 5));
        if(!PlayerPrefs.HasKey("LevelDone") || levelDone < PlayerPrefs.GetInt("LevelDone"))
            PlayerPrefs.SetInt("LevelDone", levelDone);
        Debug.Log(PlayerPrefs.GetInt("LevelDone"));
    }

    public void nextLevel()
    {
        string level = SceneManager.GetActiveScene().name;
        int levelToLoad = int.Parse(level.Remove(0, 5)) + 1;
        SceneManager.LoadScene("Level" + levelToLoad);
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void accessSettings()
    {
        settingScreen.SetActive(!settingScreen.activeSelf);
    } 
}
