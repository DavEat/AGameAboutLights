
using UnityEngine;

public class AG_Utils
{
    #region Var
    #endregion
    #region MonoFunctions
    #endregion
    #region Functions
    public static float ClampAngle(float angle)
    {
        float a = angle;
        if (a > 0)
            while (a >= 360)
                a -= 360;
        else if (angle != 0)
            while (a < 0)
                a += 360;
        return a;
    }
    public static bool ScreenPointIsOnRects(RectTransform[] rects, Vector2 screenPoint)
    {
        bool r = false;
        for (int i = 0; i < rects.Length; i++)
            if (!r && RectTransformUtility.RectangleContainsScreenPoint(rects[i], screenPoint))
                r = true;

        return r;
    }
    #endregion
}
