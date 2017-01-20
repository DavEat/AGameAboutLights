using UnityEngine;
using UnityEngine.UI;

public class AG_Light_Mono : MonoBehaviour {

    private AG_Light _light;
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
        protected set { _color = value; }
    }
    public AG_Line line
    {
        get { return _line; }
        protected set { _line = value; }
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
        this._img.color = _color;
    }

    public void UpdateLightPosition()
    {
        Vector2 dir = line.origin - line.end;
        float angleZ = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        _rect.localPosition = line.origin;
        _rect.localEulerAngles = new Vector3(0, 0, -angleZ);
        _rect.sizeDelta = new Vector2(Vector2.Distance(line.origin, line.end), line.width);        
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
        protected set { _origin = value; }
    }
    public Vector2 end
    {
        get { return _end; }
        protected set { _end = value; }
    }
    public float width
    {
        get { return _width; }
        protected set { _width = value; }
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
