using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AG_InventoryObjectManager : MonoBehaviour {

    #region Var
    /// <summary> number of item of that type </summary>
    private int _currentNumber;
    [SerializeField] private int _maxNumber;
    /// <summary> list of all already instance object and in the inventory </summary>
    private List<GameObject> _listObjects = new List<GameObject>(); 

    [SerializeField] private Text _text;
    [SerializeField] private GameObject prefab;
    /// <summary> parent of new obj </summary>
    [SerializeField] private Transform parent;

    private Transform _transform;
    private AG_ElementType _elementType;
    #endregion
    #region Struct
    /// <summary> number of item of that type </summary>
    private int currentNumber
    {
        get { return _currentNumber; }
        set { _currentNumber = value; }
    }
    private int maxNumber
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
        _elementType = GetComponent<AG_ElementType>();
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
                listObjects[0].transform.position = _transform.position;
                return listObjects[0].transform;
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
    private void UpdateDisplayNumber()
    {
        _text.text = (maxNumber - currentNumber).ToString();
    }

    private void SetDisplayNumber(int value)
    {
        _text.text = value.ToString();
    }
    #endregion
}
