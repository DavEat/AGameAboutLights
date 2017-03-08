using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AG_SelectLevelManager : AG_Singleton<AG_SelectLevelManager>
{
    private bool _developper = true;
    public bool developper { get { return _developper; } private set { _developper = value; } }

    [HideInInspector] public string folder, fileName;
    [HideInInspector] public int levelIndex;

    private XmlManager _xml;
    public XmlManager xml { get { return _xml; } private set { _xml = value; } }

    public void Start()
    {
        _xml = new XmlManager();
    }

    public void SetFileToLoad(string _folder, string _fileName, int _levelIndex)
    {
        folder = _folder;
        fileName = _fileName;
        levelIndex = _levelIndex;

        SceneManager.LoadScene(1);
    }

    /*/// <summary>set the save to load</summary>
    /// <param name="_name">the name of the save to load</param>
    public void SetSelectedSave(string _name)
    {
        xml.fileName = _name;
    }

    /// <summary>select all save files in al folder</summary>
    /// <param name="_folderPath">in the folder path</param>
    public void SelectAllFiles(string _folderPath)
    {

    } */
}
