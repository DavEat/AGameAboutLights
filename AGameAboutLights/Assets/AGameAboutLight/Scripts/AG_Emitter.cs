using UnityEngine;

public class AG_Emitter : AG_ElementType_Color 
{       
    public Transform startLightPos;

    public override void Init()
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
        infos.rect.position = new Vector2(_rect.position.x / Screen.width, _rect.position.y / Screen.height);
        infos.rect.angleZ = _rect.eulerAngles.z;

        return infos;
    }
}
