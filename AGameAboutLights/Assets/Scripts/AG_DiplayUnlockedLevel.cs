using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_DiplayUnlockedLevel : MonoBehaviour {

    public List<UnityEngine.UI.Button> levelsButton;

	public void checkUnlockedLevel () {
        for (int i = 0; i < levelsButton.Count; i++)
        {
            if (i <= PlayerPrefs.GetInt("LevelDone"))
            { 
                levelsButton[i].interactable = (true);
            }
            else
            {
                levelsButton[i].interactable=(false);
                //levelsButton[i].GetComponentInChildren<UnityEngine.UI.Text>().color = new Color(200, 200, 200, 128);
            }
        }
	}
}
