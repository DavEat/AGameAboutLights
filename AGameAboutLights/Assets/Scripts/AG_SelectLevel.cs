using UnityEngine;
//using UnityEngine.SceneManagement;

public class AG_SelectLevel : MonoBehaviour {

    [HideInInspector] public string fileName, folder;
    [HideInInspector] public int levelIndex;

    public void LoadLevel()
    {
        AG_CallSound.inst.PlayButtonPress();
        AG_SelectLevelManager.inst.SetFileToLoad(folder, fileName, levelIndex);

        //int level = int.Parse(this.GetComponentInChildren<UnityEngine.UI.Text>().text);
        //Debug.Log(level);
        //SceneManager.LoadScene(1);
    } 
}
