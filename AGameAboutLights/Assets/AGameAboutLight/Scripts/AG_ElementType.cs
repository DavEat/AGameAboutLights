using UnityEngine;

public enum ObjectType
{
    empthy,
    mirrorDouble,
    emitter,
    receiver,
    prisma,
    filter,
    wall,
    mirrorSimple,
    repeter,
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