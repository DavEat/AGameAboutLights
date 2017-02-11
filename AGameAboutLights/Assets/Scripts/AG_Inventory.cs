using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Inventory : MonoBehaviour {

    [SerializeField]
    private List<Transform> _listPoints;
    [SerializeField] private UnityEngine.UI.ScrollRect _scroll;
    /// <summary>order by the enum ObjectType</summary>
    [SerializeField] private AG_InventoryObjectManager[] _lisObjects;

    public Transform inventoryLimite;

    public List<Transform> listPoints
    {
        get { return _listPoints; }
        private set { _listPoints = value; }
    }

    ///<summary>Scrool rect enable = value </summary>
    public void SetScroll(bool value)
    {
        if (_scroll != null)
        if (_scroll.enabled != value)
            _scroll.enabled = value;
    }

    public void AddToInventory(Transform obj)
    {
        AG_ElementType elem = obj.GetComponent<AG_ElementType>();

        switch (elem.objectType)
        {
            case ObjectType.prisma:
                _lisObjects[(int)elem.objectType].AddIn(obj);
                break;
            case ObjectType.mirror:
                _lisObjects[(int)elem.objectType].AddIn(obj);
                break;
            default: Debug.Log("out of switch");
                break;
        }
    }
}
