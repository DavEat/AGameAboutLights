using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_Settings : AG_Singleton<AG_Settings> {

    #region Var
    public int screenLenght = 1920;

    public float maxLightLenght = 1000;

    public bool customAngle = false;
    #endregion
    #region MonoFunctions
    private void Update()
    {
        if (Input.GetKey("a") && Input.GetKey("c"))
        {
            if (Input.GetKey("x"))
                customAngle = false;
            else customAngle = true;
        }
    }
    #endregion
    #region Functions
    #endregion
}
