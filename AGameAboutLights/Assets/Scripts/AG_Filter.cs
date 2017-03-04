using UnityEngine;
using UnityEngine.UI;

public class AG_Filter : AG_ElementType_Color
{
    [HideInInspector] public RectTransform _rect;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        if (color != AG_Color.ColorName.none)
            colorImage.color = AG_Color.colorList[(int)color];
        else colorImage.color = Color.white;
    }

    public Save.FiltersInfos CollectInfos()
    {
        Save.FiltersInfos infos = new Save.FiltersInfos();
        infos.colorIndex = (int)color;
        infos.typeId = (int)objectType;
        infos.rect.position = _rect.position;
        infos.rect.angleZ = _rect.eulerAngles.z;

        return infos;
    }
}
