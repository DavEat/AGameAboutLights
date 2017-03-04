using UnityEngine;
using UnityEngine.UI;

public class AG_Wall : AG_EditorElement
{
    private BoxCollider2D _boxCollider;
    private RectTransform _rect;

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        if (objectInteractionType != ObjectInteractionType.movable)
            _rect = GetComponent<RectTransform>();
        else _rect = transform.parent.GetComponent<RectTransform>();
        RestCollider();
    }

    public void RestCollider()
    {        
        _boxCollider.size = _rect.sizeDelta;

        if (objectInteractionType != ObjectInteractionType.movable)
            _boxCollider.offset = _rect.sizeDelta * 0.5f;
        else _boxCollider.offset = Vector2.zero;
    }

    public override void OnSelected()
    {
        //editorM.scaleTool
    }

    public void DoScale(Vector2 pos, Vector2 scale)
    {
        _rect.position = pos;
        _rect.sizeDelta = scale;
    }

    public Save.WallsInfos CollectInfos()
    {
        Save.WallsInfos infos = new Save.WallsInfos();
        infos.typeId = (int)objectType;
        infos.rect.position = _rect.position;
        infos.rect.angleZ = _rect.eulerAngles.z;
        infos.rect.deltaSize = _rect.sizeDelta;

        return infos;
    }
}
