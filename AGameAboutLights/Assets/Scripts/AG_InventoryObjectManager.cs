using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AG_InventoryObjectManager : MonoBehaviour {

    #region Var
    [SerializeField] private bool editorElem;
    /// <summary> number of item of that type </summary>
    private int _currentNumber;
    [SerializeField] private float _maxNumber = Mathf.Infinity;
    /// <summary> list of all already instance object and in the inventory </summary>
    private List<GameObject> _listObjects = new List<GameObject>(); 

    public Text text;
    [SerializeField] private GameObject prefab;
    /// <summary> parent of new obj </summary>
    [SerializeField] private Transform parent;

    private Transform _transform;
    #endregion
    #region Struct
    /// <summary> number of item of that type </summary>
    private int currentNumber
    {
        get { return _currentNumber; }
        set { _currentNumber = value; }
    }
    public float maxNumber
    {
        get { return _maxNumber; }
        set { _maxNumber = value; }
    }
    private List<GameObject> listObjects
    {
        get { return _listObjects; }
        set { _listObjects = value; }
    }
    #endregion
    #region Instance this Function
    private void Start()
    {
        _transform = transform;
        UpdateDisplayNumber();
    }
    #endregion
    #region Main Function
    public Transform OnSelect()
    {
        if (currentNumber < maxNumber)
        {
            if (_listObjects.Count < 1)
            {
                return InstanceNewObject();
            }
            else
            {
                currentNumber++;
                UpdateDisplayNumber();
                listObjects[0].SetActive(true);
                Transform t = listObjects[0].transform;
                t.transform.position = _transform.position;
                listObjects.RemoveAt(0);
                return t;
            }
        }
        else
        {
            Debug.Log("not enought obj, currently instant : " + currentNumber);
            return null;
        }
    }
    #endregion
    private Transform InstanceNewObject()
    {
        currentNumber++;
        UpdateDisplayNumber();
        return Instantiate(prefab, _transform.position, Quaternion.Euler(Vector3.zero), parent).transform;
    }

    public void AddIn(Transform obj)
    {
        GameObject o = obj.parent.gameObject;
        currentNumber--;
        UpdateDisplayNumber();
        listObjects.Add(o);
        o.SetActive(false);
    }
    #region Graphics Function
    public void UpdateDisplayNumber()
    {
        text.text = (maxNumber - currentNumber).ToString();
    }

    private void SetDisplayNumber(int value)
    {
        text.text = value.ToString();
    }
    #endregion

    public Save.InventoryElem CollectInfos()
    {
        Save.InventoryElem infos = new Save.InventoryElem();

        AG_ElementType elem = GetComponent<AG_ElementType>();
        if (elem.objectType == ObjectType.mirror && ((AG_Mirror)elem).mirrorType == AG_Mirror.MirrorType.Simple)
        {
            infos.typeId = (int)((AG_Mirror)elem).mirrorType;
            Debug.Log("mirror type : " + (int)((AG_Mirror)elem).mirrorType);
        }
        else infos.typeId = (int)elem.objectType;
        infos.quantity = (int)maxNumber;

        return infos;
    }
}