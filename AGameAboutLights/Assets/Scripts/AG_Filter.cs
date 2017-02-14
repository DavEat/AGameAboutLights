using UnityEngine;
using UnityEngine.UI;

public class AG_Filter : AG_ElementType
{
    public AG_Color.ColorName color;
    public Image filterColorImage;

    void Start()
    {
        if (color != AG_Color.ColorName.none)
            filterColorImage.color = AG_Color.colorList[(int)color];
        else filterColorImage.color = Color.white;
    }
}
