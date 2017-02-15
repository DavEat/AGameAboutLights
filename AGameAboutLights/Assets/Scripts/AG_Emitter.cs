using UnityEngine;
using UnityEngine.UI;

public class AG_Emitter : AG_ElementType 
{
    public AG_Color.ColorName color;
    public Image receiverColorImage;
    [HideInInspector] public RectTransform _rect;
    public Save.EmitterInfos info;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        if (color != AG_Color.ColorName.none)
            receiverColorImage.color = AG_Color.colorList[(int)color];
        else receiverColorImage.color = Color.white;
    }
}
