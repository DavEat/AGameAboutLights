using UnityEngine;

public class AG_ScaleToolManager : MonoBehaviour {

    #region Var
    [SerializeField] private LayerMask layer;	
    [SerializeField] private Transform[] corners;
    private int selectedCornerIndex = -1, xCornerIndex, yCornerIndex;
    private GameObject _gameObject;
    private AG_EditorManager editorM;	
    #endregion

    private void Start()
    {
        _gameObject = gameObject;
        _gameObject.SetActive(false);
        editorM = FindObjectOfType<AG_EditorManager>();
    }

    public bool ActiveSelf()
    {
        return _gameObject.activeSelf;
    }

    public void InitScaleTool(RectTransform target)
    {
        _gameObject.SetActive(true);
        this.transform.eulerAngles = target.eulerAngles;

        corners[2].position = target.position - (target.rotation * (target.sizeDelta * 0.5f));
        corners[0].localPosition = new Vector2(corners[2].localPosition.x, target.sizeDelta.y + corners[2].localPosition.y);
        corners[3].localPosition = new Vector2(target.sizeDelta.x + corners[2].localPosition.x, corners[2].localPosition.y);
        corners[1].localPosition = new Vector2(target.sizeDelta.x + corners[2].localPosition.x, target.sizeDelta.y + corners[2].localPosition.y);
    }

    private RaycastHit2D RaycastScreenPoint(Vector2 inputPosition)
    {
        return Physics2D.Raycast(inputPosition, Vector2.up, 0.01f, layer);
    }

    public bool OnPointerDown(Vector2 inputPosition)
    {
        RaycastHit2D hit = RaycastScreenPoint(inputPosition);
        if (hit.transform != null)
        {
            for (int i = 0; i < corners.Length; i++)
                if (corners[i] == hit.transform)
                    selectedCornerIndex = i;

            if (selectedCornerIndex == 0)
            {
                yCornerIndex = 1;
                xCornerIndex = 2;
            }
            else if (selectedCornerIndex == 1)
            {
                yCornerIndex = 0;
                xCornerIndex = 3;
            }
            else if (selectedCornerIndex == 2)
            {
                yCornerIndex = 3;
                xCornerIndex = 0;
            }
            else if (selectedCornerIndex == 3)
            {
                yCornerIndex = 2;
                xCornerIndex = 1;
            }
            return true;
        }
        else
        {
            _gameObject.SetActive(false);
            return false;
        }
    }

    public bool OnPointer(Vector2 inputPosition, AG_ElementType elem)
    {
        if (selectedCornerIndex != -1)
        {
            ManageCornersPosition(inputPosition, elem);
            return true;
        } else return false;
    }

    public void OnPointerUp(Vector2 inputPosition, AG_ElementType elem)
    {
        if (editorM.snap)
            ManageCornersPosition(AG_Grid.inst.ChoseClosestPoint(inputPosition), elem);

        selectedCornerIndex = -1;

        if (elem.objectType == ObjectType.wall)
        {
            ((AG_Wall)elem).RestCollider();
            elem.transform.GetChild(0).GetComponent<AG_Wall>().RestCollider();
        }
    }

    private void ManageCornersPosition(Vector2 inputPosition, AG_ElementType target)
    {
        if (selectedCornerIndex == -1)
            return;

        corners[selectedCornerIndex].position = inputPosition;
        corners[xCornerIndex].localPosition = new Vector2(corners[selectedCornerIndex].localPosition.x, corners[xCornerIndex].localPosition.y);
        corners[yCornerIndex].localPosition = new Vector2(corners[yCornerIndex].localPosition.x, corners[selectedCornerIndex].localPosition.y);

        if (target.objectType == ObjectType.wall)
        {
            AG_Wall wall = (AG_Wall)target;

            float minX = Mathf.Min(corners[2].localPosition.x, corners[3].localPosition.x);
            float maxX = Mathf.Max(corners[2].localPosition.x, corners[3].localPosition.x);
            float sX = Mathf.Abs(maxX - minX);
            float minY = Mathf.Min(corners[2].localPosition.y, corners[0].localPosition.y);
            float maxY = Mathf.Max(corners[2].localPosition.y, corners[0].localPosition.y);
            float sY = Mathf.Abs(maxY - minY);

            wall._rect.position = corners[2].position + (wall._rect.rotation * (new Vector2(sX, sY) * 0.5f)); ;
            wall._rect.sizeDelta = new Vector2(sX, sY);
        }
    }
}