using UnityEngine;
using UnityEngine.UI;

public class AG_Receiver : AG_ElementType 
{
    public int colorIndex;
    public Image receiverColorImage;

    void Start()
    {
        if (colorIndex != -1)
            receiverColorImage.color = AG_Color.colorList[colorIndex];
        else receiverColorImage.color = Color.white;
    }
}
