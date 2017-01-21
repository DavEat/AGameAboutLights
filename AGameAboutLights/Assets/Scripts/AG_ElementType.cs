using UnityEngine;

public enum ObjectType
{
    wall,
    mirror,
    emitter,
    receiver,
    prism
}

public enum ObjectInteractionType
{
    empthy,
    movable
}

public class AG_ElementType : MonoBehaviour {

    public ObjectType objectType;
    public ObjectInteractionType objectInteractionType;
}
