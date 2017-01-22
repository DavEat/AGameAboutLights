using UnityEngine;
using UnityEngine.UI;

public class AG_Filter : AG_ElementType
{
    public int colorIndex;
    public Image filterColorImage;

    void Start()
    {
        if (colorIndex != -1)
            filterColorImage.color = AG_Color.colorList[colorIndex];
        else filterColorImage.color = Color.white;
    }
}
