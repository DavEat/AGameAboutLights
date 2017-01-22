using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {


    private static DontDestroyOnLoad _instance = null;
    public static DontDestroyOnLoad instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
