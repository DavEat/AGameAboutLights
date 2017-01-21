using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AG_SelectLevel : MonoBehaviour {

    public void loadLevel()
    {
       int level = int.Parse(this.GetComponentInChildren<UnityEngine.UI.Text>().text);
       Debug.Log(level);
       SceneManager.LoadScene("Level" + level);
    } 
}
