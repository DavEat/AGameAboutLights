using UnityEngine;
using UnityEngine.UI;

public class AG_Receiver : AG_ElementType 
{
    public int colorIndex;
    public Image receiverColorImage;
    private bool _alimented;

    public bool alimented
    {
        get { return _alimented; }
        set { _alimented = value; }
    }

    void Start()
    {
        if (colorIndex != -1)
            receiverColorImage.color = AG_Color.colorList[colorIndex];
        else receiverColorImage.color = Color.white;
    }
}
