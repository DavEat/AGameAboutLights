using UnityEngine;

public enum ObjectType
{
    wall,
    mirror,
    emitter,
    receiver,
    prisma,
    filter
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
