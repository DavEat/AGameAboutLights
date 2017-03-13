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
        SceneManager.LoadScene(1);
    }

    public void SetFileToLoad(string _folder, string _fileName, int _levelIndex, bool _newEditor)
    {
        folder = _folder;
        fileName = _fileName;
        levelIndex = _levelIndex;

        if (loadInEditor || _newEditor)
            SceneManager.LoadScene(3);
        else SceneManager.LoadScene(2);

        loadInEditor = false;
    }

    [HideInInspector] public bool loadInEditor;
}
