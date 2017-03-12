using UnityEngine;

public class AG_LightAbsorber : AG_LightCaster
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
    public override void Cast(int _colorIndex, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal, int _lightIndex)
    {
        //Debug.Log("wall");
        //AG_LightsManagementNew.inst.RemoveLightHeadAtIndex(AG_LightsManagementNew.inst.firstListLightHead , _currentEmitterIndex);
        //AG_LightsManagementNew.inst.SetListLightHeadAtIndex(_currentEmitterIndex);
    } 
	#endregion
}
