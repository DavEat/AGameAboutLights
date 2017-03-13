using UnityEngine;
//using UnityEngine.SceneManagement;

public class AG_SelectLevel : MonoBehaviour {

    public bool newEditor;
    [HideInInspector] public string fileName, folder;
    [HideInInspector] public int levelIndex;

    public void LoadLevel()
    {
        AG_CallSound.inst.PlayButtonPress();
        if (newEditor)
            AG_SelectLevelManager.inst.SetFileToLoad(folder, "$newLevel$", levelIndex, newEditor);
        else AG_SelectLevelManager.inst.SetFileToLoad(folder, fileName, levelIndex, newEditor);

        //int level = int.Parse(this.GetComponentInChildren<UnityEngine.UI.Text>().text);
        //Debug.Log(level);
        //SceneManager.LoadScene(1);
    } 
}
