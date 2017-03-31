using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class AG_SaveSelecterGeneretor : MonoBehaviour
{
    public GameObject levelButton;
    public Transform levelButtonParent;
    public bool editor;

    public void Start()
    {
        #if !UNITY_WEBGL
        if (!editor)
        {
            XmlManager xml = AG_SelectLevelManager.inst.xml;
            AG_SelectLevelManager.inst.savesList = FilesManager.FindFiles(xml.levelFolderName);
            AG_SelectLevelManager.inst.savesList = AG_SelectLevelManager.inst.savesList.OrderBy(xx => int.Parse((xx.Name.Remove(xx.Name.Length - 10, 10)).Remove(0, 5))).ToArray();

            //List<Button> listButton = new List<Button>();

            for (int i = 0; i < AG_SelectLevelManager.inst.savesList.Length; i++)
            {
                //Save save = xml.Load(xml.levelFolderName, AG_SelectLevelManager.inst.savesList[i].Name);
                //if (save.infos.saveInfos.developperLevel)
                {
                    string name = AG_SelectLevelManager.inst.savesList[i].Name;
                    string levelIndex = (name.Remove(name.Length - 10, 10)).Remove(0, 5);

                    GameObject obj = Instantiate(levelButton, levelButtonParent);
                    obj.GetComponentInChildren<Text>().text = levelIndex;

                    AG_SelectLevel selectLevel = obj.GetComponent<AG_SelectLevel>();
                    selectLevel.fileName = name;
                    selectLevel.folder = xml.levelFolderName;
                    selectLevel.levelIndex = int.Parse(levelIndex);

                    obj.GetComponent<Button>().interactable = i <= PlayerPrefs.GetInt("LevelDone");
                }
            }
        }
        else
        {
            XmlManager xml = AG_SelectLevelManager.inst.xml;
            FileInfo[] filesInfos = FilesManager.FindFiles(xml.editorFolderName);
            filesInfos = filesInfos.OrderBy(xx => int.Parse((xx.Name.Remove(xx.Name.Length - 10, 10)).Remove(0, 5))).ToArray();

            //List<Button> listButton = new List<Button>();

            for (int i = 0; i < filesInfos.Length; i++)
            {
                //Save save = xml.Load(xml.editorFolderName, filesInfos[i].Name);
                //if (save.infos.saveInfos.developperLevel)
                {
                    string name = filesInfos[i].Name;
                    string levelIndex = (name.Remove(name.Length - 10, 10)).Remove(0, 5);

                    GameObject obj = Instantiate(levelButton, levelButtonParent);
                    obj.GetComponentInChildren<Text>().text = levelIndex;

                    AG_SelectLevel selectLevel = obj.GetComponent<AG_SelectLevel>();
                    selectLevel.fileName = name;
                    selectLevel.folder = xml.editorFolderName;
                    selectLevel.levelIndex = int.Parse(levelIndex);
                }
            }
        }
        #endif
    }

    public void ToogleLoadEditor()
    {
        AG_SelectLevelManager.inst.loadInEditor = !AG_SelectLevelManager.inst.loadInEditor;
    }
    public void SetLoadEditor(bool value)
    {
        AG_SelectLevelManager.inst.loadInEditor = value;
    }
}
