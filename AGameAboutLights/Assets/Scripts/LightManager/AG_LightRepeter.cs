using UnityEngine;

public class AG_LightRepeter : AG_LightCaster
{
    #region Var
    protected Transform _transform;
    #endregion
    #region MonoFunction
    void Awake()
    {
        _transform = transform;
    }
    #endregion
    #region Function
    public override LightHead[] Cast(int colorIndex, float lightPower, Vector2 origin, Vector2 hitPoint, Vector2 normal, int lightIndex)
    {
        return new LightHead[] { new LightHead(colorIndex, AG_LightsManagementNewNew.inst.maxLightLenght, hitPoint, (hitPoint - origin).normalized, _transform) }; ;
    }
    #endregion
}