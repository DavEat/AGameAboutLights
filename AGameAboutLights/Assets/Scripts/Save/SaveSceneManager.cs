using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveSceneManager : AG_Singleton<SaveSceneManager>
{
    #region Var
    public AG_Inventory playerInventory;
    public Transform staticObjectContener, movableObjectContener;

    public GameObject[] elementsPrefab;
    private XmlManager _xml;
    #endregion

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

    public void Save()
    {
        _xml.Save();
    }

    public void Load()
    {
        Save save = _xml.Load();

        foreach (Save.EmitterInfos emitter in save.infos.levelInfos.emitters)
        {
            GameObject obj = Instantiate(elementsPrefab[emitter.typeId], emitter.rect.position, Quaternion.Euler(new Vector3(0, 0, emitter.rect.angleZ)), staticObjectContener);
            obj.GetComponent<AG_Emitter>().color = (AG_Color.ColorName)emitter.colorIndex;
        }
        foreach (Save.ReceiverInfos receiver in save.infos.levelInfos.receivers)
        {
            GameObject obj = Instantiate(elementsPrefab[receiver.typeId], receiver.rect.position, Quaternion.Euler(new Vector3(0, 0, receiver.rect.angleZ)), staticObjectContener);
            obj.GetComponent<AG_Receiver>().color = (AG_Color.ColorName)receiver.colorIndex;
        }
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

    private void OnLevelWasLoaded()
    {
        Debug.Log("mager");
    }

    public bool save;
    public bool load;
    void Update()
    {
        if (_xml == null)
        {
            _xml = new XmlManager();
        }
            

        if (save)
        {
            save = false;
            Save();
        }
        else if (load)
        {
            load = false;
            Load();
        }
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