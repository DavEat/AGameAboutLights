using UnityEngine;

public enum ObjectType
{
    wall,
    mirror,
    emitter,
    receiver,
    prism
}

public class AG_ElementType : MonoBehaviour {

    public ObjectType objectType;
}
