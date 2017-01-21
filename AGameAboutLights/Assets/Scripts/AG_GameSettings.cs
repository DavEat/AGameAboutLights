using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_GameSettings : MonoBehaviour {
    
    private static bool _displayGrid; // display grid every time
    public static bool displayGrid
    {
        get { return _displayGrid; }
        set { _displayGrid = value; }
    }
}
