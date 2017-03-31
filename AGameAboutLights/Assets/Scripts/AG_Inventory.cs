using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Inventory : MonoBehaviour {

    [SerializeField] private UnityEngine.UI.ScrollRect _scroll;
    /// <summary>order by the enum ObjectType</summary>
    public AG_InventoryObjectManager[] _listObjects;

    public Transform inventoryLimite;

    ///<summary>Scrool rect enable = value </summary>
    public void SetScroll(bool value)
    {
        if (_scroll != null)
        if (_scroll.enabled != value)
            _scroll.enabled = value;
    }

    public void AddToInventory(Transform obj)
    {
        AG_ElementType elem = obj.parent.GetComponent<AG_ElementType>();
        _listObjects[(int)elem.objectType].AddIn(obj);
    }
}
