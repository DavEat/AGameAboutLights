using UnityEngine;
using UnityEngine.UI;

public class AG_Filter : AG_ElementType_Color
{
    void Start()
    {
        if (color != AG_Color.ColorName.none)
            colorImage.color = AG_Color.colorList[(int)color];
        else colorImage.color = Color.white;
    }
}
