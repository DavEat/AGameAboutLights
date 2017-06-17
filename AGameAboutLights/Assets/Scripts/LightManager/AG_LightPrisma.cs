using UnityEngine;

public class AG_LightPrisma : AG_LightCaster
{
    #region Var
    private Transform _transform;
    private AG_PrismaFace _elem;
    #endregion

    #region Struct

    #endregion

    #region MonoFunction
    void Awake()
    {
        _transform = transform;
        _elem = GetComponent<AG_PrismaFace>();
    }
    #endregion

    #region Function
    public override LightHead[] Cast(int _colorIndex, float lightPower, Vector2 _origin, Vector2 _hitPoint, Vector2 _normal, int _lightIndex)
    {
        //Debug.Log("prisma");
        int[] colorsIndexs = PrismaColorManager(_colorIndex);
        lightPower *= 0.5f;
        //AG_LightsManagementNew.inst.AddLightHead(!AG_LightsManagementNew.inst.firstListLightHead, InitLightHead(((AG_PrismaFace)_elem).face1, _hitPoint, colorsIndexs[0], _lightIndex));
        //AG_LightsManagementNew.inst.AddLightHead(!AG_LightsManagementNew.inst.firstListLightHead, InitLightHead(((AG_PrismaFace)_elem).face2, _hitPoint, colorsIndexs[1], _lightIndex + 1));
        return new LightHead[] { InitLightHead(_elem.face1, _hitPoint, colorsIndexs[0], _lightIndex, lightPower), InitLightHead(_elem.face2, _hitPoint, colorsIndexs[1], _lightIndex + 1, lightPower) };
    }

    private LightHead InitLightHead(Transform _face, Vector2 _hitPoint, int _colorIndex, int _lightIndex, float lightPower)
    {
        Vector2 offset = CalculOffset(_transform.position, _hitPoint, _face);
        Vector2 origin = (Vector2)_face.position + offset;
        float angle = _face.eulerAngles.z + 90;
        Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

        return new LightHead(_colorIndex, lightPower, origin, direction, _face);
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
            arrayToReturn = new int[] { 1, 3 };
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
