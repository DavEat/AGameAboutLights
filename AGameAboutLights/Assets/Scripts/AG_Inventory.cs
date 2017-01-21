using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Inventory : MonoBehaviour {

    [SerializeField]
    private List<Transform> _listPoints;

    public List<Transform> listPoints
    {
        get { return _listPoints; }
        private set { _listPoints = value; }
    }
}
