using UnityEngine;
using UnityEngine.UI;

public class AG_Emitter : AG_ElementType_Color 
{       
    [HideInInspector] public RectTransform _rect;
    public Transform startLightPos;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        if (color != AG_Color.ColorName.none)
            colorImage.color = AG_Color.colorList[(int)color];
        else colorImage.color = Color.white;
    }

    public Save.EmitterInfos CollectInfos()
    {
        Save.EmitterInfos infos = new Save.EmitterInfos();
        infos.colorIndex = (int)color;
        infos.typeId = (int)objectType;
        infos.rect.position = _rect.position;
        infos.rect.angleZ = _rect.eulerAngles.z;

        return infos;
    }
}
