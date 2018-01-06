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

    private static bool _snap; // display grid every time
    public static bool snap
    {
        get { return _snap; }
        set { _snap = value; }
    }

    public void ToogleSnap() { _snap = !_snap; }
}
