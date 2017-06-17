using UnityEngine;

public class AG_FilterLight : AG_LightCaster
{
    #region Var
    private Transform _transform;
    private AG_ElementType _elem;
	#endregion

	#region Struct

	#endregion

	#region MonoFunction
	void Awake ()
	{
        _transform = transform;
        _elem = GetComponent<AG_ElementType>();
    }
    #endregion

    #region Function
    public override LightHead[] Cast(int _colorIndex, float lightPower, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal, int _lightIndex)
    {
        //Debug.Log("filter");
        if (_colorIndex == (int)((AG_Filter)_elem).color)
        {            
            Vector2 direction = -(_origin - _hitPoint).normalized;
            Vector2 origin = _hitPoint;

            //AG_LightsManagementNew.inst.AddLightHead(!AG_LightsManagementNew.inst.firstListLightHead, new LightHead(_colorIndex, origin, direction, _transform, new int[] {_lightIndex + 1}, _lightIndex));
            return new LightHead[] { new LightHead(_colorIndex, lightPower, origin, direction, _transform) };
        }
        return new LightHead[0];
    }
    #endregion
}
