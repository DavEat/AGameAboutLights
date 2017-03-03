using UnityEngine;

public class AG_LightPrisma : AG_LightCaster
{
    #region Var
    private Transform _transform;
    private AG_ElementType _elem;
    #endregion

    #region Struct

    #endregion

    #region MonoFunction
    void Awake()
    {
        _transform = transform;
        _elem = GetComponent<AG_ElementType>();
    }
    #endregion

    #region Function
    public override void Cast(int _currentLightHeadIndex, int _colorIndex, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal)
    {
        Debug.Log("prisma");
        if (_colorIndex == (int)((AG_Filter)_elem).color)
        {
            AG_LightsManagementNew.inst.RaycastIgnore(_transform);
            Vector2 direction = -(_origin - _hitPoint).normalized;
            Vector2 origin = _hitPoint;

            AG_LightsManagementNew.inst.AddLightHead(AG_LightsManagementNew.inst.firstListLightHead, new LightHead(_colorIndex, origin, direction));
            //AG_LightsManagementNew.inst.SetListLightHeadAtIndex(_currentLightHeadIndex);
            //AG_LightsManagementNew.inst.AddLight(_colorIndex, origin, direction);
        }
    }
    #endregion
}
