using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AG_EndLevel : AG_Singleton<AG_EndLevel> {

    public UnityEngine.UI.Button buttonNext;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public static void SaveProgression()
    {
        //string level = SceneManager.GetActiveScene().name;
        int levelDone = AG_SelectLevelManager.inst.levelIndex;//int.Parse(level.Remove(0, 5));
        if (levelDone > PlayerPrefs.GetInt("LevelDone") || !PlayerPrefs.HasKey("LevelDone"))
            PlayerPrefs.SetInt("LevelDone", levelDone);
        Debug.Log(PlayerPrefs.GetInt("LevelDone"));
    }

    public void SetVictoryScreen(bool value)
    {
        gameObject.SetActive(value);
        if (buttonNext != null)
        {
            buttonNext.gameObject.SetActive(true);
            bool interactable = AG_SelectLevelManager.inst.savesList.Length > AG_SelectLevelManager.inst.levelIndex;
            buttonNext.interactable = interactable;
            buttonNext.transform.GetComponentInChildren<UnityEngine.UI.Text>().color = interactable ? new Color(0.38f, 0.38f, 0.38f, 1) : new Color(0.38f, 0.38f, 0.38f, 0.5f);
        }
    }

    /*public void NextLevel()
    {
        string level = SceneManager.GetActiveScene().name;
        int levelToLoad = int.Parse(level.Remove(0, 5)) + 1;
        SceneManager.LoadScene("Level" + levelToLoad);
    }*/

    public void backToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
