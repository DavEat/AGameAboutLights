using UnityEngine;
using UnityEngine.UI;

public class AG_Emitter : AG_ElementType_Color 
{       
    [HideInInspector] public RectTransform _rect;
    public Transform startLightPos;
    public Save.EmitterInfos info;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        if (color != AG_Color.ColorName.none)
            colorImage.color = AG_Color.colorList[(int)color];
        else colorImage.color = Color.white;
    }
}
