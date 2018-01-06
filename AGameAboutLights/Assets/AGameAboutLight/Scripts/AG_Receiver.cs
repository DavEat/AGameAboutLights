using UnityEngine;
using UnityEngine.UI;

public class AG_Receiver : AG_ElementType_Color
{
    private bool _alimented;

    public bool alimented
    {
        get { return _alimented; }
        set { _alimented = value; }
    }

    public override void Init()
    {
        _rect = GetComponent<RectTransform>();
        if (color != AG_Color.ColorName.none)
            colorImage.color = AG_Color.colorList[(int)color];
        else colorImage.color = Color.white;
    }

    public Save.ReceiverInfos CollectInfos()
    {
        Save.ReceiverInfos infos = new Save.ReceiverInfos();
        infos.colorIndex = (int)color;
        infos.typeId = (int)objectType;
        infos.rect.position = new Vector2(_rect.position.x / Screen.width, _rect.position.y / Screen.height);
        infos.rect.angleZ = _rect.eulerAngles.z;

        return infos;
    }
}
