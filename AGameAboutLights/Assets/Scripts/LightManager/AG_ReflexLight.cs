using UnityEngine;

public class AG_ReflexLight : AG_LightCaster
{
    #region Var
    protected Transform _transform;
	#endregion
	#region MonoFunction
	void Awake ()
	{
        _transform = transform;
    }
    #endregion

    #region Function
    public override LightHead[] Cast(int _colorIndex, float lightPower, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal, int _lightIndex)
    {
        //Debug.Log("Cast Reflex");
        Vector2 direction = -Vector2.Reflect((_origin - _hitPoint).normalized, _normal);
        Vector2 origin = _hitPoint;

        //AG_LightsManagementNew.inst.AddLightHead(!AG_LightsManagementNew.inst.firstListLightHead, new LightHead(_colorIndex, origin, direction, _transform, new int[] { _lightIndex + 1 }, _lightIndex));
        return new LightHead[] { new LightHead(_colorIndex, lightPower,origin, direction, _transform) };
    }
    #endregion
}
