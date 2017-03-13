using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Inventory : MonoBehaviour {

    [SerializeField]
    private List<Transform> _listPoints;
    [SerializeField] private UnityEngine.UI.ScrollRect _scroll;
    /// <summary>order by the enum ObjectType</summary>
    public AG_InventoryObjectManager[] _listObjects;

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

        if (elem.objectType == ObjectType.mirror)
        {
            if (((AG_Mirror)elem).mirrorType == AG_Mirror.MirrorType.Double)
                _listObjects[(int)elem.objectType].AddIn(obj);
            else _listObjects[(int)((AG_Mirror)elem).mirrorType].AddIn(obj);
        }
        else _listObjects[(int)elem.objectType].AddIn(obj);
    }
}
