using UnityEngine;

public enum ObjectType
{
    empthy,
    mirror,
    emitter,
    receiver,
    prisma,
    filter,
    wall
}

public enum ObjectInteractionType
{
    empthy,
    movable,
    inventory
}

public class AG_ElementType : MonoBehaviour {

    public bool tosave;
    public ObjectInteractionType objectInteractionType;
    public ObjectType objectType;
}