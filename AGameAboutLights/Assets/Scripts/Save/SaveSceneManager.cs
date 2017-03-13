using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveSceneManager : AG_Singleton<SaveSceneManager>
{
    #region Var
    public AG_Inventory playerInventory;
    public Transform staticObjectContener, movableObjectContener;
    public UnityEngine.UI.Text saveName;
    public GameObject[] elementsPrefab;
    #endregion

    #region Functions
    /// <summary>Collect all object in the scene that they need to be save</summary>
    /// <returns>Return all AG_ElementType that htey need to be save</returns>
    public static AG_ElementType[] CollectSavable()
    {
        AG_ElementType[] array = FindObjectsOfType<AG_ElementType>();
        List<AG_ElementType> list = new List<AG_ElementType>();
        foreach (AG_ElementType elem in array)
            if (elem.tosave)
                list.Add(elem);

        return list.ToArray();
    }

    /// <summary>Get and sort object to save in the scene</summary>
    /// <returns>Return info to save for all object in scene that need to be save</returns>
    public static Save.Infos SortObjToSave()
    {
        Save.Infos infos = new Save.Infos();
        List<Save.EmitterInfos> listEmitter = new List<Save.EmitterInfos>();
        List<Save.ReceiverInfos> listReceiver = new List<Save.ReceiverInfos>();
        List<Save.FiltersInfos> listFilter = new List<Save.FiltersInfos>();
        List<Save.WallsInfos> listWall = new List<Save.WallsInfos>();
        List<Save.InventoryElem> inventory = new List<Save.InventoryElem>();

        AG_ElementType[] elements = CollectSavable();

        foreach (AG_ElementType elem in elements)
        {
            switch (elem.objectType)
            {
                case ObjectType.emitter:
                    listEmitter.Add(((AG_Emitter)elem).CollectInfos());
                    break;
                case ObjectType.receiver:
                    listReceiver.Add(((AG_Receiver)elem).CollectInfos());
                    break;
                case ObjectType.filter:
                    listFilter.Add(((AG_Filter)elem).CollectInfos());
                    break;
                case ObjectType.wall:
                    listWall.Add(((AG_Wall)elem).CollectInfos());
                    break;
                default:
                    if (elem.objectInteractionType == ObjectInteractionType.inventory)
                    {
                        AG_InventoryObjectManager inventoryObjectManager = elem.GetComponent<AG_InventoryObjectManager>();
                        if (inventoryObjectManager.maxNumber > 0)
                            inventory.Add(inventoryObjectManager.CollectInfos());
                    }
                    else Debug.Log("Uncorrect enter in saves");
                    break;
            }
        }

        infos.levelInfos.emitters = listEmitter.ToArray();
        infos.levelInfos.receivers = listReceiver.ToArray();
        infos.levelInfos.filters = listFilter.ToArray();
        infos.levelInfos.walls = listWall.ToArray();
        infos.levelInfos.inventory.listElements = inventory.ToArray();

        return infos;
    }

    public void Save(bool editor)
    {
        if (saveName.text.Length > 0)
        {
            if (editor)
                AG_SelectLevelManager.inst.xml.Save(AG_SelectLevelManager.inst.xml.levelFolderName, saveName.text + AG_SelectLevelManager.inst.xml.fileExtention);
            else AG_SelectLevelManager.inst.xml.Save(AG_SelectLevelManager.inst.xml.editorFolderName, saveName.text + AG_SelectLevelManager.inst.xml.fileExtention);
        }
    }

    public void Load(string _folder, string _fileName)
    {
        Save save = AG_SelectLevelManager.inst.xml.Load(_folder, _fileName);

        List<AG_Emitter> listEmitter = new List<AG_Emitter>();
        foreach (Save.EmitterInfos emitter in save.infos.levelInfos.emitters)
        {
            GameObject obj = Instantiate(elementsPrefab[emitter.typeId], emitter.rect.position, Quaternion.Euler(new Vector3(0, 0, emitter.rect.angleZ)), staticObjectContener);
            AG_Emitter _emitter = obj.GetComponent<AG_Emitter>();
            _emitter.color = (AG_Color.ColorName)emitter.colorIndex;

            listEmitter.Add(_emitter);
        }
        AG_LightsManagementNew.inst.listEmitter = listEmitter.ToArray();
        List<AG_Receiver> listReceiver = new List<AG_Receiver>();
        foreach (Save.ReceiverInfos receiver in save.infos.levelInfos.receivers)
        {
            GameObject obj = Instantiate(elementsPrefab[receiver.typeId], receiver.rect.position, Quaternion.Euler(new Vector3(0, 0, receiver.rect.angleZ)), staticObjectContener);
            AG_Receiver _receiver = obj.GetComponent<AG_Receiver>();
            _receiver.color = (AG_Color.ColorName)receiver.colorIndex;

            listReceiver.Add(_receiver);
        }
        AG_LightsManagementNew.inst.listReceiver = listReceiver.ToArray();
        foreach (Save.FiltersInfos filter in save.infos.levelInfos.filters)
        {
            GameObject obj = Instantiate(elementsPrefab[filter.typeId], filter.rect.position, Quaternion.Euler(new Vector3(0, 0, filter.rect.angleZ)), staticObjectContener);
            obj.GetComponent<AG_Filter>().color = (AG_Color.ColorName)filter.colorIndex;
        }
        foreach (Save.WallsInfos wall in save.infos.levelInfos.walls)
        {
            GameObject obj = Instantiate(elementsPrefab[wall.typeId], wall.rect.position, Quaternion.Euler(new Vector3(0, 0, wall.rect.angleZ)), staticObjectContener);
            obj.GetComponent<RectTransform>().sizeDelta = wall.rect.deltaSize;
        }
        foreach (AG_InventoryObjectManager obj in playerInventory._listObjects)
        {
            if (obj != null)
            {
                AG_ElementType objectType = obj.GetComponent<AG_ElementType>();
                if (obj.gameObject.activeSelf)
                    obj.gameObject.SetActive(false);
                foreach (Save.InventoryElem elem in save.infos.levelInfos.inventory.listElements)
                {
                    if ((int)objectType.objectType == elem.typeId)
                    {
                        obj.maxNumber = elem.quantity;
                        obj.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    private string CreateFileName()
    {
        DateTime date = DateTime.Now;
        int chapter = 15, room = 12;

        string month;
        if (date.Month < 10)
            month = "0" + date.Month.ToString();
        else month = date.Month.ToString();

        string day;
        if (date.Day < 10)
            day = "0" + date.Day.ToString();
        else day = date.Day.ToString();

        string hour;
        if (date.Hour < 10)
            hour = "0" + date.Hour.ToString();
        else hour = date.Hour.ToString();

        string minute;
        if (date.Minute < 10)
            minute = "0" + date.Minute.ToString();
        else minute = date.Minute.ToString();

        return (date.Year.ToString() + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + chapter.ToString() + "-" + room.ToString() + AG_SelectLevelManager.inst.xml);
    }
    #endregion

    private void OnLevelWasLoaded()
    {
        Load(AG_SelectLevelManager.inst.folder, AG_SelectLevelManager.inst.fileName);

        //Debug.Log("Level was load");
    }
}

// Save is our custom class that holds our defined objects we want to store in XML format 
public class Save
{
    public Infos infos;

    public Save() { }

    // Anything we want to store in the XML file, we define it here
    public struct Infos
    {
        public int sceneID;
        public string sceneName;
        public SaveInfo saveInfos;
        public LevelInfos levelInfos;
    }

    public struct SaveInfo
    {
        public int difficulty;
        public bool developperLevel;
        public DateTime date;
    }

    public struct InventoryInfos
    {
        public InventoryElem[] listElements;
    }

    public struct InventoryElem
    {
        public int typeId, quantity;
    }

    public struct LevelInfos
    {
        public InventoryInfos inventory;
        public EmitterInfos[] emitters;
        public ReceiverInfos[] receivers;
        public FiltersInfos[] filters;
        public WallsInfos[] walls;
    }

    public struct EmitterInfos
    {
        public int typeId, colorIndex;
        public RectMinTransformInfos rect;
    }

    public struct ReceiverInfos
    {
        public int typeId, colorIndex;
        public RectMinTransformInfos rect;
    }

    public struct WallsInfos
    {
        public int typeId;
        public RectTransformInfos rect;
    }

    public struct FiltersInfos
    {
        public int typeId, colorIndex;
        public RectMinTransformInfos rect;
    }

    public struct RectTransformInfos
    {
        public Vector2 position, deltaSize;
        public float angleZ;
    }

    public struct RectMinTransformInfos
    {
        public Vector2 position;
        public float angleZ;
    }

    public struct RectTransformCompleteInfos
    {
        public Vector2 position, scale, deltaSize;
        public float angleZ;
    }
}