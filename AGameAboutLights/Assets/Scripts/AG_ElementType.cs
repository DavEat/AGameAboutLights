using UnityEngine;

public enum ObjectType
{
    wall,
    mirror,
    emitter,
    receiver,
    prisma
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

public class PrismaFace : AG_ElementType
{
    public Transform face1, face2;
}
