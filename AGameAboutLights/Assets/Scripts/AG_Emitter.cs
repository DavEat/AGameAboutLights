using UnityEngine;
using UnityEngine.UI;

public class AG_Emitter : AG_ElementType 
{
    public AG_Color.ColorName color;
    public Image emitterColorImage;
    [HideInInspector] public RectTransform _rect;
    public Save.EmitterInfos info;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        if (color != AG_Color.ColorName.none)
            emitterColorImage.color = AG_Color.colorList[(int)color];
        else emitterColorImage.color = Color.white;
    }
}
