using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AG_Light_Mono : MonoBehaviour {

    public AG_Light ag_light;

    public void Init()
    {
        ag_light = new AG_Light(Color.red, new AG_Line(GetComponent<RectTransform>().localPosition, Vector2.right * 10, 1), GetComponent<RectTransform>(), GetComponent<Image>());
    }

    public void Init(Color color, AG_Line line)
    {
        ag_light = new AG_Light(color, line, GetComponent<RectTransform>(), GetComponent<Image>());
    }
}

public class AG_Light
{
    #region Var
    private Color _color;
    private AG_Line _line;
    private RectTransform _rect;
    private Image _img;
    #endregion
    #region Struct
    public Color color
    {
        get { return _color; }
        set { _color = value; }
    }
    public AG_Line line
    {
        get { return _line; }
        set { _line = value; }
    }
    #endregion
    #region Function
    public AG_Light(Color _color, AG_Line _line, RectTransform _rect, Image _img)
    {
        this._color = _color;
        this._line = _line;
        this._rect = _rect;
        this._img = _img;
        UpdateLightColor();
        UpdateLightPosition();
    }

    public void UpdateLightColor()
    {
        _img.color = _color;
    }

    public void UpdateLightPosition()
    {
        Vector2 dir = line.origin - line.end;
        float angleZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rect.position = line.origin;
        _rect.localEulerAngles = new Vector3(0, 0, 180 + angleZ);
        _rect.sizeDelta = new Vector2(Vector2.Distance(line.origin, line.end), line.width);        
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
    private float _width;
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
