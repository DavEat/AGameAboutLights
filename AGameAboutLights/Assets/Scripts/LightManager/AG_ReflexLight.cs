using UnityEngine;

public class AG_ReflexLight : AG_LightCaster
{
    #region Var
    private Transform _transform;
	#endregion

	#region Struct

	#endregion

	#region MonoFunction
	void Awake ()
	{
        _transform = transform;
    }
    #endregion

    #region Function
    public override void Cast(int _currentLightHeadIndex, int _colorIndex, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal, float _previousAnimDuration, int _lightIndex)
    {
        //Debug.Log("Cast Reflex");
        Vector2 direction = -Vector2.Reflect((_origin - _hitPoint).normalized, _normal);
        Vector2 origin = _hitPoint;

        //AG_LightsManagementNew.inst.AddRaycastIgnore(_transform.gameObject);
        AG_LightsManagementNew.inst.AddLightHead(AG_LightsManagementNew.inst.firstListLightHead, new LightHead(_colorIndex, origin, direction, _transform, new int[] { _lightIndex + 1 }));
        //AG_LightsManagementNew.inst.SetListLightHeadAtIndex(_currentLightHeadIndex);
        //AG_LightsManagementNew.inst.AddLight(_colorIndex, origin, direction);
    }
    #endregion
}
