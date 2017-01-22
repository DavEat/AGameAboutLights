using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_DiplayUnlockedLevel : MonoBehaviour {

	void Update () {
		for(int i = 1; i < PlayerPrefs.GetInt("LevelDone"); i++)
        {
            Transform bouton = GetComponent<Transform>().transform.Find("Level" + i);
            bouton.GetComponent<GameObject>().SetActive(true);
        }
	}
}
