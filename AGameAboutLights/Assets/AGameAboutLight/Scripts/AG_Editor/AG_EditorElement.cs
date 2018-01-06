using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AG_EditorElement : AG_ElementType {

    [HideInInspector] public RectTransform _rect;
    public virtual void OnSelected() { }
    public virtual void Init() { Debug.Log("manger"); }
}
