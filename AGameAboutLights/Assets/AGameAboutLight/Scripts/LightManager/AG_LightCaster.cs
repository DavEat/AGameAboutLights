﻿using UnityEngine;

public class AG_LightCaster : MonoBehaviour
{
	#region Var

	#endregion

	#region Struct

	#endregion

	#region MonoFunction
	void Awake ()
	{
		
	}

	void Start ()
	{
		
	}
	
	void Update () 
	{
		
	}
    #endregion

    #region Function

    /// <summary>Cast the light and return call the right function with the parameter setted</summary>
    /// <param name="_colorIndex">index of the color</param>
    /// <param name="_currentEmitterIndex">index of the source of the light</param>
    /// <param name="_origin">origin point of the previous light</param>
    /// <param name="_normal">normal of the previous light and the surface</param>
    public virtual LightHead[] Cast(int _colorIndex, float lightPower, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal, int _lightIndex)
    {
        Debug.Log("virtual");
        return new LightHead[0];
    } 
	#endregion
}
