using UnityEngine;
using UnityEngine.UI;

public class AG_Wall : AG_EditorElement
{
    private BoxCollider2D _boxCollider;

    public override void Init()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        if (objectInteractionType != ObjectInteractionType.movable)
            _rect = GetComponent<RectTransform>();
        else _rect = transform.parent.GetComponent<RectTransform>();        
    }

    public void RestCollider()
    {
        if (_boxCollider == null || _rect == null)
        {
            Init();
            if (_boxCollider == null || _rect == null)
            {
                Debug.Log("ERROR");
                return;
            }
        }

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
        infos.rect.position = new Vector2(_rect.position.x / Screen.width, _rect.position.y / Screen.height);
        infos.rect.angleZ = _rect.eulerAngles.z;
        infos.rect.deltaSize = new Vector2(_rect.sizeDelta.x / Screen.width, _rect.sizeDelta.y / Screen.height);

        return infos;
    }
}
