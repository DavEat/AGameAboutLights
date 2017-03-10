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
    public override void Cast(int _currentLightHeadIndex, int _colorIndex, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal, float _previousAnimDuration, int _lightIndex)
    {
        //Debug.Log("prisma");
        int[] colorsIndexs = PrismaColorManager(_colorIndex);        

        AG_LightsManagementNew.inst.AddLightHead(AG_LightsManagementNew.inst.firstListLightHead, InitLightHead(((AG_PrismaFace)_elem).face1, _hitPoint, colorsIndexs[0], _previousAnimDuration, _lightIndex));
        AG_LightsManagementNew.inst.AddLightHead(AG_LightsManagementNew.inst.firstListLightHead, InitLightHead(((AG_PrismaFace)_elem).face2, _hitPoint, colorsIndexs[1], _previousAnimDuration, _lightIndex));
        //AG_LightsManagementNew.inst.SetListLightHeadAtIndex(_currentLightHeadIndex);
        //AG_LightsManagementNew.inst.AddLight(_colorIndex, origin, direction);
    }

    private LightHead InitLightHead(Transform _face, Vector2 _hitPoint, int _colorIndex, float _previousAnimDuration, int _lightIndex)
    {
        Vector2 offset = CalculOffset(_transform.position, _hitPoint, _face);
        Vector2 origin = (Vector2)_face.position + offset;
        float angle = _face.eulerAngles.z + 90;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

        return new LightHead(_colorIndex, origin, direction, _face, new int[] { _lightIndex + 1 });
    }
    private Vector2 CalculOffset(Vector2 _hitObjPos, Vector2 _hitPoint, Transform _targetObj)
    {
        float distance = Vector2.Distance(_hitPoint, _hitObjPos);
        Vector2 offset = _targetObj.rotation * Vector2.up * distance;
        return -offset;
    }
    private int[] PrismaColorManager(int colorIndex)
    {
        int[] arrayToReturn;
        if (colorIndex == 1)
            arrayToReturn = new int[] { 6, 2 };
        else if (colorIndex == 2)
            arrayToReturn = new int[] { 1, 2 };
        else if (colorIndex == 3)
            arrayToReturn = new int[] { 2, 4 };
        else if (colorIndex == 4)
            arrayToReturn = new int[] { 3, 5 };
        else if (colorIndex == 5)
            arrayToReturn = new int[] { 4, 6 };
        else if (colorIndex == 6)
            arrayToReturn = new int[] { 1, 5 };
        else arrayToReturn = null;

        return arrayToReturn;
    }
    #endregion
}
