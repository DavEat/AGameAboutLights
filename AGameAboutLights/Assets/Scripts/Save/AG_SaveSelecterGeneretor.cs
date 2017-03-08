using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class AG_SaveSelecterGeneretor : MonoBehaviour
{
    public GameObject levelButton;
    public Transform levelButtonParent;
    public void Start()
    {
        XmlManager xml = AG_SelectLevelManager.inst.xml;
        FileInfo[] filesInfos = FilesManager.FindFiles(xml.levelFolderName);
        filesInfos = filesInfos.OrderByDescending(xx => xx.Name).ToArray();

        List<Button> listButton = new List<Button>();

        for (int i  =0; i < filesInfos.Length; i++)
        {
            Save save = xml.Load(xml.levelFolderName, filesInfos[i].Name);           
            //if (save.infos.saveInfos.developperLevel)
            {
                string name = filesInfos[i].Name;
                string levelIndex = (name.Remove(name.Length - 10, 10)).Remove(0, 5);

                GameObject obj = Instantiate(levelButton, levelButtonParent);
                obj.GetComponentInChildren<UnityEngine.UI.Text>().text = levelIndex;

                AG_SelectLevel selectLevel = obj.GetComponent<AG_SelectLevel>();
                selectLevel.fileName = name;
                selectLevel.folder = xml.levelFolderName;
                selectLevel.levelIndex = int.Parse(levelIndex);

                obj.GetComponent<Button>().interactable = i <= PlayerPrefs.GetInt("LevelDone");
            }
        }
    }
}
