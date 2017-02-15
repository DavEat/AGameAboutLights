﻿using UnityEngine;
using UnityEngine.UI;

public class AG_Receiver : AG_ElementType 
{
    public AG_Color.ColorName color;
    public Image receiverColorImage;
    private bool _alimented;
    [HideInInspector] public RectTransform _rect;

    public bool alimented
    {
        get { return _alimented; }
        set { _alimented = value; }
    }

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        if (color != AG_Color.ColorName.none)
            receiverColorImage.color = AG_Color.colorList[(int)color];
        else receiverColorImage.color = Color.white;
    }
}
