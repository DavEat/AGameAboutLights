using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AG_Light_Mono : MonoBehaviour {

    public AG_Light ag_light;

    public void Init()
    {
        ag_light = new AG_Light(0, new AG_Line(GetComponent<RectTransform>().localPosition, Vector2.right * 10, 1), GetComponent<RectTransform>(), GetComponent<Image>());
    }

    public void Init(int colorIndex, AG_Line line)
    {
        ag_light = new AG_Light(colorIndex, line, GetComponent<RectTransform>(), GetComponent<Image>());
    }

    public void Init(int colorIndex, AG_Line line, bool notSetRendere)
    {
        ag_light = new AG_Light(colorIndex, line, GetComponent<RectTransform>(), GetComponent<Image>(), notSetRendere);
    }
}

public class AG_Light
{
    #region Var
    private int _colorIndex;
    private AG_Line _line;
    private RectTransform _rect;
    private Image _img;
    #endregion
    #region Struct
    public int colorIndex
    {
        get { return _colorIndex; }
        set { _colorIndex = value; }
    }
    public AG_Line line
    {
        get { return _line; }
        set { _line = value; }
    }
    #endregion
    #region Function
    public AG_Light(int _colorIndex, AG_Line _line, RectTransform _rect, Image _img)
    {
        this._colorIndex = _colorIndex;
        this._line = _line;
        this._rect = _rect;
        this._img = _img;
        UpdateLightColor();
        UpdateLightPosition();
    }

    public AG_Light(int _colorIndex, AG_Line _line, RectTransform _rect, Image _img, bool notSetRendere)
    {
        this._colorIndex = _colorIndex;
        this._line = _line;
        this._rect = _rect;
        this._img = _img;
        if (notSetRendere)
            UpdateLightPosition();
        else UpdateLightLenghtZero();
        UpdateLightColor();        
    }

    public void UpdateLightColor()
    {
        _img.color = AG_Color.colorList[_colorIndex];
    }

    public AG_Line GetLightValue()
    {
        AG_Line _line = new AG_Line(line.origin, line.end, line.width);
        Vector2 dir = line.origin - line.end;
        _line.angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _line.distance = Vector2.Distance(line.origin, line.end);

        return _line;
    }

    public void UpdateLightPosition()
    {
        Vector2 dir = line.origin - line.end;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rect.position = line.origin;
        _rect.localEulerAngles = new Vector3(0, 0, 180 + angleZ);
        _rect.sizeDelta = new Vector2(Vector2.Distance(line.origin, line.end), line.width);        
    }

    public void UpdateLightLenghtZero()
    {
        Vector2 dir = line.origin - line.end;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rect.position = line.origin;
        _rect.localEulerAngles = new Vector3(0, 0, 180 + angleZ);
        _rect.sizeDelta = new Vector2(0, line.width);
    }

    public void UpdateLightPositionRaycast()
    {
        RaycastHit2D hit = GetRaycast();
        if (hit.collider != null)
        {
            line.origin = _rect.position;
            line.end = hit.point;
            //Debug.DrawRay(_rect.position, Vector2.right * 100, Color.blue, 100);
            UpdateLightPosition();
        }
        
    }

    public RaycastHit2D GetRaycast()
    {
        return Physics2D.Raycast(_rect.position, Quaternion.Euler(0,0,_rect.rotation.z - 90) * Vector2.up, Mathf.Infinity);
    }
    #endregion
}

public class AG_Line
{
    #region Var
    private Vector2 _origin, _end;
    private float _width, _distance, _angle;
    #endregion
    #region Struct
    public Vector2 origin
    {
        get { return _origin; }
        set { _origin = value; }
    }
    public Vector2 end
    {
        get { return _end; }
        set { _end = value; }
    }
    public float width
    {
        get { return _width; }
        set { _width = value; }
    }
    public float distance
    {
        get { return _distance; }
        set { _distance = value; }
    }
    public float angle
    {
        get { return _angle; }
        set { _angle = value; }
    }
    #endregion
    #region Function
    public AG_Line(Vector2 _origin, Vector2 _end, float _width)
    {
        this._origin = _origin;
        this._end = _end;
        this._width = _width;
    }
    #endregion
}
